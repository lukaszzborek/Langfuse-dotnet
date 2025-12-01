using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     OpenTelemetry-based Langfuse trace that manages activities and parent-child relationships.
///     Similar to LangfuseTrace but uses OpenTelemetry activities instead of direct ingestion.
/// </summary>
public class OtelLangfuseTrace : IDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly Stack<Activity> _activityStack = new();
    private readonly Activity? _rootActivity;
    private bool _disposed;

    /// <summary>
    ///     The root trace activity.
    /// </summary>
    public Activity? TraceActivity => _rootActivity;

    /// <summary>
    ///     The current active activity (top of the stack).
    /// </summary>
    public Activity? CurrentActivity => _activityStack.Count > 0 ? _activityStack.Peek() : _rootActivity;

    /// <summary>
    ///     Input collected from child observations (propagated to trace).
    /// </summary>
    public object? CollectedInput { get; private set; }

    /// <summary>
    ///     Output collected from child observations (propagated to trace).
    /// </summary>
    public object? CollectedOutput { get; private set; }

    /// <summary>
    ///     Creates a new OpenTelemetry-based Langfuse trace.
    /// </summary>
    /// <param name="activitySource">The activity source to use for creating activities.</param>
    /// <param name="traceName">The name of the trace.</param>
    /// <param name="config">Optional trace configuration.</param>
    public OtelLangfuseTrace(ActivitySource activitySource, string traceName, TraceConfig? config = null)
    {
        _activitySource = activitySource;
        config ??= new TraceConfig();

        _rootActivity = GenAiActivityHelper.CreateTraceActivity(activitySource, traceName, config);

        if (_rootActivity != null)
        {
            _activityStack.Push(_rootActivity);
        }
    }

    /// <summary>
    ///     Sets the trace name.
    /// </summary>
    public void SetTraceName(string name)
    {
        _rootActivity?.SetTag("langfuse.trace.name", name);
    }

    /// <summary>
    ///     Sets trace-level input.
    /// </summary>
    public void SetInput(object input)
    {
        GenAiActivityHelper.SetTraceInput(_rootActivity, input);
        CollectedInput ??= input;
    }

    /// <summary>
    ///     Sets trace-level output.
    /// </summary>
    public void SetOutput(object output)
    {
        GenAiActivityHelper.SetTraceOutput(_rootActivity, output);
        CollectedOutput = output;
    }

    /// <summary>
    ///     Creates a generation (LLM call) observation.
    /// </summary>
    public OtelGeneration CreateGeneration(string name, GenAiChatCompletionConfig config)
    {
        var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, name, config);
        return new OtelGeneration(this, activity, scoped: false);
    }

    /// <summary>
    ///     Creates a scoped generation that becomes the parent for subsequent observations.
    /// </summary>
    public OtelGeneration CreateGenerationScoped(string name, GenAiChatCompletionConfig config)
    {
        var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, name, config);
        var generation = new OtelGeneration(this, activity, scoped: true);

        if (activity != null)
        {
            _activityStack.Push(activity);
        }

        return generation;
    }

    /// <summary>
    ///     Creates a span observation.
    /// </summary>
    public OtelSpan CreateSpan(string name, SpanConfig? config = null)
    {
        config ??= new SpanConfig();
        var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, name, config, CurrentActivity);
        return new OtelSpan(this, activity, scoped: false);
    }

    /// <summary>
    ///     Creates a scoped span that becomes the parent for subsequent observations.
    /// </summary>
    public OtelSpan CreateSpanScoped(string name, SpanConfig? config = null)
    {
        config ??= new SpanConfig();
        var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, name, config, CurrentActivity);
        var span = new OtelSpan(this, activity, scoped: true);

        if (activity != null)
        {
            _activityStack.Push(activity);
        }

        return span;
    }

    /// <summary>
    ///     Creates a tool call observation.
    /// </summary>
    public OtelToolCall CreateToolCall(string name, string toolName, string? toolDescription = null,
        string toolType = "function", string? toolCallId = null)
    {
        var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, name, toolName, toolDescription, toolType,
                toolCallId);
        return new OtelToolCall(this, activity, scoped: false);
    }

    /// <summary>
    ///     Creates a scoped tool call that becomes the parent for subsequent observations.
    /// </summary>
    public OtelToolCall CreateToolCallScoped(string name, string toolName, string? toolDescription = null,
        string toolType = "function", string? toolCallId = null)
    {
        var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, name, toolName, toolDescription, toolType,
                toolCallId);
        var toolCall = new OtelToolCall(this, activity, scoped: true);

        if (activity != null)
        {
            _activityStack.Push(activity);
        }

        return toolCall;
    }

    /// <summary>
    ///     Creates an event observation.
    /// </summary>
    public OtelEvent CreateEvent(string name, object? input = null, object? output = null)
    {
        var activity = _activitySource.StartActivity(name);
        activity?.SetTag("langfuse.observation.type", "event");

        if (input != null)
        {
            GenAiActivityHelper.SetObservationInput(activity, input);
        }

        if (output != null)
        {
            GenAiActivityHelper.SetObservationOutput(activity, output);
        }

        return new OtelEvent(this, activity);
    }

    /// <summary>
    ///     Creates an embeddings observation.
    /// </summary>
    public OtelEmbedding CreateEmbedding(string name, GenAiEmbeddingsConfig config)
    {
        var activity = GenAiActivityHelper.CreateEmbeddingsActivity(_activitySource, name, config);
        return new OtelEmbedding(this, activity, scoped: false);
    }

    /// <summary>
    ///     Creates an agent observation.
    /// </summary>
    public OtelAgent CreateAgent(string name, GenAiAgentConfig config)
    {
        var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, name, config);
        return new OtelAgent(this, activity, scoped: false);
    }

    /// <summary>
    ///     Creates a scoped agent that becomes the parent for subsequent observations.
    /// </summary>
    public OtelAgent CreateAgentScoped(string name, GenAiAgentConfig config)
    {
        var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, name, config);
        var agent = new OtelAgent(this, activity, scoped: true);

        if (activity != null)
        {
            _activityStack.Push(activity);
        }

        return agent;
    }

    /// <summary>
    ///     Called by child observations to propagate input to the trace.
    /// </summary>
    internal void PropagateInput(object input)
    {
        if (CollectedInput == null)
        {
            CollectedInput = input;
            GenAiActivityHelper.SetTraceInput(_rootActivity, input);
        }
    }

    /// <summary>
    ///     Called by child observations to propagate output to the trace.
    /// </summary>
    internal void PropagateOutput(object output)
    {
        CollectedOutput = output;
        GenAiActivityHelper.SetTraceOutput(_rootActivity, output);
    }

    /// <summary>
    ///     Pops the current activity from the stack (called when a scoped observation ends).
    /// </summary>
    internal void PopActivity()
    {
        if (_activityStack.Count > 1) // Keep root activity
        {
            _activityStack.Pop();
        }
    }

    /// <summary>
    ///     Disposes the trace and all activities.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        while (_activityStack.Count > 0)
        {
            var activity = _activityStack.Pop();
            activity.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}