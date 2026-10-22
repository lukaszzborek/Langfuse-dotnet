using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.ObservationV2;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace Langfuse.Tests.Integration;

/// <summary>
///     Integration tests for OpenTelemetry implementation.
///     Tests observation types, token usage, baggage propagation, hierarchy, skip functionality, and DI registration.
/// </summary>
[Collection(LangfuseTestCollection.Name)]
public class OpenTelemetryTests
{
    private readonly LangfuseTestFixture _fixture;

    public OpenTelemetryTests(LangfuseTestFixture fixture)
    {
        _fixture = fixture;
    }

    private ILangfuseClient CreateClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLangfuse(config =>
        {
            config.Url = _fixture.LangfuseBaseUrl;
            config.PublicKey = _fixture.PublicKey;
            config.SecretKey = _fixture.SecretKey;
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ILangfuseClient>();
    }

    private TraceTestHelper CreateTraceHelper(ILangfuseClient client)
    {
        return new TraceTestHelper(client, _fixture);
    }

    [Fact]
    public async Task EmbeddingObservation_IsExportedWithCorrectMetadata()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var embeddingName = $"embedding-test-{Guid.NewGuid():N}";

        var (_, embeddingId) = traceHelper.CreateTraceWithEmbedding(
            embeddingName: embeddingName,
            model: "text-embedding-3-small",
            provider: "openai",
            input: "Test input text");

        var observation = await traceHelper.GetObservationV2Async(embeddingId, "core,basic,model", cancellationToken: TestContext.Current.CancellationToken);

        TraceTestHelper.GetString(observation, "id").ShouldBe(embeddingId);
        TraceTestHelper.GetString(observation, "name").ShouldBe(embeddingName);
        TraceTestHelper.GetString(observation, "model").ShouldBe("text-embedding-3-small");
    }

    [Fact]
    public async Task ToolCallObservation_IsExportedWithArguments()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var toolCallName = $"tool-call-test-{Guid.NewGuid():N}";

        var (_, toolCallId) = traceHelper.CreateTraceWithToolCall(
            toolCallName: toolCallName,
            toolName: "get_weather",
            toolDescription: "Gets current weather",
            input: new { location = "San Francisco", unit = "celsius" },
            result: new { temperature = 18, conditions = "sunny" });

        var observation = await traceHelper.GetObservationV2Async(toolCallId, "core,basic,io", cancellationToken: TestContext.Current.CancellationToken);

        TraceTestHelper.GetString(observation, "id").ShouldBe(toolCallId);
        TraceTestHelper.GetString(observation, "name").ShouldBe("get_weather");
        TraceTestHelper.GetString(observation, "input").ShouldNotBeNullOrEmpty();
        TraceTestHelper.GetString(observation, "output").ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task AgentObservation_IsExportedWithId()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var agentName = $"agent-test-{Guid.NewGuid():N}";
        var agentId = $"agent-{Guid.NewGuid():N}";

        var (_, agentSpanId) = traceHelper.CreateTraceWithAgent(
            agentName: agentName,
            agentId: agentId,
            description: "A test research agent",
            input: "Research query",
            output: "Research results");

        var observation = await traceHelper.GetObservationV2Async(agentSpanId, "core,basic,io", cancellationToken: TestContext.Current.CancellationToken);

        TraceTestHelper.GetString(observation, "id").ShouldBe(agentSpanId);
        TraceTestHelper.GetString(observation, "name").ShouldBe(agentName);
        TraceTestHelper.GetString(observation, "input").ShouldNotBeNullOrEmpty();
        TraceTestHelper.GetString(observation, "output").ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task Generation_TokenUsage_IsExported()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var generationName = $"gen-tokens-{Guid.NewGuid():N}";

        var (_, generationId) = traceHelper.CreateTraceWithTokenUsage(
            generationName: generationName,
            model: "gpt-4-turbo",
            inputTokens: 150,
            outputTokens: 75);

        var observation = await traceHelper.GetObservationV2Async(generationId, "core,basic,model,usage", cancellationToken: TestContext.Current.CancellationToken);

        TraceTestHelper.GetString(observation, "id").ShouldBe(generationId);
        TraceTestHelper.GetString(observation, "name").ShouldBe(generationName);
        TraceTestHelper.GetString(observation, "model").ShouldBe("gpt-4-turbo");
        TraceTestHelper.GetString(observation, "type").ShouldBe("GENERATION");

