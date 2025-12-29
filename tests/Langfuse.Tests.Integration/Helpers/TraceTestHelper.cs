using System.Diagnostics;
using Langfuse.Tests.Integration.Fixtures;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace Langfuse.Tests.Integration.Helpers;

/// <summary>
///     Helper class for creating test traces and observations for integration tests
///     using OpenTelemetry-based tracing.
/// </summary>
public class TraceTestHelper
{
    private readonly ILangfuseClient _client;
    private readonly LangfuseTestFixture _fixture;

    public TraceTestHelper(ILangfuseClient client, LangfuseTestFixture fixture)
    {
        _client = client;
        _fixture = fixture;
    }

    /// <summary>
    ///     Waits for a trace to become available in the API (handles eventual consistency)
    /// </summary>
    public async Task WaitForTraceAsync(string traceId, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                var trace = await _client.GetTraceAsync(traceId, cancellationToken);
                if (trace != null)
                {
                    return;
                }
            }
            catch (LangfuseApiException ex) when (ex.StatusCode == 404)
            {
                // Not found yet, wait and retry
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException($"Trace {traceId} did not become available within {timeout}");
    }

    /// <summary>
    ///     Waits for an observation to become available in the API (handles eventual consistency)
    /// </summary>
    public async Task WaitForObservationAsync(string observationId, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                var observation = await _client.GetObservationAsync(observationId, cancellationToken);
                if (observation != null)
                {
                    return;
                }
            }
            catch (LangfuseApiException ex) when (ex.StatusCode == 404)
            {
                // Not found yet, wait and retry
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException($"Observation {observationId} did not become available within {timeout}");
    }

    /// <summary>
    ///     Waits for a score to become available in the API with full data (handles eventual consistency)
    /// </summary>
    public async Task<ScoreModel> WaitForScoreAsync(string scoreId, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                var score = await _client.GetScoreAsync(scoreId, cancellationToken);
                if (score != null && !string.IsNullOrEmpty(score.Name))
                {
                    return score;
                }
            }
            catch (LangfuseApiException ex) when (ex.StatusCode == 404)
            {
                // Not found yet, wait and retry
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException($"Score {scoreId} did not become available within {timeout}");
    }

    /// <summary>
    ///     Waits for a session to become available in the API (handles eventual consistency)
    /// </summary>
    public async Task WaitForSessionAsync(string sessionId, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(30);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                var session = await _client.GetSessionAsync(sessionId, cancellationToken);
                if (session != null)
                {
                    return;
                }
            }
            catch (LangfuseApiException ex) when (ex.StatusCode == 404)
            {
                // Not found yet, wait and retry
            }

