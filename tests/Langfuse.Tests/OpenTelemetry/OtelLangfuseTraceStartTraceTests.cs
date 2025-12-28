using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using Shouldly;
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

        trace.TraceActivity.ShouldBeNull();
        trace.HasActiveTrace.ShouldBeFalse();
    }

    [Fact]
    public void StartTrace_CreatesNewTrace()
    {
        using var trace = new OtelLangfuseTrace();

        var result = trace.StartTrace("test-trace");

        trace.TraceActivity.ShouldNotBeNull();
        result.ShouldBeSameAs(trace);
        trace.HasActiveTrace.ShouldBeTrue();
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

        trace.TraceActivity.ShouldNotBeNull();
        trace.TraceActivity.GetTagItem(LangfuseAttributes.UserId).ShouldBe("user-123");
        trace.TraceActivity.GetTagItem(LangfuseAttributes.SessionId).ShouldBe("session-456");
        trace.TraceActivity.GetTagItem(LangfuseAttributes.Version).ShouldBe("1.0.0");
    }

    [Fact]
    public void StartTrace_WhenTraceAlreadyActive_ThrowsInvalidOperationException()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("first-trace");

        var exception = Should.Throw<InvalidOperationException>(() => trace.StartTrace("second-trace"));
        exception.Message.ShouldContain("already active");
    }


    [Fact]
    public void CreateDetachedTrace_ReturnsNewTrace()
    {
        using var detachedTrace = OtelLangfuseTrace.CreateDetachedTrace("detached-trace");

        detachedTrace.ShouldNotBeNull();
        detachedTrace.TraceActivity.ShouldNotBeNull();
        detachedTrace.HasActiveTrace.ShouldBeTrue();
    }

    [Fact]
    public void CreateDetachedTrace_CreatesRootTrace()
    {
        using var mainTrace = new OtelLangfuseTrace();
        mainTrace.StartTrace("main-trace");

        using var detachedTrace = OtelLangfuseTrace.CreateDetachedTrace("detached-trace");

        (detachedTrace.TraceActivity?.TraceId == mainTrace.TraceActivity?.TraceId).ShouldBeFalse();
    }

    [Fact]
    public void CreateSpan_WithActiveTrace_ReturnsSpan()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var span = trace.CreateSpan("test-span");

        span.ShouldNotBeNull();
        span.ShouldBeOfType<OtelSpan>();
    }

    [Fact]
    public void CreateSpan_WithoutActiveTrace_ReturnsNoOpSpan()
    {
        using var trace = new OtelLangfuseTrace();

        using var span = trace.CreateSpan("test-span");

        span.ShouldNotBeNull();
        span.ShouldBeOfType<OtelSpan>();
        // No-op span has null activity
        span.Activity.ShouldBeNull();
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

        configureInvoked.ShouldBeTrue();
    }

    [Fact]
    public void CreateGeneration_WithActiveTrace_ReturnsGeneration()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.ShouldNotBeNull();
        generation.ShouldBeOfType<OtelGeneration>();
    }

    [Fact]
    public void CreateGeneration_WithoutActiveTrace_ReturnsNoOpGeneration()
    {
        using var trace = new OtelLangfuseTrace();

        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.ShouldNotBeNull();
        generation.ShouldBeOfType<OtelGeneration>();
        generation.Activity.ShouldBeNull();
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
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(GenAiAttributes.RequestModel).ShouldBe("gpt-4");
        genActivity.GetTagItem(GenAiAttributes.ProviderName).ShouldBe("openai");
        genActivity.GetTagItem(GenAiAttributes.RequestTemperature).ShouldBe(0.7);
    }

    [Fact]
    public void CreateToolCall_WithActiveTrace_ReturnsToolCall()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        toolCall.ShouldNotBeNull();
        toolCall.ShouldBeOfType<OtelToolCall>();
    }

    [Fact]
    public void CreateToolCall_WithoutActiveTrace_ReturnsNoOpToolCall()
    {
        using var trace = new OtelLangfuseTrace();

        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        toolCall.ShouldNotBeNull();
        toolCall.ShouldBeOfType<OtelToolCall>();
        toolCall.Activity.ShouldBeNull();
    }

    [Fact]
    public void CreateEvent_WithActiveTrace_ReturnsEvent()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var otelEvent = trace.CreateEvent("test-event");

        otelEvent.ShouldNotBeNull();
        otelEvent.ShouldBeOfType<OtelEvent>();
    }

    [Fact]
    public void CreateEvent_WithoutActiveTrace_ReturnsNoOpEvent()
    {
        using var trace = new OtelLangfuseTrace();

        using var otelEvent = trace.CreateEvent("test-event");

        otelEvent.ShouldNotBeNull();
        otelEvent.ShouldBeOfType<OtelEvent>();
        otelEvent.Activity.ShouldBeNull();
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
        eventActivity.ShouldNotBeNull();

        var inputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        var outputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        inputJson.ShouldNotBeNull();
        outputJson.ShouldNotBeNull();
        inputJson.ShouldContain("input");
        outputJson.ShouldContain("output");
    }

    [Fact]
    public void CreateEmbedding_WithActiveTrace_ReturnsEmbedding()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        embedding.ShouldNotBeNull();
        embedding.ShouldBeOfType<OtelEmbedding>();
    }

    [Fact]
    public void CreateEmbedding_WithoutActiveTrace_ReturnsNoOpEmbedding()
    {
        using var trace = new OtelLangfuseTrace();

        using var embedding = trace.CreateEmbedding("test-embedding", "text-ada");

        embedding.ShouldNotBeNull();
        embedding.ShouldBeOfType<OtelEmbedding>();
        embedding.Activity.ShouldBeNull();
    }

    [Fact]
    public void CreateAgent_WithActiveTrace_ReturnsAgent()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        using var agent = trace.CreateAgent("test-agent", "agent-123");

        agent.ShouldNotBeNull();
        agent.ShouldBeOfType<OtelAgent>();
    }

    [Fact]
    public void CreateAgent_WithoutActiveTrace_ReturnsNoOpAgent()
    {
        using var trace = new OtelLangfuseTrace();

        using var agent = trace.CreateAgent("test-agent", "agent-123");

        agent.ShouldNotBeNull();
        agent.ShouldBeOfType<OtelAgent>();
        agent.Activity.ShouldBeNull();
    }


    [Fact]
    public void SetInput_WithActiveTrace_SetsTraceInput()
    {
        using var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        trace.SetInput(new { query = "test input" });

        var inputJson = trace.TraceActivity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        inputJson.ShouldNotBeNull();
        inputJson.ShouldContain("test input");
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
        outputJson.ShouldNotBeNull();
        outputJson.ShouldContain("test output");
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

        trace.TraceActivity.ShouldBeNull();
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

        trace.HasActiveTrace.ShouldBeFalse();
    }

    [Fact]
    public void HasActiveTrace_AfterStartTrace_ReturnsTrue()
    {
        using var trace = new OtelLangfuseTrace();

        trace.StartTrace("test-trace");

        trace.HasActiveTrace.ShouldBeTrue();
    }

    [Fact]
    public void HasActiveTrace_AfterDispose_ReturnsFalse()
    {
        var trace = new OtelLangfuseTrace();
        trace.StartTrace("test-trace");

        trace.Dispose();

        trace.HasActiveTrace.ShouldBeFalse();
    }
}