        // Token usage should be captured
        var usage = TraceTestHelper.GetElement(observation, "usageDetails");
        usage.ShouldNotBeNull();
        usage.Value.GetProperty("input").GetInt64().ShouldBe(150);
        usage.Value.GetProperty("output").GetInt64().ShouldBe(75);
        usage.Value.GetProperty("total").GetInt64().ShouldBe(225);
    }

    [Fact]
    public async Task BaggageContext_PropagesToTrace()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var userId = $"user-{Guid.NewGuid():N}";
        var sessionId = $"session-{Guid.NewGuid():N}";
        var tags = new[] { "tag1", "tag2" };

        var result = traceHelper.CreateNestedHierarchy(
            userId: userId,
            sessionId: sessionId,
            tags: tags);

        var observation = await traceHelper.GetObservationV2Async(result.SpanId, "core,basic,trace_context", cancellationToken: TestContext.Current.CancellationToken);

        TraceTestHelper.GetString(observation, "userId").ShouldBe(userId);
        TraceTestHelper.GetString(observation, "sessionId").ShouldBe(sessionId);
        var traceTags = GetTags(observation);
        traceTags.ShouldContain("tag1");
        traceTags.ShouldContain("tag2");
    }

    [Fact]
    public async Task BaggageContext_AllChildObservationsHaveTags()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var userId = $"user-baggage-{Guid.NewGuid():N}";
        var sessionId = $"session-baggage-{Guid.NewGuid():N}";
        var tags = new[] { "baggage-tag1", "baggage-tag2" };

        // Create a trace with multiple nested observations
        var result = traceHelper.CreateNestedHierarchy(
            userId: userId,
            sessionId: sessionId,
            tags: tags);

        await traceHelper.WaitForObservationAsync(result.SpanId, cancellationToken: TestContext.Current.CancellationToken);
        await traceHelper.WaitForObservationAsync(result.GenerationId, cancellationToken: TestContext.Current.CancellationToken);

        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = result.TraceId,
            Fields = "core,basic,trace_context",
            Limit = 50
        }, TestContext.Current.CancellationToken);

        // Verify we have multiple observations linked to this trace
        observations.Data.Count.ShouldBeGreaterThanOrEqualTo(2);

        // ALL child observations should carry the baggage-propagated attributes
        foreach (var observation in observations.Data)
        {
            TraceTestHelper.GetString(observation, "traceId").ShouldBe(result.TraceId);
            TraceTestHelper.GetString(observation, "userId").ShouldBe(userId);
            TraceTestHelper.GetString(observation, "sessionId").ShouldBe(sessionId);
            var observationTags = GetTags(observation);
            observationTags.ShouldContain("baggage-tag1");
            observationTags.ShouldContain("baggage-tag2");
        }
    }

    [Fact]
    public async Task BaggageContext_NestedObservationsInheritTraceContext()
    {
        var client = CreateClient();
        var userId = $"user-inherit-{Guid.NewGuid():N}";
        var sessionId = $"session-inherit-{Guid.NewGuid():N}";
        var traceName = $"baggage-inherit-{Guid.NewGuid():N}";

        // Create trace with deep nesting to verify baggage propagates through all levels
        using var trace = new OtelLangfuseTrace(
            traceName,
            userId,
            sessionId,
            tags: new[] { "level0" },
            input: "root input",
            isRoot: true);

        // Level 1: Span
        using var span1 = trace.CreateSpan("level1-span", input: "span1 input");
        span1.SetOutput("span1 output");

        // Level 2: Nested span inside span
        using var span2 = trace.CreateSpan("level2-span", input: "span2 input");
        span2.SetOutput("span2 output");

        // Level 3: Generation inside nested span
        using var generation = trace.CreateGeneration("level3-generation", "gpt-4", "openai", "prompt");
        generation.SetResponse(new GenAiResponse
        {
            InputTokens = 10,
            OutputTokens = 5,
            Completion = "response"
        });

        var traceId = trace.TraceActivity?.TraceId.ToHexString()!;
        var span1Id = span1.Activity?.SpanId.ToHexString()!;
        var span2Id = span2.Activity?.SpanId.ToHexString()!;
        var generationId = generation.Activity?.SpanId.ToHexString()!;

        generation.Dispose();
        span2.Dispose();
        span1.Dispose();
        trace.Dispose();
        _fixture.FlushTraces();

        // Wait for all observations
        var traceHelper = CreateTraceHelper(client);
        await traceHelper.WaitForObservationAsync(span1Id, cancellationToken: TestContext.Current.CancellationToken);
        await traceHelper.WaitForObservationAsync(span2Id, cancellationToken: TestContext.Current.CancellationToken);
        await traceHelper.WaitForObservationAsync(generationId, cancellationToken: TestContext.Current.CancellationToken);

        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = traceId,
            Fields = "core,basic,trace_context",
            Limit = 50
        }, TestContext.Current.CancellationToken);

        // All observations should be linked to the trace with baggage context
        observations.Data.Count.ShouldBeGreaterThanOrEqualTo(3);
        observations.Data.ShouldAllBe(o => TraceTestHelper.GetString(o, "traceId") == traceId);
        observations.Data.ShouldAllBe(o => TraceTestHelper.GetString(o, "userId") == userId);
        observations.Data.ShouldAllBe(o => TraceTestHelper.GetString(o, "sessionId") == sessionId);
        observations.Data.ShouldAllBe(o => GetTags(o).Contains("level0"));
    }

    [Fact]
    public async Task NestedHierarchy_MaintainsParentChildRelationships()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateNestedHierarchy();

        var spanObs = await traceHelper.GetObservationV2Async(result.SpanId, cancellationToken: TestContext.Current.CancellationToken);
        var genObs = await traceHelper.GetObservationV2Async(result.GenerationId, cancellationToken: TestContext.Current.CancellationToken);

        // Verify parent-child relationships
        TraceTestHelper.GetString(spanObs, "name").ShouldBe("parent-span");
        TraceTestHelper.GetString(genObs, "name").ShouldBe("child-generation");

        // Generation should be child of span
        TraceTestHelper.GetString(genObs, "parentObservationId").ShouldBe(result.SpanId);
    }

    [Fact]
    public async Task NestedHierarchy_AllObservationsBelongToSameTrace()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateNestedHierarchy();

        var spanObs = await traceHelper.GetObservationV2Async(result.SpanId, cancellationToken: TestContext.Current.CancellationToken);
        var genObs = await traceHelper.GetObservationV2Async(result.GenerationId, cancellationToken: TestContext.Current.CancellationToken);

        TraceTestHelper.GetString(spanObs, "traceId").ShouldBe(result.TraceId);
        TraceTestHelper.GetString(genObs, "traceId").ShouldBe(result.TraceId);
    }

    [Fact]
    public async Task SkipTrace_NothingExported()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateSkippedTrace();

        // Wait a bit to ensure export would have happened
        await Task.Delay(3000, TestContext.Current.CancellationToken);

        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = traceId,
            Limit = 1
        }, TestContext.Current.CancellationToken);

        observations.Data.ShouldBeEmpty();
    }

    [Fact]
    public void AddLangfuseTracing_RegistersIOtelLangfuseTrace()
    {
        var services = new ServiceCollection();
        services.AddLangfuseTracing();

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var trace = scope.ServiceProvider.GetService<IOtelLangfuseTrace>();

        trace.ShouldNotBeNull();
        trace.ShouldBeOfType<OtelLangfuseTrace>();
    }

    [Fact]
    public void AddLangfuseTracingNoOp_RegistersNullImplementation()
    {
        var services = new ServiceCollection();
        services.AddLangfuseTracingNoOp();

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var trace = scope.ServiceProvider.GetService<IOtelLangfuseTrace>();

        trace.ShouldNotBeNull();
        trace.ShouldBeOfType<NullOtelLangfuseTrace>();
    }

    [Fact]
    public async Task DiRegistration_TracesWorkWithInjectedTrace()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // Create a service collection with Langfuse tracing
        var services = new ServiceCollection();
        services.AddLangfuseTracing();

        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var injectedTrace = scope.ServiceProvider.GetRequiredService<IOtelLangfuseTrace>();

        // Start a trace using the injected service
        var traceName = $"di-trace-{Guid.NewGuid():N}";
        injectedTrace.StartTrace(traceName, isRoot: true);
        injectedTrace.SetInput("test input");
        injectedTrace.SetOutput("test output");

        var traceId = injectedTrace.TraceActivity?.TraceId.ToHexString();
        traceId.ShouldNotBeNull();

        injectedTrace.Dispose();
        _fixture.FlushTraces();

        // Verify the trace was exported
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);

        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = traceId,
            Fields = "core,basic,trace_context",
            Limit = 50
        }, TestContext.Current.CancellationToken);

        observations.Data.ShouldContain(o => TraceTestHelper.GetString(o, "traceName") == traceName);
    }

    [Fact]
    public void AddLangfuseExporter_ConfiguresTracerCorrectly()
    {
        var exception = Record.Exception(() =>
        {
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddLangfuseExporter(options =>
                {
                    options.Url = _fixture.LangfuseBaseUrl;
                    options.PublicKey = _fixture.PublicKey;
                    options.SecretKey = _fixture.SecretKey;
                    options.OnlyGenAiActivities = true;
                })
                .Build();
        });

        exception.ShouldBeNull();
    }

    [Fact]
    public void AddLangfuseExporter_WhenDisabled_DoesNotThrow()
    {
        var exception = Record.Exception(() =>
        {
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddLangfuseExporter(options =>
                {
                    options.EnableOpenTelemetryExporter = false;
                    // No credentials needed when disabled
                })
                .Build();
        });

        exception.ShouldBeNull();
    }

    [Fact]
    public async Task TraceWithMultipleObservationTypes_AllExported()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var traceName = $"multi-obs-{Guid.NewGuid():N}";

        // Create a trace with multiple observation types
        using var trace = new OtelLangfuseTrace(
            traceName,
            input: "test input",
            isRoot: true);

        trace.SetOutput("test output");

        // Add embedding
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-3-small", "openai", "embed text");
        embedding.SetResponse(new GenAiResponse { InputTokens = 10 });

        // Add generation
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "openai", "prompt");
        generation.SetResponse(new GenAiResponse
        {
            InputTokens = 50,
            OutputTokens = 25,
            Completion = "response"
        });

        // Add span
        using var span = trace.CreateSpan("test-span", input: "span input");
        span.SetOutput("span output");

        // Add event
        using var eventObs = trace.CreateEvent("test-event", new { step = 1 }, new { status = "ok" });

        var traceId = trace.TraceActivity?.TraceId.ToHexString()!;
        var embeddingId = embedding.Activity?.SpanId.ToHexString()!;
        var generationId = generation.Activity?.SpanId.ToHexString()!;
        var spanId = span.Activity?.SpanId.ToHexString()!;
        var eventId = eventObs.Activity?.SpanId.ToHexString()!;

        eventObs.Dispose();
        span.Dispose();
        generation.Dispose();
        embedding.Dispose();
        trace.Dispose();
        _fixture.FlushTraces();

        // Wait for all observations
        await traceHelper.WaitForObservationAsync(embeddingId, cancellationToken: TestContext.Current.CancellationToken);
        await traceHelper.WaitForObservationAsync(generationId, cancellationToken: TestContext.Current.CancellationToken);
        await traceHelper.WaitForObservationAsync(spanId, cancellationToken: TestContext.Current.CancellationToken);
        await traceHelper.WaitForObservationAsync(eventId, cancellationToken: TestContext.Current.CancellationToken);

        // Verify all observations exist
        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = traceId,
            Limit = 50
        }, TestContext.Current.CancellationToken);

        observations.Data.Count.ShouldBeGreaterThanOrEqualTo(4);
        observations.Data.ShouldContain(o => TraceTestHelper.GetString(o, "id") == embeddingId);
        observations.Data.ShouldContain(o => TraceTestHelper.GetString(o, "id") == generationId);
        observations.Data.ShouldContain(o => TraceTestHelper.GetString(o, "id") == spanId);
        observations.Data.ShouldContain(o => TraceTestHelper.GetString(o, "id") == eventId);
    }

    private static string[] GetTags(Dictionary<string, object?> observation)
    {
        var tags = TraceTestHelper.GetElement(observation, "tags");
        return tags?.EnumerateArray().Select(t => t.GetString()!).ToArray() ?? [];
    }
}
