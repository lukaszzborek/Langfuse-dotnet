using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class OtelLangfuseTraceContextTests : IDisposable
{
    private readonly ActivityListener _listener;
    private readonly ConcurrentBag<Activity> _capturedActivities;

    public OtelLangfuseTraceContextTests()
    {
        _capturedActivities = new ConcurrentBag<Activity>();

        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Langfuse",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => _capturedActivities.Add(activity)
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public void Dispose()
    {
        Baggage.ClearBaggage();
        _listener.Dispose();
    }

    #region Constructor Tests

    [Fact]
    public void DefaultConstructor_DoesNotCreateTrace()
    {
        // Act
        using var context = new OtelLangfuseTraceContext();

        // Assert
        Assert.Null(context.CurrentTrace);
        Assert.False(context.HasActiveTrace);
    }

    [Fact]
    public void ConstructorWithTraceName_CreatesTrace()
    {
        // Act
        using var context = new OtelLangfuseTraceContext("test-trace");

        // Assert
        Assert.NotNull(context.CurrentTrace);
        Assert.True(context.HasActiveTrace);
    }

    [Fact]
    public void ConstructorWithAllParameters_CreatesConfiguredTrace()
    {
        // Act
        using var context = new OtelLangfuseTraceContext(
            "test-trace",
            userId: "user-123",
            sessionId: "session-456",
            version: "1.0.0",
            release: "prod-1",
            tags: ["tag1", "tag2"],
            input: new { query = "test" });

        // Assert
        Assert.NotNull(context.CurrentTrace);
        Assert.NotNull(context.CurrentTrace.TraceActivity);

        var activity = context.CurrentTrace.TraceActivity;
        Assert.Equal("user-123", activity.GetTagItem(LangfuseAttributes.UserId));
        Assert.Equal("session-456", activity.GetTagItem(LangfuseAttributes.SessionId));
        Assert.Equal("1.0.0", activity.GetTagItem(LangfuseAttributes.Version));
    }

    #endregion

    #region StartTrace Tests

    [Fact]
    public void StartTrace_CreatesNewTrace()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act
        var trace = context.StartTrace("test-trace");

        // Assert
        Assert.NotNull(trace);
        Assert.Same(trace, context.CurrentTrace);
        Assert.True(context.HasActiveTrace);
    }

    [Fact]
    public void StartTrace_WithParameters_SetsTraceProperties()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act
        var trace = context.StartTrace(
            "test-trace",
            userId: "user-123",
            sessionId: "session-456",
            tags: ["tag1"]);

        // Assert
        Assert.NotNull(trace.TraceActivity);
        Assert.Equal("user-123", trace.TraceActivity.GetTagItem(LangfuseAttributes.UserId));
        Assert.Equal("session-456", trace.TraceActivity.GetTagItem(LangfuseAttributes.SessionId));
    }

    [Fact]
    public void StartTrace_WhenTraceAlreadyActive_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("first-trace");

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => context.StartTrace("second-trace"));
        Assert.Contains("already active", exception.Message);
    }

    #endregion

    #region CreateDetachedTrace Tests

    [Fact]
    public void CreateDetachedTrace_ReturnsNewTrace()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act
        using var detachedTrace = context.CreateDetachedTrace("detached-trace");

        // Assert
        Assert.NotNull(detachedTrace);
        Assert.NotNull(detachedTrace.TraceActivity);
    }

    [Fact]
    public void CreateDetachedTrace_DoesNotSetCurrentTrace()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act
        using var detachedTrace = context.CreateDetachedTrace("detached-trace");

        // Assert
        Assert.Null(context.CurrentTrace);
        Assert.False(context.HasActiveTrace);
    }

    [Fact]
    public void CreateDetachedTrace_WithExistingTrace_DoesNotInterfere()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        var mainTrace = context.StartTrace("main-trace");

        // Act
        using var detachedTrace = context.CreateDetachedTrace("detached-trace");

        // Assert
        Assert.Same(mainTrace, context.CurrentTrace);
        Assert.NotSame(mainTrace, detachedTrace);
    }

    [Fact]
    public void CreateDetachedTrace_CreatesRootTrace()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        var mainTrace = context.StartTrace("main-trace");

        // Act
        using var detachedTrace = context.CreateDetachedTrace("detached-trace");

        // Assert - detached trace should have different TraceId (it's a root trace)
        Assert.NotEqual(mainTrace.TraceActivity?.TraceId, detachedTrace.TraceActivity?.TraceId);
    }

    #endregion

    #region CreateSpan Tests (via context)

    [Fact]
    public void CreateSpan_WithActiveTrace_ReturnsSpan()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        using var span = context.CreateSpan("test-span");

        // Assert
        Assert.NotNull(span);
        Assert.IsType<OtelSpan>(span);
    }

    [Fact]
    public void CreateSpan_WithoutActiveTrace_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => context.CreateSpan("test-span"));
        Assert.Contains("No active trace", exception.Message);
    }

    [Fact]
    public void CreateSpan_WithAllParameters_InvokesConfigureAction()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");
        var configureInvoked = false;

        // Act
        using var span = context.CreateSpan(
            "test-span",
            type: "retrieval",
            description: "Test description",
            input: new { query = "test" },
            configure: s =>
            {
                configureInvoked = true;
                s.SetOutput(new { result = "output" });
            });

        // Assert
        Assert.True(configureInvoked);
    }

    #endregion

    #region CreateGeneration Tests (via context)

    [Fact]
    public void CreateGeneration_WithActiveTrace_ReturnsGeneration()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        using var generation = context.CreateGeneration("test-generation", "gpt-4");

        // Assert
        Assert.NotNull(generation);
        Assert.IsType<OtelGeneration>(generation);
    }

    [Fact]
    public void CreateGeneration_WithoutActiveTrace_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act & Assert
        var exception =
            Assert.Throws<InvalidOperationException>(() => context.CreateGeneration("test-generation", "gpt-4"));
        Assert.Contains("No active trace", exception.Message);
    }

    [Fact]
    public void CreateGeneration_WithAllParameters_CreatesConfiguredGeneration()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        using var generation = context.CreateGeneration(
            "test-generation",
            "gpt-4",
            provider: "openai",
            input: new { prompt = "Hello" },
            configure: g => { g.SetTemperature(0.7); });

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal("gpt-4", genActivity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal("openai", genActivity.GetTagItem(GenAiAttributes.ProviderName));
        Assert.Equal(0.7, genActivity.GetTagItem(GenAiAttributes.RequestTemperature));
    }

    #endregion

    #region CreateToolCall Tests (via context)

    [Fact]
    public void CreateToolCall_WithActiveTrace_ReturnsToolCall()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        using var toolCall = context.CreateToolCall("test-tool-call", "get_weather");

        // Assert
        Assert.NotNull(toolCall);
        Assert.IsType<OtelToolCall>(toolCall);
    }

    [Fact]
    public void CreateToolCall_WithoutActiveTrace_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act & Assert
        var exception =
            Assert.Throws<InvalidOperationException>(() => context.CreateToolCall("test-tool-call", "get_weather"));
        Assert.Contains("No active trace", exception.Message);
    }

    #endregion

    #region CreateEvent Tests (via context)

    [Fact]
    public void CreateEvent_WithActiveTrace_ReturnsEvent()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        using var otelEvent = context.CreateEvent("test-event");

        // Assert
        Assert.NotNull(otelEvent);
        Assert.IsType<OtelEvent>(otelEvent);
    }

    [Fact]
    public void CreateEvent_WithoutActiveTrace_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => context.CreateEvent("test-event"));
        Assert.Contains("No active trace", exception.Message);
    }

    [Fact]
    public void CreateEvent_WithInputAndOutput_SetsObservationData()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        using var otelEvent = context.CreateEvent(
            "test-event",
            input: new { data = "input" },
            output: new { result = "output" });

        // Assert
        var eventActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-event");
        Assert.NotNull(eventActivity);

        var inputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        var outputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(inputJson);
        Assert.NotNull(outputJson);
        Assert.Contains("input", inputJson);
        Assert.Contains("output", outputJson);
    }

    #endregion

    #region CreateEmbedding Tests (via context)

    [Fact]
    public void CreateEmbedding_WithActiveTrace_ReturnsEmbedding()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        using var embedding = context.CreateEmbedding("test-embedding", "text-embedding-ada");

        // Assert
        Assert.NotNull(embedding);
        Assert.IsType<OtelEmbedding>(embedding);
    }

    [Fact]
    public void CreateEmbedding_WithoutActiveTrace_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act & Assert
        var exception =
            Assert.Throws<InvalidOperationException>(() => context.CreateEmbedding("test-embedding", "text-ada"));
        Assert.Contains("No active trace", exception.Message);
    }

    #endregion

    #region CreateAgent Tests (via context)

    [Fact]
    public void CreateAgent_WithActiveTrace_ReturnsAgent()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        using var agent = context.CreateAgent("test-agent", "agent-123");

        // Assert
        Assert.NotNull(agent);
        Assert.IsType<OtelAgent>(agent);
    }

    [Fact]
    public void CreateAgent_WithoutActiveTrace_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => context.CreateAgent("test-agent", "agent-123"));
        Assert.Contains("No active trace", exception.Message);
    }

    #endregion

    #region SetInput/SetOutput Tests (via context)

    [Fact]
    public void SetInput_WithActiveTrace_SetsTraceInput()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        context.SetInput(new { query = "test input" });

        // Assert
        var inputJson = context.CurrentTrace?.TraceActivity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("test input", inputJson);
    }

    [Fact]
    public void SetInput_WithoutActiveTrace_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => context.SetInput(new { query = "test" }));
        Assert.Contains("No active trace", exception.Message);
    }

    [Fact]
    public void SetOutput_WithActiveTrace_SetsTraceOutput()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        context.SetOutput(new { result = "test output" });

        // Assert
        var outputJson = context.CurrentTrace?.TraceActivity?.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        Assert.NotNull(outputJson);
        Assert.Contains("test output", outputJson);
    }

    [Fact]
    public void SetOutput_WithoutActiveTrace_ThrowsInvalidOperationException()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => context.SetOutput(new { result = "test" }));
        Assert.Contains("No active trace", exception.Message);
    }

    #endregion

    #region Dispose Tests

    [Fact]
    public void Dispose_CleansUpTrace()
    {
        // Arrange
        var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        context.Dispose();

        // Assert
        Assert.Null(context.CurrentTrace);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act & Assert - should not throw
        context.Dispose();
        context.Dispose();
        context.Dispose();
    }

    [Fact]
    public void Dispose_WithoutTrace_DoesNotThrow()
    {
        // Arrange
        var context = new OtelLangfuseTraceContext();

        // Act & Assert - should not throw
        context.Dispose();
    }

    #endregion

    #region HasActiveTrace Tests

    [Fact]
    public void HasActiveTrace_WhenNoTrace_ReturnsFalse()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Assert
        Assert.False(context.HasActiveTrace);
    }

    [Fact]
    public void HasActiveTrace_AfterStartTrace_ReturnsTrue()
    {
        // Arrange
        using var context = new OtelLangfuseTraceContext();

        // Act
        context.StartTrace("test-trace");

        // Assert
        Assert.True(context.HasActiveTrace);
    }

    [Fact]
    public void HasActiveTrace_AfterDispose_ReturnsFalse()
    {
        // Arrange
        var context = new OtelLangfuseTraceContext();
        context.StartTrace("test-trace");

        // Act
        context.Dispose();

        // Assert
        Assert.False(context.HasActiveTrace);
    }

    #endregion
}
