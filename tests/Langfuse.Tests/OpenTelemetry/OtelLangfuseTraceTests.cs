using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using Shouldly;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class OtelLangfuseTraceTests : IDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly ConcurrentBag<Activity> _capturedActivities;
    private readonly ActivityListener _listener;

    public OtelLangfuseTraceTests()
    {
        _activitySource = new ActivitySource("TestSource");
        _capturedActivities = new ConcurrentBag<Activity>();

        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Langfuse" || source.Name == "TestSource",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => _capturedActivities.Add(activity)
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public void Dispose()
    {
        // Clear Baggage after each test
        Baggage.ClearBaggage();
        _listener.Dispose();
        _activitySource.Dispose();
    }


    [Fact]
    public void Constructor_WithTraceName_CreatesTraceActivity()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        trace.TraceActivity.ShouldNotBeNull();
        trace.TraceActivity.GetTagItem(LangfuseAttributes.TraceName).ShouldBe("test-trace");
    }

    [Fact]
    public void Constructor_WithUserId_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", "user-123");

        trace.TraceActivity?.GetTagItem(LangfuseAttributes.UserId).ShouldBe("user-123");
        Baggage.GetBaggage(LangfuseBaggageKeys.UserId).ShouldBe("user-123");
    }

    [Fact]
    public void Constructor_WithSessionId_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", sessionId: "session-456");

        trace.TraceActivity?.GetTagItem(LangfuseAttributes.SessionId).ShouldBe("session-456");
        Baggage.GetBaggage(LangfuseBaggageKeys.SessionId).ShouldBe("session-456");
    }

    [Fact]
    public void Constructor_WithVersion_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", version: "1.0.0");

        trace.TraceActivity?.GetTagItem(LangfuseAttributes.Version).ShouldBe("1.0.0");
        Baggage.GetBaggage(LangfuseBaggageKeys.Version).ShouldBe("1.0.0");
    }

    [Fact]
    public void Constructor_WithRelease_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", release: "prod-1");

        Baggage.GetBaggage(LangfuseBaggageKeys.Release).ShouldBe("prod-1");
    }

    [Fact]
    public void Constructor_WithTags_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", tags: ["tag1", "tag2"]);

        var tagsJson = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceTags) as string;
        tagsJson.ShouldNotBeNull();
        tagsJson.ShouldContain("tag1");
        tagsJson.ShouldContain("tag2");

        var baggageTags = Baggage.GetBaggage(LangfuseBaggageKeys.Tags);
        baggageTags.ShouldNotBeNull();
        baggageTags.ShouldContain("tag1");
        baggageTags.ShouldContain("tag2");
    }

    [Fact]
    public void Constructor_WithInput_SetsTraceInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace", input: new { query = "test query" });

        var inputJson = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        inputJson.ShouldNotBeNull();
        inputJson.ShouldContain("test query");
    }

    [Fact]
    public void Constructor_WithIsRoot_CreatesNewTraceId()
    {
        using var parentActivity = _activitySource.StartActivity("parent");
        parentActivity.ShouldNotBeNull();

        using var trace = new OtelLangfuseTrace("test-trace", isRoot: true);

        trace.TraceActivity.ShouldNotBeNull();
        trace.TraceActivity.TraceId.ShouldNotBe(parentActivity.TraceId);
    }

    [Fact]
    public void Constructor_WithCustomActivitySource_UsesProvidedSource()
    {
        var customSource = new ActivitySource("CustomSource");
        var customListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "CustomSource",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(customListener);

        try
        {
            using var trace = new OtelLangfuseTrace(customSource, "test-trace");

            trace.TraceActivity.ShouldNotBeNull();
            trace.TraceActivity.Source.Name.ShouldBe("CustomSource");
        }
        finally
        {
            customListener.Dispose();
            customSource.Dispose();
        }
    }


    [Fact]
    public void Dispose_ClearsBaggageContext()
    {
        var trace = new OtelLangfuseTrace(
            "test-trace",
            "user-123",
            "session-456",
            "1.0.0",
            "prod-1",
            ["tag1"]);

        Baggage.GetBaggage(LangfuseBaggageKeys.UserId).ShouldNotBeNull();

        trace.Dispose();

        Baggage.GetBaggage(LangfuseBaggageKeys.UserId).ShouldBeNull();
        Baggage.GetBaggage(LangfuseBaggageKeys.SessionId).ShouldBeNull();
        Baggage.GetBaggage(LangfuseBaggageKeys.Version).ShouldBeNull();
        Baggage.GetBaggage(LangfuseBaggageKeys.Release).ShouldBeNull();
        Baggage.GetBaggage(LangfuseBaggageKeys.Tags).ShouldBeNull();
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        var trace = new OtelLangfuseTrace("test-trace");

        trace.Dispose();
        trace.Dispose();
        trace.Dispose();
    }


    [Fact]
    public void SetTraceName_UpdatesTraceNameTag()
    {
        using var trace = new OtelLangfuseTrace("initial-name");

        trace.SetTraceName("updated-name");

        trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceName).ShouldBe("updated-name");
    }

    [Fact]
    public void SetTraceName_UpdatesDisplayName()
    {
        using var trace = new OtelLangfuseTrace("initial-name");

        trace.SetTraceName("updated-name");

        trace.TraceActivity?.DisplayName.ShouldBe("updated-name");
    }


    [Fact]
    public void SetInput_SetsTraceAndObservationInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        trace.SetInput(new { query = "test input" });

        var traceInput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        var observationInput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        traceInput.ShouldNotBeNull();
        observationInput.ShouldNotBeNull();
        traceInput.ShouldContain("test input");
        traceInput.ShouldBe(observationInput);
    }

    [Fact]
    public void SetOutput_SetsTraceAndObservationOutput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");


        trace.SetOutput(new { result = "test output" });

        var traceOutput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        var observationOutput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        traceOutput.ShouldNotBeNull();
        observationOutput.ShouldNotBeNull();
        traceOutput.ShouldContain("test output");
        traceOutput.ShouldBe(observationOutput);
    }

    [Fact]
    public void CreateSpan_ReturnsOtelSpan()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        using var span = trace.CreateSpan("test-span");

        span.ShouldNotBeNull();
        span.ShouldBeOfType<OtelSpan>();
    }

    [Fact]
    public void CreateSpan_WithType_SetsSpanType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        using var span = trace.CreateSpan("test-span", "retrieval");

        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        spanActivity.ShouldNotBeNull();
        spanActivity.GetTagItem("span.type").ShouldBe("retrieval");
    }

    [Fact]
    public void CreateSpan_WithDescription_SetsDescription()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        using var span = trace.CreateSpan("test-span", description: "Test description");

        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        spanActivity.ShouldNotBeNull();
        spanActivity.GetTagItem("span.description").ShouldBe("Test description");
    }

    [Fact]
    public void CreateSpan_WithInput_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        var exception = Record.Exception(() =>
        {
            using var span = trace.CreateSpan("test-span", input: new { data = "test" });
        });
        exception.ShouldBeNull();
    }

    [Fact]
    public void CreateSpan_WithConfigureAction_InvokesAction()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        var configureInvoked = false;

        using var span = trace.CreateSpan("test-span", configure: s =>
        {
            configureInvoked = true;
            s.SetMetadata("custom_key", "custom_value");
        });

        configureInvoked.ShouldBeTrue();
        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key").ShouldBe("custom_value");
    }

    [Fact]
    public void CreateGeneration_ReturnsOtelGeneration()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.ShouldNotBeNull();
        generation.ShouldBeOfType<OtelGeneration>();
    }

    [Fact]
    public void CreateGeneration_SetsModelAndProvider()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "openai");

        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(GenAiAttributes.RequestModel).ShouldBe("gpt-4");
        genActivity.GetTagItem(GenAiAttributes.ProviderName).ShouldBe("openai");
    }

    [Fact]
    public void CreateGeneration_WithInput_SetsObservationInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation =
            trace.CreateGeneration("test-generation", "gpt-4", "test-provider", new { prompt = "Hello" });

        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        genActivity.ShouldNotBeNull();
        var inputJson = genActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        inputJson.ShouldNotBeNull();
        inputJson.ShouldContain("Hello");
    }

    [Fact]
    public void CreateGeneration_SetsGenerationObservationType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(LangfuseAttributes.ObservationType)
            .ShouldBe(LangfuseAttributes.ObservationTypeGeneration);
    }


    [Fact]
    public void CreateToolCall_ReturnsOtelToolCall()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        toolCall.ShouldNotBeNull();
        toolCall.ShouldBeOfType<OtelToolCall>();
    }

    [Fact]
    public void CreateToolCall_SetsToolName()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        toolActivity.ShouldNotBeNull();
        toolActivity.GetTagItem(GenAiAttributes.ToolName).ShouldBe("get_weather");
    }

    [Fact]
    public void CreateToolCall_WithDescription_SetsToolDescription()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall =
            trace.CreateToolCall("test-tool-call", "get_weather", "Gets current weather");

        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        toolActivity.ShouldNotBeNull();
        toolActivity.GetTagItem(GenAiAttributes.ToolDescription).ShouldBe("Gets current weather");
    }

    [Fact]
    public void CreateToolCall_WithCustomToolType_SetsToolType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather", toolType: "api");

        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        toolActivity.ShouldNotBeNull();
        toolActivity.GetTagItem(GenAiAttributes.ToolType).ShouldBe("api");
    }

    [Fact]
    public void CreateToolCall_WithInput_SetsToolCallArguments()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall =
            trace.CreateToolCall("test-tool-call", "get_weather", input: new { location = "NYC" });

        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        toolActivity.ShouldNotBeNull();
        var argsJson = toolActivity.GetTagItem(GenAiAttributes.ToolCallArguments) as string;
        argsJson.ShouldNotBeNull();
        argsJson.ShouldContain("NYC");
    }


    [Fact]
    public void CreateEvent_ReturnsOtelEvent()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var otelEvent = trace.CreateEvent("test-event");

        otelEvent.ShouldNotBeNull();
        otelEvent.ShouldBeOfType<OtelEvent>();
    }

    [Fact]
    public void CreateEvent_SetsEventObservationType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var otelEvent = trace.CreateEvent("test-event");

        var eventActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-event");
        eventActivity.ShouldNotBeNull();
        eventActivity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeEvent);
    }

    [Fact]
    public void CreateEvent_WithInput_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        var exception = Record.Exception(() =>
        {
            using var otelEvent = trace.CreateEvent("test-event", new { data = "test input" });
        });
        exception.ShouldBeNull();
    }

    [Fact]
    public void CreateEvent_WithOutput_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        var exception = Record.Exception(() =>
        {
            using var otelEvent = trace.CreateEvent("test-event", output: new { result = "test output" });
        });
        exception.ShouldBeNull();
    }


    [Fact]
    public void CreateEmbedding_ReturnsOtelEmbedding()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", "test-provider");

        embedding.ShouldNotBeNull();
        embedding.ShouldBeOfType<OtelEmbedding>();
    }

    [Fact]
    public void CreateEmbedding_SetsModelAndObservationType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", "openai");

        var embedActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-embedding");
        embedActivity.ShouldNotBeNull();
        embedActivity.GetTagItem(GenAiAttributes.RequestModel).ShouldBe("text-embedding-ada");
        embedActivity.GetTagItem(GenAiAttributes.ProviderName).ShouldBe("openai");
        embedActivity.GetTagItem(LangfuseAttributes.ObservationType)
            .ShouldBe(LangfuseAttributes.ObservationTypeEmbedding);
    }

    [Fact]
    public void CreateEmbedding_WithInput_SetsObservationInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding =
            trace.CreateEmbedding("test-embedding", "text-embedding-ada", "test-provider", "Text to embed");

        var embedActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-embedding");
        embedActivity.ShouldNotBeNull();
        var inputJson = embedActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        inputJson.ShouldNotBeNull();
        inputJson.ShouldContain("Text to embed");
    }


    [Fact]
    public void CreateAgent_ReturnsOtelAgent()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        agent.ShouldNotBeNull();
        agent.ShouldBeOfType<OtelAgent>();
    }

    [Fact]
    public void CreateAgent_SetsAgentIdAndName()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        agentActivity.ShouldNotBeNull();
        agentActivity.GetTagItem(GenAiAttributes.AgentId).ShouldBe("agent-123");
        agentActivity.GetTagItem(GenAiAttributes.AgentName).ShouldBe("test-agent");
    }

    [Fact]
    public void CreateAgent_WithDescription_SetsAgentDescription()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123", "A helpful assistant");

        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        agentActivity.ShouldNotBeNull();
        agentActivity.GetTagItem(GenAiAttributes.AgentDescription).ShouldBe("A helpful assistant");
    }

    [Fact]
    public void CreateAgent_SetsAgentObservationType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        agentActivity.ShouldNotBeNull();
        agentActivity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeAgent);
    }


    [Fact]
    public void CreateSpan_CreatesChildOfTraceActivity()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("child-span");

        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "child-span");
        spanActivity.ShouldNotBeNull();
        (spanActivity.ParentSpanId == trace.TraceActivity?.SpanId).ShouldBeTrue();
    }

    [Fact]
    public void NestedSpans_CreateProperHierarchy()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var outerSpan = trace.CreateSpan("outer-span");
        using var innerSpan = trace.CreateSpan("inner-span");

        var outerActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "outer-span");
        var innerActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "inner-span");

        outerActivity.ShouldNotBeNull();
        innerActivity.ShouldNotBeNull();

        // Inner span should be child of outer span (due to Activity.Current propagation)
        (innerActivity.ParentSpanId == outerActivity.SpanId).ShouldBeTrue();
    }
}