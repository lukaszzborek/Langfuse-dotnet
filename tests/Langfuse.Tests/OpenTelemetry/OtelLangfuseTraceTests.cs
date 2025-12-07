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

    #region Constructor Tests

    [Fact]
    public void Constructor_WithTraceName_CreatesTraceActivity()
    {
        // Act
        using var trace = new OtelLangfuseTrace("test-trace");

        // Assert
        Assert.NotNull(trace.TraceActivity);
        Assert.Equal("test-trace", trace.TraceActivity.GetTagItem(LangfuseAttributes.TraceName));
    }

    [Fact]
    public void Constructor_WithUserId_SetsBaggageAndTag()
    {
        // Act
        using var trace = new OtelLangfuseTrace("test-trace", userId: "user-123");

        // Assert
        Assert.Equal("user-123", trace.TraceActivity?.GetTagItem(LangfuseAttributes.UserId));
        Assert.Equal("user-123", Baggage.GetBaggage(LangfuseBaggageKeys.UserId));
    }

    [Fact]
    public void Constructor_WithSessionId_SetsBaggageAndTag()
    {
        // Act
        using var trace = new OtelLangfuseTrace("test-trace", sessionId: "session-456");

        // Assert
        Assert.Equal("session-456", trace.TraceActivity?.GetTagItem(LangfuseAttributes.SessionId));
        Assert.Equal("session-456", Baggage.GetBaggage(LangfuseBaggageKeys.SessionId));
    }

    [Fact]
    public void Constructor_WithVersion_SetsBaggageAndTag()
    {
        // Act
        using var trace = new OtelLangfuseTrace("test-trace", version: "1.0.0");

        // Assert
        Assert.Equal("1.0.0", trace.TraceActivity?.GetTagItem(LangfuseAttributes.Version));
        Assert.Equal("1.0.0", Baggage.GetBaggage(LangfuseBaggageKeys.Version));
    }

    [Fact]
    public void Constructor_WithRelease_SetsBaggageAndTag()
    {
        // Act
        using var trace = new OtelLangfuseTrace("test-trace", release: "prod-1");

        // Assert
        Assert.Equal("prod-1", Baggage.GetBaggage(LangfuseBaggageKeys.Release));
    }

    [Fact]
    public void Constructor_WithTags_SetsBaggageAndTag()
    {
        // Act
        using var trace = new OtelLangfuseTrace("test-trace", tags: ["tag1", "tag2"]);

        // Assert
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
        // Act
        using var trace = new OtelLangfuseTrace("test-trace", input: new { query = "test query" });

        // Assert
        var inputJson = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("test query", inputJson);
    }

    [Fact]
    public void Constructor_WithIsRoot_CreatesNewTraceId()
    {
        // Create a parent activity first
        using var parentActivity = _activitySource.StartActivity("parent");
        Assert.NotNull(parentActivity);

        // Act - create trace with isRoot=true
        using var trace = new OtelLangfuseTrace("test-trace", isRoot: true);

        // Assert - trace should have different TraceId than parent
        Assert.NotNull(trace.TraceActivity);
        Assert.NotEqual(parentActivity.TraceId, trace.TraceActivity.TraceId);
    }

    [Fact]
    public void Constructor_WithCustomActivitySource_UsesProvidedSource()
    {
        // Arrange
        var customSource = new ActivitySource("CustomSource");
        var customListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "CustomSource",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(customListener);

        try
        {
            // Act
            using var trace = new OtelLangfuseTrace(customSource, "test-trace");

            // Assert
            Assert.NotNull(trace.TraceActivity);
            Assert.Equal("CustomSource", trace.TraceActivity.Source.Name);
        }
        finally
        {
            customListener.Dispose();
            customSource.Dispose();
        }
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_ClearsBaggageContext()
    {
        // Arrange
        var trace = new OtelLangfuseTrace(
            "test-trace",
            userId: "user-123",
            sessionId: "session-456",
            version: "1.0.0",
            release: "prod-1",
            tags: ["tag1"]);

        // Verify Baggage is set
        Assert.NotNull(Baggage.GetBaggage(LangfuseBaggageKeys.UserId));

        // Act
        trace.Dispose();

        // Assert
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.UserId));
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.SessionId));
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.Version));
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.Release));
        Assert.Null(Baggage.GetBaggage(LangfuseBaggageKeys.Tags));
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var trace = new OtelLangfuseTrace("test-trace");

        // Act & Assert - should not throw
        trace.Dispose();
        trace.Dispose();
        trace.Dispose();
    }

    #endregion

    #region SetTraceName Tests

    [Fact]
    public void SetTraceName_UpdatesTraceNameTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("initial-name");

        // Act
        trace.SetTraceName("updated-name");

        // Assert
        Assert.Equal("updated-name", trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceName));
    }

    [Fact]
    public void SetTraceName_UpdatesDisplayName()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("initial-name");

        // Act
        trace.SetTraceName("updated-name");

        // Assert
        Assert.Equal("updated-name", trace.TraceActivity?.DisplayName);
    }

    #endregion

    #region SetInput/SetOutput Tests

    [Fact]
    public void SetInput_SetsTraceAndObservationInput()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        trace.SetInput(new { query = "test input" });

        // Assert
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
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        trace.SetOutput(new { result = "test output" });

        // Assert
        var traceOutput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        var observationOutput = trace.TraceActivity?.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(traceOutput);
        Assert.NotNull(observationOutput);
        Assert.Contains("test output", traceOutput);
        Assert.Equal(traceOutput, observationOutput);
    }

    #endregion

    #region CreateSpan Tests

    [Fact]
    public void CreateSpan_ReturnsOtelSpan()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var span = trace.CreateSpan("test-span");

        // Assert
        Assert.NotNull(span);
        Assert.IsType<OtelSpan>(span);
    }

    [Fact]
    public void CreateSpan_WithType_SetsSpanType()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var span = trace.CreateSpan("test-span", type: "retrieval");

        // Assert - verify through captured activities
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("retrieval", spanActivity.GetTagItem("span.type"));
    }

    [Fact]
    public void CreateSpan_WithDescription_SetsDescription()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var span = trace.CreateSpan("test-span", description: "Test description");

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("Test description", spanActivity.GetTagItem("span.description"));
    }

    [Fact]
    public void CreateSpan_WithInput_DoesNotThrow()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act & Assert - should not throw
        var exception = Record.Exception(() =>
        {
            using var span = trace.CreateSpan("test-span", input: new { data = "test" });
        });
        Assert.Null(exception);
    }

    [Fact]
    public void CreateSpan_WithConfigureAction_InvokesAction()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        var configureInvoked = false;

        // Act
        using var span = trace.CreateSpan("test-span", configure: s =>
        {
            configureInvoked = true;
            s.SetMetadata("custom_key", "custom_value");
        });

        // Assert
        Assert.True(configureInvoked);
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("custom_value",
            spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key"));
    }

    #endregion

    #region CreateGeneration Tests

    [Fact]
    public void CreateGeneration_ReturnsOtelGeneration()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Assert
        Assert.NotNull(generation);
        Assert.IsType<OtelGeneration>(generation);
    }

    [Fact]
    public void CreateGeneration_SetsModelAndProvider()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", provider: "openai");

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal("gpt-4", genActivity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal("openai", genActivity.GetTagItem(GenAiAttributes.ProviderName));
    }

    [Fact]
    public void CreateGeneration_WithInput_SetsObservationInput()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", input: new { prompt = "Hello" });

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        var inputJson = genActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("Hello", inputJson);
    }

    [Fact]
    public void CreateGeneration_SetsGenerationObservationType()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal(LangfuseAttributes.ObservationTypeGeneration,
            genActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    #endregion

    #region CreateToolCall Tests

    [Fact]
    public void CreateToolCall_ReturnsOtelToolCall()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        // Assert
        Assert.NotNull(toolCall);
        Assert.IsType<OtelToolCall>(toolCall);
    }

    [Fact]
    public void CreateToolCall_SetsToolName()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        // Assert
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);
        Assert.Equal("get_weather", toolActivity.GetTagItem(GenAiAttributes.ToolName));
    }

    [Fact]
    public void CreateToolCall_WithDescription_SetsToolDescription()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var toolCall =
            trace.CreateToolCall("test-tool-call", "get_weather", toolDescription: "Gets current weather");

        // Assert
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);
        Assert.Equal("Gets current weather", toolActivity.GetTagItem(GenAiAttributes.ToolDescription));
    }

    [Fact]
    public void CreateToolCall_WithCustomToolType_SetsToolType()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather", toolType: "api");

        // Assert
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);
        Assert.Equal("api", toolActivity.GetTagItem(GenAiAttributes.ToolType));
    }

    [Fact]
    public void CreateToolCall_WithInput_SetsToolCallArguments()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var toolCall =
            trace.CreateToolCall("test-tool-call", "get_weather", input: new { location = "NYC" });

        // Assert
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);
        var argsJson = toolActivity.GetTagItem(GenAiAttributes.ToolCallArguments) as string;
        Assert.NotNull(argsJson);
        Assert.Contains("NYC", argsJson);
    }

    #endregion

    #region CreateEvent Tests

    [Fact]
    public void CreateEvent_ReturnsOtelEvent()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var otelEvent = trace.CreateEvent("test-event");

        // Assert
        Assert.NotNull(otelEvent);
        Assert.IsType<OtelEvent>(otelEvent);
    }

    [Fact]
    public void CreateEvent_SetsEventObservationType()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var otelEvent = trace.CreateEvent("test-event");

        // Assert
        var eventActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-event");
        Assert.NotNull(eventActivity);
        Assert.Equal(LangfuseAttributes.ObservationTypeEvent, eventActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateEvent_WithInput_DoesNotThrow()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act & Assert - should not throw
        var exception = Record.Exception(() =>
        {
            using var otelEvent = trace.CreateEvent("test-event", input: new { data = "test input" });
        });
        Assert.Null(exception);
    }

    [Fact]
    public void CreateEvent_WithOutput_DoesNotThrow()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act & Assert - should not throw
        var exception = Record.Exception(() =>
        {
            using var otelEvent = trace.CreateEvent("test-event", output: new { result = "test output" });
        });
        Assert.Null(exception);
    }

    #endregion

    #region CreateEmbedding Tests

    [Fact]
    public void CreateEmbedding_ReturnsOtelEmbedding()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        // Assert
        Assert.NotNull(embedding);
        Assert.IsType<OtelEmbedding>(embedding);
    }

    [Fact]
    public void CreateEmbedding_SetsModelAndObservationType()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", provider: "openai");

        // Assert
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
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", input: "Text to embed");

        // Assert
        var embedActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-embedding");
        Assert.NotNull(embedActivity);
        var inputJson = embedActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("Text to embed", inputJson);
    }

    #endregion

    #region CreateAgent Tests

    [Fact]
    public void CreateAgent_ReturnsOtelAgent()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        // Assert
        Assert.NotNull(agent);
        Assert.IsType<OtelAgent>(agent);
    }

    [Fact]
    public void CreateAgent_SetsAgentIdAndName()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        // Assert
        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        Assert.NotNull(agentActivity);
        Assert.Equal("agent-123", agentActivity.GetTagItem(GenAiAttributes.AgentId));
        Assert.Equal("test-agent", agentActivity.GetTagItem(GenAiAttributes.AgentName));
    }

    [Fact]
    public void CreateAgent_WithDescription_SetsAgentDescription()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var agent = trace.CreateAgent("test-agent", "agent-123", description: "A helpful assistant");

        // Assert
        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        Assert.NotNull(agentActivity);
        Assert.Equal("A helpful assistant", agentActivity.GetTagItem(GenAiAttributes.AgentDescription));
    }

    [Fact]
    public void CreateAgent_SetsAgentObservationType()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        // Assert
        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        Assert.NotNull(agentActivity);
        Assert.Equal(LangfuseAttributes.ObservationTypeAgent,
            agentActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    #endregion

    #region Parent-Child Relationship Tests

    [Fact]
    public void CreateSpan_CreatesChildOfTraceActivity()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var span = trace.CreateSpan("child-span");

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "child-span");
        Assert.NotNull(spanActivity);
        Assert.Equal(trace.TraceActivity?.SpanId, spanActivity.ParentSpanId);
    }

    [Fact]
    public void NestedSpans_CreateProperHierarchy()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");

        // Act
        using var outerSpan = trace.CreateSpan("outer-span");
        using var innerSpan = trace.CreateSpan("inner-span");

        // Assert
        var outerActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "outer-span");
        var innerActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "inner-span");

        Assert.NotNull(outerActivity);
        Assert.NotNull(innerActivity);

        // Inner span should be child of outer span (due to Activity.Current propagation)
        Assert.Equal(outerActivity.SpanId, innerActivity.ParentSpanId);
    }

    #endregion
}
