using System.Diagnostics;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Observation;
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
            config.BatchMode = false;
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

        var (traceId, embeddingId) = traceHelper.CreateTraceWithEmbedding(
            embeddingName: embeddingName,
            model: "text-embedding-3-small",
            provider: "openai",
            input: "Test input text");

        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(embeddingId);

        var observation = await client.GetObservationAsync(embeddingId);

        observation.ShouldNotBeNull();
        observation.Id.ShouldBe(embeddingId);
        observation.Name.ShouldBe(embeddingName);
        observation.Model.ShouldBe("text-embedding-3-small");
    }

    [Fact]
    public async Task ToolCallObservation_IsExportedWithArguments()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var toolCallName = $"tool-call-test-{Guid.NewGuid():N}";

        var (traceId, toolCallId) = traceHelper.CreateTraceWithToolCall(
            toolCallName: toolCallName,
            toolName: "get_weather",
            toolDescription: "Gets current weather",
            input: new { location = "San Francisco", unit = "celsius" },
            result: new { temperature = 18, conditions = "sunny" });

        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(toolCallId);

        var observation = await client.GetObservationAsync(toolCallId);

        observation.ShouldNotBeNull();
        observation.Id.ShouldBe(toolCallId);
        observation.Name.ShouldBe(toolCallName);
        observation.Input.ShouldNotBeNull();
        observation.Output.ShouldNotBeNull();
    }

    [Fact]
    public async Task AgentObservation_IsExportedWithId()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var agentName = $"agent-test-{Guid.NewGuid():N}";
        var agentId = $"agent-{Guid.NewGuid():N}";

        var (traceId, agentSpanId) = traceHelper.CreateTraceWithAgent(
            agentName: agentName,
            agentId: agentId,
            description: "A test research agent",
            input: "Research query",
            output: "Research results");

        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(agentSpanId);

        var observation = await client.GetObservationAsync(agentSpanId);

        observation.ShouldNotBeNull();
        observation.Id.ShouldBe(agentSpanId);
        observation.Name.ShouldBe(agentName);
        observation.Input.ShouldNotBeNull();
        observation.Output.ShouldNotBeNull();
    }

    [Fact]
    public async Task Generation_TokenUsage_IsExported()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var generationName = $"gen-tokens-{Guid.NewGuid():N}";

        var (traceId, generationId) = traceHelper.CreateTraceWithTokenUsage(
            generationName: generationName,
            model: "gpt-4-turbo",
            inputTokens: 150,
            outputTokens: 75);

        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        var observation = await client.GetObservationAsync(generationId);

        observation.ShouldNotBeNull();
        observation.Id.ShouldBe(generationId);
        observation.Name.ShouldBe(generationName);
        observation.Model.ShouldBe("gpt-4-turbo");
        observation.Type.ShouldBe("GENERATION");

        // Token usage should be captured
        observation.Usage.ShouldNotBeNull();
        observation.Usage.Input.ShouldBe(150);
        observation.Usage.Output.ShouldBe(75);
        observation.Usage.Total.ShouldBe(225);
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

        await traceHelper.WaitForTraceAsync(result.TraceId);

        var trace = await client.GetTraceAsync(result.TraceId);

        trace.ShouldNotBeNull();
        trace.UserId.ShouldBe(userId);
        trace.SessionId.ShouldBe(sessionId);
        trace.Tags.ShouldNotBeNull();
        trace.Tags!.ShouldContain("tag1");
        trace.Tags!.ShouldContain("tag2");
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

        await traceHelper.WaitForTraceAsync(result.TraceId);
        await traceHelper.WaitForObservationAsync(result.SpanId);
        await traceHelper.WaitForObservationAsync(result.GenerationId);

        // Get all observations for the trace
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            TraceId = result.TraceId,
            Page = 1,
            Limit = 50
        });

        observations.ShouldNotBeNull();
        observations.Data.ShouldNotBeNull();

        // ALL child observations should belong to the same trace
        foreach (var observation in observations.Data)
        {
            observation.TraceId.ShouldBe(result.TraceId);
        }

        // Verify the trace has the correct baggage-propagated attributes
        var trace = await client.GetTraceAsync(result.TraceId);
        trace.UserId.ShouldBe(userId);
        trace.SessionId.ShouldBe(sessionId);
        trace.Tags.ShouldNotBeNull();
        trace.Tags!.ShouldContain("baggage-tag1");
        trace.Tags!.ShouldContain("baggage-tag2");

        // Verify we have multiple observations linked to this trace
        observations.Data.Length.ShouldBeGreaterThanOrEqualTo(2);
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
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(span1Id);
        await traceHelper.WaitForObservationAsync(span2Id);
        await traceHelper.WaitForObservationAsync(generationId);

        // Verify all observations belong to the same trace
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            TraceId = traceId,
            Page = 1,
            Limit = 50
        });

        // All observations should be linked to the trace with baggage context
        observations.Data.Length.ShouldBeGreaterThanOrEqualTo(3);
        observations.Data.ShouldAllBe(o => o.TraceId == traceId);

        // Verify the parent trace has baggage-propagated attributes
        var resultTrace = await client.GetTraceAsync(traceId);
        resultTrace.UserId.ShouldBe(userId);
        resultTrace.SessionId.ShouldBe(sessionId);
        resultTrace.Tags.ShouldNotBeNull();
        resultTrace.Tags!.ShouldContain("level0");
    }

    [Fact]
    public async Task NestedHierarchy_MaintainsParentChildRelationships()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateNestedHierarchy();

        await traceHelper.WaitForTraceAsync(result.TraceId);
        await traceHelper.WaitForObservationAsync(result.SpanId);
        await traceHelper.WaitForObservationAsync(result.GenerationId);

        // Get all observations for the trace
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            TraceId = result.TraceId,
            Page = 1,
            Limit = 50
        });

        observations.ShouldNotBeNull();
        observations.Data.ShouldNotBeNull();

        // Verify we have the expected observations
        var spanObs = observations.Data.FirstOrDefault(o => o.Id == result.SpanId);
        var genObs = observations.Data.FirstOrDefault(o => o.Id == result.GenerationId);

        spanObs.ShouldNotBeNull();
        genObs.ShouldNotBeNull();

        // Verify parent-child relationships
        spanObs.Name.ShouldBe("parent-span");
        genObs.Name.ShouldBe("child-generation");

        // Generation should be child of span
        genObs.ParentObservationId.ShouldBe(result.SpanId);
    }

    [Fact]
    public async Task NestedHierarchy_AllObservationsBelongToSameTrace()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateNestedHierarchy();

        await traceHelper.WaitForTraceAsync(result.TraceId);
        await traceHelper.WaitForObservationAsync(result.SpanId);
        await traceHelper.WaitForObservationAsync(result.GenerationId);

        var spanObs = await client.GetObservationAsync(result.SpanId);
        var genObs = await client.GetObservationAsync(result.GenerationId);

        spanObs.TraceId.ShouldBe(result.TraceId);
        genObs.TraceId.ShouldBe(result.TraceId);
    }

    [Fact]
    public async Task SkipTrace_NothingExported()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateSkippedTrace();

        // Wait a bit to ensure export would have happened
        await Task.Delay(3000);

        // The trace should not exist
        var traceExists = false;
        try
        {
            await client.GetTraceAsync(traceId);
            traceExists = true;
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 404)
        {
            traceExists = false;
        }

        traceExists.ShouldBeFalse();
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
        await traceHelper.WaitForTraceAsync(traceId);

        var trace = await client.GetTraceAsync(traceId);
        trace.ShouldNotBeNull();
        trace.Name.ShouldBe(traceName);
    }

    [Fact]
    public void AddLangfuseExporter_ConfiguresTracerCorrectly()
    {
        var exception = Record.Exception(() =>
        {
            using var tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddLangfuseExporter(options =>
                {
                    options.Endpoint = _fixture.LangfuseBaseUrl;
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
                    options.Enabled = false;
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
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(embeddingId);
        await traceHelper.WaitForObservationAsync(generationId);
        await traceHelper.WaitForObservationAsync(spanId);
        await traceHelper.WaitForObservationAsync(eventId);

        // Verify all observations exist
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            TraceId = traceId,
            Page = 1,
            Limit = 50
        });

        observations.Data.Length.ShouldBeGreaterThanOrEqualTo(4);
        observations.Data.ShouldContain(o => o.Id == embeddingId);
        observations.Data.ShouldContain(o => o.Id == generationId);
        observations.Data.ShouldContain(o => o.Id == spanId);
        observations.Data.ShouldContain(o => o.Id == eventId);
    }
}
