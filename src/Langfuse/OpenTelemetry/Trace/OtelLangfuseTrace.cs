using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     OpenTelemetry-based Langfuse trace that manages activities and parent-child relationships.
///     Similar to LangfuseTrace but uses OpenTelemetry activities instead of direct ingestion.
/// </summary>
public class OtelLangfuseTrace : IDisposable
{
    /// <summary>
    ///     The ActivitySource name used by Langfuse traces.
    /// </summary>
    public const string ActivitySourceName = "Langfuse";

    private static readonly ActivitySource DefaultActivitySource = new(ActivitySourceName);

    private readonly ActivitySource _activitySource;
    private readonly Stack<Activity> _activityStack = new();
    private readonly TraceConfig _config;
    private bool _disposed;

    /// <summary>
    ///     The root trace activity.
    /// </summary>
    public Activity? TraceActivity { get; }

    /// <summary>
    ///     The current active activity (top of the stack).
    /// </summary>
    public Activity? CurrentActivity => _activityStack.Count > 0 ? _activityStack.Peek() : TraceActivity;

    /// <summary>
    ///     Input collected from child observations (propagated to trace).
    /// </summary>
    public object? CollectedInput { get; private set; }

    /// <summary>
    ///     Output collected from child observations (propagated to trace).
    /// </summary>
    public object? CollectedOutput { get; private set; }

    /// <summary>
    ///     Creates a new OpenTelemetry-based Langfuse trace using the default ActivitySource.
    /// </summary>
    /// <param name="traceName">The name of the trace.</param>
    /// <param name="config">Optional trace configuration.</param>
    public OtelLangfuseTrace(string traceName, TraceConfig? config = null)
        : this(DefaultActivitySource, traceName, config)
    {
    }

    /// <summary>
    ///     Creates a new OpenTelemetry-based Langfuse trace with a custom ActivitySource.
    /// </summary>
    /// <param name="activitySource">The activity source to use for creating activities.</param>
    /// <param name="traceName">The name of the trace.</param>
    /// <param name="config">Optional trace configuration.</param>
    public OtelLangfuseTrace(ActivitySource activitySource, string traceName, TraceConfig? config = null)
    {
        _activitySource = activitySource;
        _config = config ?? new TraceConfig();

        TraceActivity = GenAiActivityHelper.CreateTraceActivity(activitySource, traceName, _config);

        if (TraceActivity != null)
        {
            _activityStack.Push(TraceActivity);
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

    /// <summary>
    ///     Sets the trace name.
    /// </summary>
    public void SetTraceName(string name)
    {
        TraceActivity?.SetTag(LangfuseAttributes.TraceName, name);
    }

    /// <summary>
    ///     Sets trace-level input.
    /// </summary>
    public void SetInput(object input)
    {
        GenAiActivityHelper.SetTraceInput(TraceActivity, input);
        CollectedInput ??= input;
    }

    /// <summary>
    ///     Sets trace-level output.
    /// </summary>
    public void SetOutput(object output)
    {
        GenAiActivityHelper.SetTraceOutput(TraceActivity, output);
        CollectedOutput = output;
    }

    /// <summary>
    ///     Creates a generation (LLM call) observation.
    /// </summary>
    public OtelGeneration CreateGeneration(string name, GenAiChatCompletionConfig config)
    {
        var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, name, config);
        ApplyTraceContext(activity);
        return new OtelGeneration(this, activity, false);
    }

    /// <summary>
    ///     Creates a scoped generation that becomes the parent for subsequent observations.
    /// </summary>
    public OtelGeneration CreateGenerationScoped(string name, GenAiChatCompletionConfig config)
    {
        var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, name, config);
        ApplyTraceContext(activity);
        var generation = new OtelGeneration(this, activity, true);

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
        ApplyTraceContext(activity);
        return new OtelSpan(this, activity, false);
    }

    /// <summary>
    ///     Creates a scoped span that becomes the parent for subsequent observations.
    /// </summary>
    public OtelSpan CreateSpanScoped(string name, SpanConfig? config = null)
    {
        config ??= new SpanConfig();
        var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, name, config, CurrentActivity);
        ApplyTraceContext(activity);
        var span = new OtelSpan(this, activity, true);

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
        ApplyTraceContext(activity);
        return new OtelToolCall(this, activity, false);
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
        ApplyTraceContext(activity);
        var toolCall = new OtelToolCall(this, activity, true);

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
        activity?.SetTag(LangfuseAttributes.ObservationType, LangfuseAttributes.ObservationTypeEvent);
        ApplyTraceContext(activity);

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
        ApplyTraceContext(activity);
        return new OtelEmbedding(this, activity, false);
    }

    /// <summary>
    ///     Creates an agent observation.
    /// </summary>
    public OtelAgent CreateAgent(string name, GenAiAgentConfig config)
    {
        var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, name, config);
        ApplyTraceContext(activity);
        return new OtelAgent(this, activity, false);
    }

    /// <summary>
    ///     Creates a scoped agent that becomes the parent for subsequent observations.
    /// </summary>
    public OtelAgent CreateAgentScoped(string name, GenAiAgentConfig config)
    {
        var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, name, config);
        ApplyTraceContext(activity);
        var agent = new OtelAgent(this, activity, true);

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
            GenAiActivityHelper.SetTraceInput(TraceActivity, input);
        }
    }

    /// <summary>
    ///     Called by child observations to propagate output to the trace.
    /// </summary>
    internal void PropagateOutput(object output)
    {
        CollectedOutput = output;
        GenAiActivityHelper.SetTraceOutput(TraceActivity, output);
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
    ///     Applies trace-level context (user_id, session_id, etc.) to child observations.
    /// </summary>
    private void ApplyTraceContext(Activity? activity)
    {
        if (activity == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(_config.UserId))
        {
            activity.SetTag(LangfuseAttributes.UserId, _config.UserId);
        }

        if (!string.IsNullOrEmpty(_config.SessionId))
        {
            activity.SetTag(LangfuseAttributes.SessionId, _config.SessionId);
        }

        if (!string.IsNullOrEmpty(_config.Version))
        {
            activity.SetTag(LangfuseAttributes.Version, _config.Version);
        }

        if (_config.Tags is { Count: > 0 })
        {
            activity.SetTag(LangfuseAttributes.TraceTags, _config.Tags);
        }

        if (_config.Metadata is { Count: > 0 })
        {
            foreach (var (key, value) in _config.Metadata)
            {
                activity.SetTag($"{LangfuseAttributes.ObservationMetadataPrefix}{key}", value);
            }
        }
    }
}