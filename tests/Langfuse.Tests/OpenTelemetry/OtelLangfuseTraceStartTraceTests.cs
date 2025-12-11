using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class OtelLangfuseTraceStartTraceTests : IDisposable
{
    private readonly ConcurrentBag<Activity> _capturedActivities;
    private readonly ActivityListener _listener;

    public OtelLangfuseTraceStartTraceTests()
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


    [Fact]
    public void DefaultConstructor_DoesNotCreateTrace()
    {
        using var trace = new OtelLangfuseTrace();

        Assert.Null(trace.TraceActivity);
        Assert.False(trace.HasActiveTrace);
    }

    [Fact]
    public void StartTrace_CreatesNewTrace()
    {
        using var trace = new OtelLangfuseTrace();

        var result = trace.StartTrace("test-trace");

        Assert.NotNull(trace.TraceActivity);
        Assert.Same(trace, result);
        Assert.True(trace.HasActiveTrace);
    }

    [Fact]
    public void StartTrace_WithParameters_SetsTraceProperties()
    {
        using var trace = new OtelLangfuseTrace();

        trace.StartTrace(
            "test-trace",
            "user-123",
            "session-456",
            "1.0.0",
            "prod-1",
            ["tag1", "tag2"],
            new { query = "test" });

        Assert.NotNull(trace.TraceActivity);
        Assert.Equal("user-123", trace.TraceActivity.GetTagItem(LangfuseAttributes.UserId));
        Assert.Equal("session-456", trace.TraceActivity.GetTagItem(LangfuseAttributes.SessionId));
        Assert.Equal("1.0.0", trace.TraceActivity.GetTagItem(LangfuseAttributes.Version));
    }

    [Fact]
    public void StartTrace_WhenTraceAlreadyActive_ThrowsInvalidOperationException()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("first-trace");

        var exception = Assert.Throws<InvalidOperationException>(() => trace.StartTrace("second-trace"));
        Assert.Contains("already active", exception.Message);
    }


    [Fact]
    public void CreateDetachedTrace_ReturnsNewTrace()
    {
        using var detachedTrace = OtelLangfuseTrace.CreateDetachedTrace("detached-trace");

        Assert.NotNull(detachedTrace);
        Assert.NotNull(detachedTrace.TraceActivity);
        Assert.True(detachedTrace.HasActiveTrace);
    }

    [Fact]
    public void CreateDetachedTrace_CreatesRootTrace()
    {
        using var mainTrace = new OtelLangfuseTrace();
        mainTrace.StartTrace("main-trace");

        using var detachedTrace = OtelLangfuseTrace.CreateDetachedTrace("detached-trace");

        Assert.NotEqual(mainTrace.TraceActivity?.TraceId, detachedTrace.TraceActivity?.TraceId);
    }

    [Fact]
    public void CreateSpan_WithActiveTrace_ReturnsSpan()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var span = trace.CreateSpan("test-span");

        Assert.NotNull(span);
        Assert.IsType<OtelSpan>(span);
    }

    [Fact]
    public void CreateSpan_WithoutActiveTrace_ReturnsNoOpSpan()
    {
        using var trace = new OtelLangfuseTrace();

        using var span = trace.CreateSpan("test-span");

        Assert.NotNull(span);
        Assert.IsType<OtelSpan>(span);
        // No-op span has null activity
        Assert.Null(span.Activity);
    }

    [Fact]
    public void CreateSpan_WithAllParameters_InvokesConfigureAction()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");
        var configureInvoked = false;

        using var span = trace.CreateSpan(
            "test-span",
            "retrieval",
            "Test description",
            new { query = "test" },
            s =>
            {
                configureInvoked = true;
                s.SetOutput(new { result = "output" });
            });

        Assert.True(configureInvoked);
    }

    [Fact]
    public void CreateGeneration_WithActiveTrace_ReturnsGeneration()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        Assert.NotNull(generation);
        Assert.IsType<OtelGeneration>(generation);
    }

    [Fact]
    public void CreateGeneration_WithoutActiveTrace_ReturnsNoOpGeneration()
    {
        using var trace = new OtelLangfuseTrace();

        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        Assert.NotNull(generation);
        Assert.IsType<OtelGeneration>(generation);
        Assert.Null(generation.Activity);
    }

    [Fact]
    public void CreateGeneration_WithAllParameters_CreatesConfiguredGeneration()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var generation = trace.CreateGeneration(
            "test-generation",
            "gpt-4",
            "openai",
            new { prompt = "Hello" },
            g => { g.SetTemperature(0.7); });

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.Equal("gpt-4", genActivity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal("openai", genActivity.GetTagItem(GenAiAttributes.ProviderName));
        Assert.Equal(0.7, genActivity.GetTagItem(GenAiAttributes.RequestTemperature));
    }

    [Fact]
    public void CreateToolCall_WithActiveTrace_ReturnsToolCall()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        Assert.NotNull(toolCall);
        Assert.IsType<OtelToolCall>(toolCall);
    }

    [Fact]
    public void CreateToolCall_WithoutActiveTrace_ReturnsNoOpToolCall()
    {
        using var trace = new OtelLangfuseTrace();

        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        Assert.NotNull(toolCall);
        Assert.IsType<OtelToolCall>(toolCall);
        Assert.Null(toolCall.Activity);
    }

    [Fact]
    public void CreateEvent_WithActiveTrace_ReturnsEvent()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var otelEvent = trace.CreateEvent("test-event");

        Assert.NotNull(otelEvent);
        Assert.IsType<OtelEvent>(otelEvent);
    }

    [Fact]
    public void CreateEvent_WithoutActiveTrace_ReturnsNoOpEvent()
    {
        using var trace = new OtelLangfuseTrace();

        using var otelEvent = trace.CreateEvent("test-event");

        Assert.NotNull(otelEvent);
        Assert.IsType<OtelEvent>(otelEvent);
        Assert.Null(otelEvent.Activity);
    }

    [Fact]
    public void CreateEvent_WithInputAndOutput_SetsObservationData()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var otelEvent = trace.CreateEvent(
            "test-event",
            new { data = "input" },
            new { result = "output" });

        var eventActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-event");
        Assert.NotNull(eventActivity);

        var inputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        var outputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(inputJson);
        Assert.NotNull(outputJson);
        Assert.Contains("input", inputJson);
        Assert.Contains("output", outputJson);
    }

    [Fact]
    public void CreateEmbedding_WithActiveTrace_ReturnsEmbedding()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        Assert.NotNull(embedding);
        Assert.IsType<OtelEmbedding>(embedding);
    }

    [Fact]
    public void CreateEmbedding_WithoutActiveTrace_ReturnsNoOpEmbedding()
    {
        using var trace = new OtelLangfuseTrace();

        using var embedding = trace.CreateEmbedding("test-embedding", "text-ada");

        Assert.NotNull(embedding);
        Assert.IsType<OtelEmbedding>(embedding);
        Assert.Null(embedding.Activity);
    }

    [Fact]
    public void CreateAgent_WithActiveTrace_ReturnsAgent()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var agent = trace.CreateAgent("test-agent", "agent-123");

        Assert.NotNull(agent);
        Assert.IsType<OtelAgent>(agent);
    }

    [Fact]
    public void CreateAgent_WithoutActiveTrace_ReturnsNoOpAgent()
    {
        using var trace = new OtelLangfuseTrace();

        using var agent = trace.CreateAgent("test-agent", "agent-123");

        Assert.NotNull(agent);
        Assert.IsType<OtelAgent>(agent);
        Assert.Null(agent.Activity);
    }


    [Fact]
    public void SetInput_WithActiveTrace_SetsTraceInput()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        trace.SetInput(new { query = "test input" });

        var inputJson = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("test input", inputJson);
    }

    [Fact]
    public void SetInput_WithoutActiveTrace_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace();

        // Should not throw, just no-op
        trace.SetInput(new { query = "test" });
    }

    [Fact]
    public void SetOutput_WithActiveTrace_SetsTraceOutput()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        trace.SetOutput(new { result = "test output" });

        var outputJson = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        Assert.NotNull(outputJson);
        Assert.Contains("test output", outputJson);
    }

    [Fact]
    public void SetOutput_WithoutActiveTrace_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace();

        // Should not throw, just no-op
        trace.SetOutput(new { result = "test" });
    }

    [Fact]
    public void Dispose_CleansUpTrace()
    {
        var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        trace.Dispose();

        Assert.Null(trace.TraceActivity);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        trace.Dispose();
        trace.Dispose();
        trace.Dispose();
    }

    [Fact]
    public void Dispose_WithoutTrace_DoesNotThrow()
    {
        var trace = new OtelLangfuseTrace();

        trace.Dispose();
    }

    [Fact]
    public void HasActiveTrace_WhenNoTrace_ReturnsFalse()
    {
        using var trace = new OtelLangfuseTrace();

        Assert.False(trace.HasActiveTrace);
    }

    [Fact]
    public void HasActiveTrace_AfterStartTrace_ReturnsTrue()
    {
        using var trace = new OtelLangfuseTrace();

        trace.StartTrace("test-trace");

        Assert.True(trace.HasActiveTrace);
    }

    [Fact]
    public void HasActiveTrace_AfterDispose_ReturnsFalse()
    {
        var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        trace.Dispose();

        Assert.False(trace.HasActiveTrace);
    }
}