            await Task.Delay(500, cancellationToken);
        }

        throw new TimeoutException($"Session {sessionId} did not become available within {timeout}");
    }

    /// <summary>
    ///     Creates a simple trace with optional parameters using OpenTelemetry
    /// </summary>
    public string CreateTrace(
        string? name = null,
        string? sessionId = null,
        string? userId = null,
        string[]? tags = null,
        object? input = null,
        object? output = null,
        object? metadata = null)
    {
        var traceName = name ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            traceName,
            userId,
            sessionId,
            tags: tags,
            input: input,
            isRoot: true);

        if (output != null)
        {
            trace.SetOutput(output);
        }

        // Get the trace ID before disposing
        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");

        // Dispose trace (ends the activity)
        trace.Dispose();

        // Flush to ensure data is exported
        _fixture.FlushTraces();

        return traceId;
    }

    /// <summary>
    ///     Creates a trace with a span observation using OpenTelemetry
    /// </summary>
    public (string TraceId, string SpanId) CreateTraceWithSpan(
        string? traceName = null,
        string? spanName = null,
        string? sessionId = null,
        string? userId = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            userId,
            sessionId,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var span = trace.CreateSpan(spanName ?? "test-span", input: "span input");
        span.SetOutput("span output");

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var spanId = span.Activity?.SpanId.ToHexString() ??
                     throw new InvalidOperationException("Span activity not created");

        // Dispose span first, then trace
        span.Dispose();
        trace.Dispose();

        // Flush to ensure data is exported
        _fixture.FlushTraces();

        return (traceId, spanId);
    }

    /// <summary>
    ///     Creates a trace with a generation observation (LLM call) using OpenTelemetry
    /// </summary>
    public (string TraceId, string GenerationId) CreateTraceWithGeneration(
        string? traceName = null,
        string? generationName = null,
        string? model = null,
        string? sessionId = null,
        string? userId = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            userId,
            sessionId,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var generation = trace.CreateGeneration(
            generationName ?? "test-generation",
            model ?? "gpt-4",
            "test-provider",
            "prompt text");

        generation.SetOutput("model response");

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var generationId = generation.Activity?.SpanId.ToHexString() ??
                           throw new InvalidOperationException("Generation activity not created");

        // Dispose generation first, then trace
        generation.Dispose();
        trace.Dispose();

        // Flush to ensure data is exported
        _fixture.FlushTraces();

        return (traceId, generationId);
    }

    /// <summary>
    ///     Creates a trace with an event observation using OpenTelemetry
    /// </summary>
    public (string TraceId, string EventId) CreateTraceWithEvent(
        string? traceName = null,
        string? eventName = null,
        string? sessionId = null,
        string? userId = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            userId,
            sessionId,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var eventObs = trace.CreateEvent(eventName ?? "test-event", new { step = 1 }, new { status = "ok" });

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var eventId = eventObs.Activity?.SpanId.ToHexString() ??
                      throw new InvalidOperationException("Event activity not created");

        // Dispose event first, then trace
        eventObs.Dispose();
        trace.Dispose();

        // Flush to ensure data is exported
        _fixture.FlushTraces();

        return (traceId, eventId);
    }

    /// <summary>
    ///     Creates a trace with multiple observations (span, generation, event) using OpenTelemetry
    /// </summary>
    public TraceWithObservations CreateComplexTrace(
        string? traceName = null,
        string? sessionId = null,
        string? userId = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            userId,
            sessionId,
            tags: new[] { "integration-test", "complex" },
            input: "complex test input",
            isRoot: true);

        trace.SetOutput("complex test output");

        using var span = trace.CreateSpan("data-processing", input: "raw data");
        span.SetOutput("processed data");

        using var generation = trace.CreateGeneration("llm-call", "gpt-4", "test-provider", "prompt");
        generation.SetOutput("response");

        using var eventObs = trace.CreateEvent("checkpoint", new { step = 1 }, new { status = "complete" });

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var spanId = span.Activity?.SpanId.ToHexString() ??
                     throw new InvalidOperationException("Span activity not created");
        var generationId = generation.Activity?.SpanId.ToHexString() ??
                           throw new InvalidOperationException("Generation activity not created");
        var eventId = eventObs.Activity?.SpanId.ToHexString() ??
                      throw new InvalidOperationException("Event activity not created");

        // Dispose children first, then trace
        eventObs.Dispose();
        generation.Dispose();
        span.Dispose();
        trace.Dispose();

        // Flush to ensure data is exported
        _fixture.FlushTraces();

        return new TraceWithObservations
        {
            TraceId = traceId,
            SpanId = spanId,
            GenerationId = generationId,
            EventId = eventId
        };
    }

    /// <summary>
    ///     Creates a trace with a skipped span observation using OpenTelemetry.
    ///     The span will be marked as skipped and should not be exported to Langfuse.
    /// </summary>
    public (string TraceId, string SkippedSpanId) CreateTraceWithSkippedSpan(
        string? traceName = null,
        string? spanName = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var span = trace.CreateSpan(spanName ?? "skipped-span", input: "span input");
        span.SetOutput("span output");
        span.Skip();

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var spanId = span.Activity?.SpanId.ToHexString() ??
                     throw new InvalidOperationException("Span activity not created");

        span.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return (traceId, spanId);
    }

    /// <summary>
    ///     Creates a trace with a skipped generation observation using OpenTelemetry.
    ///     The generation will be marked as skipped and should not be exported to Langfuse.
    /// </summary>
    public (string TraceId, string SkippedGenerationId) CreateTraceWithSkippedGeneration(
        string? traceName = null,
        string? generationName = null,
        string? model = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var generation = trace.CreateGeneration(
            generationName ?? "skipped-generation",
            model ?? "gpt-4",
            "test-provider",
            "prompt text");

        generation.SetOutput("model response");
        generation.Skip();

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var generationId = generation.Activity?.SpanId.ToHexString() ??
                           throw new InvalidOperationException("Generation activity not created");

        generation.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return (traceId, generationId);
    }

    /// <summary>
    ///     Creates a trace with both skipped and non-skipped observations.
    /// </summary>
    public (string TraceId, string ActiveSpanId, string SkippedSpanId) CreateTraceWithMixedObservations(
        string? traceName = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var activeSpan = trace.CreateSpan("active-span", input: "active span input");
        activeSpan.SetOutput("active span output");

        using var skippedSpan = trace.CreateSpan("skipped-span", input: "skipped span input");
        skippedSpan.SetOutput("skipped span output");
        skippedSpan.Skip();

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var activeSpanId = activeSpan.Activity?.SpanId.ToHexString() ??
                           throw new InvalidOperationException("Active span activity not created");
        var skippedSpanId = skippedSpan.Activity?.SpanId.ToHexString() ??
                            throw new InvalidOperationException("Skipped span activity not created");

        skippedSpan.Dispose();
        activeSpan.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return (traceId, activeSpanId, skippedSpanId);
    }

    /// <summary>
    ///     Creates a trace with an embedding observation using OpenTelemetry
    /// </summary>
    public (string TraceId, string EmbeddingId) CreateTraceWithEmbedding(
        string? traceName = null,
        string? embeddingName = null,
        string? model = null,
        string? provider = null,
        object? input = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var embedding = trace.CreateEmbedding(
            embeddingName ?? "test-embedding",
            model ?? "text-embedding-3-small",
            provider ?? "openai",
            input ?? "Text to embed");

        embedding.SetResponse(new GenAiResponse { InputTokens = 15 });

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var embeddingId = embedding.Activity?.SpanId.ToHexString() ??
                          throw new InvalidOperationException("Embedding activity not created");

        embedding.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return (traceId, embeddingId);
    }

    /// <summary>
    ///     Creates a trace with a tool call observation using OpenTelemetry
    /// </summary>
    public (string TraceId, string ToolCallId) CreateTraceWithToolCall(
        string? traceName = null,
        string? toolCallName = null,
        string? toolName = null,
        string? toolDescription = null,
        object? input = null,
        object? result = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var toolCall = trace.CreateToolCall(
            toolCallName ?? "test-tool-call",
            toolName ?? "get_weather",
            toolDescription ?? "Gets current weather",
            input: input ?? new { location = "NYC" });

        toolCall.SetResult(result ?? new { temperature = 72, unit = "F" });

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var toolCallId = toolCall.Activity?.SpanId.ToHexString() ??
                         throw new InvalidOperationException("Tool call activity not created");

        toolCall.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return (traceId, toolCallId);
    }

    /// <summary>
    ///     Creates a trace with an agent observation using OpenTelemetry
    /// </summary>
    public (string TraceId, string AgentId) CreateTraceWithAgent(
        string? traceName = null,
        string? agentName = null,
        string? agentId = null,
        string? description = null,
        object? input = null,
        object? output = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var agent = trace.CreateAgent(
            agentName ?? "test-agent",
            agentId ?? "agent-123",
            description ?? "A test agent",
            input ?? "Agent input");

        agent.SetOutput(output ?? "Agent output");

        var traceIdStr = trace.TraceActivity?.TraceId.ToHexString() ??
                         throw new InvalidOperationException("Trace activity not created");
        var agentSpanId = agent.Activity?.SpanId.ToHexString() ??
                          throw new InvalidOperationException("Agent activity not created");

        agent.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return (traceIdStr, agentSpanId);
    }

    /// <summary>
    ///     Creates a trace with a generation that has token usage metadata
    /// </summary>
    public (string TraceId, string GenerationId) CreateTraceWithTokenUsage(
        string? traceName = null,
        string? generationName = null,
        string? model = null,
        int inputTokens = 100,
        int outputTokens = 50)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        using var generation = trace.CreateGeneration(
            generationName ?? "test-generation",
            model ?? "gpt-4",
            "openai",
            "prompt text");

        generation.SetResponse(new GenAiResponse
        {
            Model = model ?? "gpt-4",
            InputTokens = inputTokens,
            OutputTokens = outputTokens,
            FinishReasons = ["stop"],
            Completion = "Generated response"
        });

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var generationId = generation.Activity?.SpanId.ToHexString() ??
                           throw new InvalidOperationException("Generation activity not created");

        generation.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return (traceId, generationId);
    }

    /// <summary>
    ///     Creates a trace with a nested hierarchy: trace -> span -> generation
    /// </summary>
    public NestedHierarchyResult CreateNestedHierarchy(
        string? traceName = null,
        string? userId = null,
        string? sessionId = null,
        string[]? tags = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            userId,
            sessionId,
            tags: tags,
            input: "trace input",
            isRoot: true);

        trace.SetOutput("trace output");

        // First level: span
        using var span = trace.CreateSpan("parent-span", input: "span input");
        span.SetOutput("span output");

        // Second level: generation inside span
        using var generation = trace.CreateGeneration("child-generation", "gpt-4", "openai", "prompt");
        generation.SetMetadata("test", "test-value");
        generation.SetResponse(new GenAiResponse
        {
            Model = "gpt-4",
            InputTokens = 50,
            OutputTokens = 25,
            Completion = "response"
        });

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");
        var spanId = span.Activity?.SpanId.ToHexString() ??
                     throw new InvalidOperationException("Span activity not created");
        var generationId = generation.Activity?.SpanId.ToHexString() ??
                           throw new InvalidOperationException("Generation activity not created");

        // Get parent observation IDs for hierarchy verification
        var traceSpanId = trace.TraceActivity?.SpanId.ToHexString();
        var spanParentId = span.Activity?.ParentSpanId.ToHexString();
        var generationParentId = generation.Activity?.ParentSpanId.ToHexString();

        generation.Dispose();
        span.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return new NestedHierarchyResult
        {
            TraceId = traceId,
            TraceSpanId = traceSpanId!,
            SpanId = spanId,
            SpanParentSpanId = spanParentId!,
            GenerationId = generationId,
            GenerationParentSpanId = generationParentId!
        };
    }

    /// <summary>
    ///     Creates a trace and skips the entire trace (nothing should be exported)
    /// </summary>
    public string CreateSkippedTrace(string? traceName = null)
    {
        var actualTraceName = traceName ?? $"test-trace-{Guid.NewGuid():N}";

        using var trace = new OtelLangfuseTrace(
            actualTraceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        // Add some observations before skipping
        using var span = trace.CreateSpan("child-span", input: "span input");
        span.SetOutput("span output");

        var traceId = trace.TraceActivity?.TraceId.ToHexString() ??
                      throw new InvalidOperationException("Trace activity not created");

        // Skip the entire trace
        trace.Skip();

        span.Dispose();
        trace.Dispose();

        _fixture.FlushTraces();

        return traceId;
    }

    /// <summary>
    ///     Checks if an observation exists in Langfuse (returns true if found, false if 404)
    /// </summary>
    public async Task<bool> ObservationExistsAsync(string observationId, TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(10);
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                var observation = await _client.GetObservationAsync(observationId, cancellationToken);
                if (observation != null)
                {
                    return true;
                }
            }
            catch (LangfuseApiException ex) when (ex.StatusCode == 404)
            {
                // Not found, continue checking
            }

            await Task.Delay(500, cancellationToken);
        }

        return false;
    }
}

/// <summary>
///     Result object containing IDs from a complex trace with multiple observations
/// </summary>
public class TraceWithObservations
{
    public required string TraceId { get; init; }
    public required string SpanId { get; init; }
    public required string GenerationId { get; init; }
    public required string EventId { get; init; }
}

/// <summary>
///     Result object containing IDs and parent relationships from a nested hierarchy
/// </summary>
public class NestedHierarchyResult
{
    public required string TraceId { get; init; }
    public required string TraceSpanId { get; init; }
    public required string SpanId { get; init; }
    public required string SpanParentSpanId { get; init; }
    public required string GenerationId { get; init; }
    public required string GenerationParentSpanId { get; init; }
}