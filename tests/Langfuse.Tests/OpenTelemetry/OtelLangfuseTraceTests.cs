using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class OtelLangfuseTraceTests : IDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly ActivityListener _listener;
    private readonly ConcurrentBag<Activity> _capturedActivities;

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

        Assert.NotNull(trace.TraceActivity);
        Assert.Equal("test-trace", trace.TraceActivity.GetTagItem(LangfuseAttributes.TraceName));
    }

    [Fact]
    public void Constructor_WithUserId_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", userId: "user-123");

        Assert.Equal("user-123", trace.TraceActivity?.GetTagItem(LangfuseAttributes.UserId));
        Assert.Equal("user-123", Baggage.GetBaggage(LangfuseBaggageKeys.UserId));
    }

    [Fact]
    public void Constructor_WithSessionId_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", sessionId: "session-456");

        Assert.Equal("session-456", trace.TraceActivity?.GetTagItem(LangfuseAttributes.SessionId));
        Assert.Equal("session-456", Baggage.GetBaggage(LangfuseBaggageKeys.SessionId));
    }

    [Fact]
    public void Constructor_WithVersion_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", version: "1.0.0");

        Assert.Equal("1.0.0", trace.TraceActivity?.GetTagItem(LangfuseAttributes.Version));
        Assert.Equal("1.0.0", Baggage.GetBaggage(LangfuseBaggageKeys.Version));
    }

    [Fact]
    public void Constructor_WithRelease_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", release: "prod-1");

        Assert.Equal("prod-1", Baggage.GetBaggage(LangfuseBaggageKeys.Release));
    }

    [Fact]
    public void Constructor_WithTags_SetsBaggageAndTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace", tags: ["tag1", "tag2"]);

        var tagsJson = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceTags) as string;
        Assert.NotNull(tagsJson);
        Assert.Contains("tag1", tagsJson);
        Assert.Contains("tag2", tagsJson);

        var baggageTags = Baggage.GetBaggage(LangfuseBaggageKeys.Tags);
        Assert.NotNull(baggageTags);
        Assert.Contains("tag1", baggageTags);
        Assert.Contains("tag2", baggageTags);
    }

    [Fact]
    public void Constructor_WithInput_SetsTraceInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace", input: new { query = "test query" });
        
        var inputJson = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("test query", inputJson);
    }

    [Fact]
    public void Constructor_WithIsRoot_CreatesNewTraceId()
    {
        using var parentActivity = _activitySource.StartActivity("parent");
        Assert.NotNull(parentActivity);

        using var trace = new OtelLangfuseTrace("test-trace", isRoot: true);

        Assert.NotNull(trace.TraceActivity);
        Assert.NotEqual(parentActivity.TraceId, trace.TraceActivity.TraceId);
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

            Assert.NotNull(trace.TraceActivity);
            Assert.Equal("CustomSource", trace.TraceActivity.Source.Name);
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
            userId: "user-123",
            sessionId: "session-456",
            version: "1.0.0",
            release: "prod-1",
            tags: ["tag1"]);

        Assert.NotNull(Baggage.GetBaggage(LangfuseBaggageKeys.UserId));
        
        trace.Dispose();
        
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.UserId));
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.SessionId));
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.Version));
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.Release));
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.Tags));
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

        Assert.Equal("updated-name", trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceName));
    }

    [Fact]
    public void SetTraceName_UpdatesDisplayName()
    {
        using var trace = new OtelLangfuseTrace("initial-name");
        
        trace.SetTraceName("updated-name");

        Assert.Equal("updated-name", trace.TraceActivity?.DisplayName);
    }



    [Fact]
    public void SetInput_SetsTraceAndObservationInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        trace.SetInput(new { query = "test input" });

        var traceInput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        var observationInput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(traceInput);
        Assert.NotNull(observationInput);
        Assert.Contains("test input", traceInput);
        Assert.Equal(traceInput, observationInput);
    }

    [Fact]
    public void SetOutput_SetsTraceAndObservationOutput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        
        trace.SetOutput(new { result = "test output" });
        
        var traceOutput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        var observationOutput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(traceOutput);
        Assert.NotNull(observationOutput);
        Assert.Contains("test output", traceOutput);
        Assert.Equal(traceOutput, observationOutput);
    }

    [Fact]
    public void CreateSpan_ReturnsOtelSpan()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        using var span = trace.CreateSpan("test-span");
        
        Assert.NotNull(span);
        Assert.IsType<OtelSpan>(span);
    }

    [Fact]
    public void CreateSpan_WithType_SetsSpanType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        using var span = trace.CreateSpan("test-span", type: "retrieval");
        
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("retrieval", spanActivity.GetTagItem("span.type"));
    }

    [Fact]
    public void CreateSpan_WithDescription_SetsDescription()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        using var span = trace.CreateSpan("test-span", description: "Test description");

        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("Test description", spanActivity.GetTagItem("span.description"));
    }

    [Fact]
    public void CreateSpan_WithInput_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        var exception = Record.Exception(() =>
        {
            using var span = trace.CreateSpan("test-span", input: new { data = "test" });
        });
        Assert.Null(exception);
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
        
        Assert.True(configureInvoked);
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("custom_value",
            spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key"));
    }

    [Fact]
    public void CreateGeneration_ReturnsOtelGeneration()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        Assert.NotNull(generation);
        Assert.IsType<OtelGeneration>(generation);
    }

    [Fact]
    public void CreateGeneration_SetsModelAndProvider()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", provider: "openai");

        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal("gpt-4", genActivity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal("openai", genActivity.GetTagItem(GenAiAttributes.ProviderName));
    }

    [Fact]
    public void CreateGeneration_WithInput_SetsObservationInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", input: new { prompt = "Hello" });
        
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        var inputJson = genActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("Hello", inputJson);
    }

    [Fact]
    public void CreateGeneration_SetsGenerationObservationType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal(LangfuseAttributes.ObservationTypeGeneration,
            genActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }



    [Fact]
    public void CreateToolCall_ReturnsOtelToolCall()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");
        
        Assert.NotNull(toolCall);
        Assert.IsType<OtelToolCall>(toolCall);
    }

    [Fact]
    public void CreateToolCall_SetsToolName()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");
        
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);
        Assert.Equal("get_weather", toolActivity.GetTagItem(GenAiAttributes.ToolName));
    }

    [Fact]
    public void CreateToolCall_WithDescription_SetsToolDescription()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall =
            trace.CreateToolCall("test-tool-call", "get_weather", toolDescription: "Gets current weather");
        
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);
        Assert.Equal("Gets current weather", toolActivity.GetTagItem(GenAiAttributes.ToolDescription));
    }

    [Fact]
    public void CreateToolCall_WithCustomToolType_SetsToolType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather", toolType: "api");
        
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);
        Assert.Equal("api", toolActivity.GetTagItem(GenAiAttributes.ToolType));
    }

    [Fact]
    public void CreateToolCall_WithInput_SetsToolCallArguments()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall =
            trace.CreateToolCall("test-tool-call", "get_weather", input: new { location = "NYC" });
        
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);
        var argsJson = toolActivity.GetTagItem(GenAiAttributes.ToolCallArguments) as string;
        Assert.NotNull(argsJson);
        Assert.Contains("NYC", argsJson);
    }



    [Fact]
    public void CreateEvent_ReturnsOtelEvent()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var otelEvent = trace.CreateEvent("test-event");

        Assert.NotNull(otelEvent);
        Assert.IsType<OtelEvent>(otelEvent);
    }

    [Fact]
    public void CreateEvent_SetsEventObservationType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var otelEvent = trace.CreateEvent("test-event");
        
        var eventActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-event");
        Assert.NotNull(eventActivity);
        Assert.Equal(LangfuseAttributes.ObservationTypeEvent, eventActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateEvent_WithInput_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        var exception = Record.Exception(() =>
        {
            using var otelEvent = trace.CreateEvent("test-event", input: new { data = "test input" });
        });
        Assert.Null(exception);
    }

    [Fact]
    public void CreateEvent_WithOutput_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");

        var exception = Record.Exception(() =>
        {
            using var otelEvent = trace.CreateEvent("test-event", output: new { result = "test output" });
        });
        Assert.Null(exception);
    }



    [Fact]
    public void CreateEmbedding_ReturnsOtelEmbedding()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        Assert.NotNull(embedding);
        Assert.IsType<OtelEmbedding>(embedding);
    }

    [Fact]
    public void CreateEmbedding_SetsModelAndObservationType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", provider: "openai");
        
        var embedActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-embedding");
        Assert.NotNull(embedActivity);
        Assert.Equal("text-embedding-ada", embedActivity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal("openai", embedActivity.GetTagItem(GenAiAttributes.ProviderName));
        Assert.Equal(LangfuseAttributes.ObservationTypeEmbedding,
            embedActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateEmbedding_WithInput_SetsObservationInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", input: "Text to embed");
        
        var embedActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-embedding");
        Assert.NotNull(embedActivity);
        var inputJson = embedActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("Text to embed", inputJson);
    }



    [Fact]
    public void CreateAgent_ReturnsOtelAgent()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");
        
        Assert.NotNull(agent);
        Assert.IsType<OtelAgent>(agent);
    }

    [Fact]
    public void CreateAgent_SetsAgentIdAndName()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");
        
        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        Assert.NotNull(agentActivity);
        Assert.Equal("agent-123", agentActivity.GetTagItem(GenAiAttributes.AgentId));
        Assert.Equal("test-agent", agentActivity.GetTagItem(GenAiAttributes.AgentName));
    }

    [Fact]
    public void CreateAgent_WithDescription_SetsAgentDescription()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123", description: "A helpful assistant");
        
        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        Assert.NotNull(agentActivity);
        Assert.Equal("A helpful assistant", agentActivity.GetTagItem(GenAiAttributes.AgentDescription));
    }

    [Fact]
    public void CreateAgent_SetsAgentObservationType()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");
        
        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        Assert.NotNull(agentActivity);
        Assert.Equal(LangfuseAttributes.ObservationTypeAgent,
            agentActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }



    [Fact]
    public void CreateSpan_CreatesChildOfTraceActivity()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("child-span");
        
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "child-span");
        Assert.NotNull(spanActivity);
        Assert.Equal(trace.TraceActivity?.SpanId, spanActivity.ParentSpanId);
    }

    [Fact]
    public void NestedSpans_CreateProperHierarchy()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var outerSpan = trace.CreateSpan("outer-span");
        using var innerSpan = trace.CreateSpan("inner-span");
        
        var outerActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "outer-span");
        var innerActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "inner-span");

        Assert.NotNull(outerActivity);
        Assert.NotNull(innerActivity);

        // Inner span should be child of outer span (due to Activity.Current propagation)
        Assert.Equal(outerActivity.SpanId, innerActivity.ParentSpanId);
    }
}
