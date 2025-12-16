using System.Diagnostics;
using OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     OpenTelemetry-based Langfuse trace that manages activities using Activity.Current for parent-child relationships.
///     Context propagation is handled via Baggage and ActivityListener.
///     Can be registered as scoped service and used with StartTrace() for lazy initialization.
/// </summary>
public class OtelLangfuseTrace : IOtelLangfuseTrace
{
    /// <summary>
    ///     The ActivitySource name used by Langfuse traces.
    /// </summary>
    public const string ActivitySourceName = "Langfuse";

    private static readonly ActivitySource DefaultActivitySource = new(ActivitySourceName);

    private readonly ActivitySource _activitySource;
    private bool _disposed;
    private bool _started;

    /// <summary>
    ///     The root trace activity.
    /// </summary>
    public Activity? TraceActivity { get; private set; }

    /// <summary>
    ///     Gets whether a trace is currently active.
    /// </summary>
    public bool HasActiveTrace => _started && TraceActivity != null;

    /// <summary>
    ///     Creates a new OtelLangfuseTrace without starting a trace.
    ///     Use StartTrace() to begin tracing.
    /// </summary>
    public OtelLangfuseTrace()
        : this(DefaultActivitySource)
    {
    }

    /// <summary>
    ///     Creates a new OtelLangfuseTrace with a custom ActivitySource without starting a trace.
    ///     Use StartTrace() to begin tracing.
    /// </summary>
    public OtelLangfuseTrace(ActivitySource activitySource)
    {
        _activitySource = activitySource;
    }

    /// <summary>
    ///     Creates a new OtelLangfuseTrace and immediately starts the trace.
    ///     Convenience constructor for immediate trace creation.
    /// </summary>
    /// <param name="traceName">The name of the trace.</param>
    /// <param name="userId">Optional user ID for the trace.</param>
    /// <param name="sessionId">Optional session ID for the trace.</param>
    /// <param name="version">Optional version string.</param>
    /// <param name="release">Optional release string.</param>
    /// <param name="tags">Optional tags for the trace.</param>
    /// <param name="input">Optional input for the trace.</param>
    /// <param name="isRoot">If true, creates a new root trace (new TraceId) ignoring any current activity context.</param>
    public OtelLangfuseTrace(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null,
        bool isRoot = false)
        : this(DefaultActivitySource)
    {
        StartTrace(traceName, userId, sessionId, version, release, tags, input, isRoot);
    }

    /// <summary>
    ///     Creates a new OtelLangfuseTrace with a custom ActivitySource and immediately starts the trace.
    /// </summary>
    public OtelLangfuseTrace(
        ActivitySource activitySource,
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null,
        bool isRoot = false)
        : this(activitySource)
    {
        StartTrace(traceName, userId, sessionId, version, release, tags, input, isRoot);
    }

    /// <summary>
    ///     Starts the trace with the specified parameters.
    /// </summary>
    /// <param name="traceName">The name of the trace.</param>
    /// <param name="userId">Optional user ID for the trace.</param>
    /// <param name="sessionId">Optional session ID for the trace.</param>
    /// <param name="version">Optional version string.</param>
    /// <param name="release">Optional release string.</param>
    /// <param name="tags">Optional tags for the trace.</param>
    /// <param name="input">Optional input for the trace.</param>
    /// <param name="isRoot">If true, creates a new root trace (new TraceId) ignoring any current activity context.</param>
    /// <returns>This trace instance for fluent API.</returns>
    public IOtelLangfuseTrace StartTrace(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null,
        bool isRoot = false)
    {
        if (_started)
        {
            throw new InvalidOperationException(
                "A trace is already active. Only one trace per instance is allowed.");
        }

        // Set context in Baggage for auto-enrichment by ActivityListener
        SetBaggageContext(userId, sessionId, version, release, tags);

        // Create trace config for the helper
        var config = new TraceConfig
        {
            UserId = userId,
            SessionId = sessionId,
            Version = version,
            Tags = tags?.ToList()
        };

        TraceActivity = GenAiActivityHelper.CreateTraceActivity(_activitySource, traceName, config, isRoot);

        if (input != null && TraceActivity != null)
        {
            GenAiActivityHelper.SetTraceInput(TraceActivity, input);
        }

        _started = true;
        return this;
    }

    /// <summary>
    ///     Disposes the trace activity and clears Baggage context.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _started = false;
        ClearBaggageContext();
        TraceActivity?.Dispose();
        TraceActivity = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Sets the trace name.
    /// </summary>
    public void SetTraceName(string name)
    {
        if (TraceActivity is null)
        {
            return;
        }

        TraceActivity.SetTag(LangfuseAttributes.TraceName, name);
        TraceActivity.DisplayName = name;
    }

    /// <summary>
    ///     Sets trace-level input.
    /// </summary>
    public void SetInput(object input)
    {
        if (TraceActivity is null)
        {
            return;
        }

        GenAiActivityHelper.SetTraceInput(TraceActivity, input);
    }

    /// <summary>
    ///     Sets trace-level output.
    /// </summary>
    public void SetOutput(object output)
    {
        if (TraceActivity is null)
        {
            return;
        }

        GenAiActivityHelper.SetTraceOutput(TraceActivity, output);
    }

    /// <summary>
    ///     Marks this trace as skipped. Skipped traces and all their observations will not be exported to Langfuse.
    ///     Use this when an entire operation turns out to be unnecessary (e.g., request already processed).
    /// </summary>
    public void SkipTrace()
    {
        if (TraceActivity == null)
        {
            return;
        }

        TraceActivity.IsAllDataRequested = false;
        TraceActivity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
    }

    /// <summary>
    ///     Gets whether this trace has been marked as skipped.
    /// </summary>
    public bool IsSkipped
    {
        get
        {
            if (TraceActivity == null)
            {
                return false;
            }

            return !TraceActivity.IsAllDataRequested || !TraceActivity.Recorded;
        }
    }

    /// <summary>
    ///     Creates a span observation.
    /// </summary>
    /// <param name="name">The span name.</param>
    /// <param name="type">Optional span type (e.g., "workflow", "retrieval").</param>
    /// <param name="description">Optional description.</param>
    /// <param name="input">Optional input data.</param>
    /// <param name="configure">Optional action to configure the span.</param>
    public OtelSpan CreateSpan(
        string name,
        string? type = null,
        string? description = null,
        object? input = null,
        Action<OtelSpan>? configure = null)
    {
        if (!HasActiveTrace)
        {
            return new OtelSpan(null);
        }

        var config = new SpanConfig
        {
            SpanType = type,
            Description = description
        };

        var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, name, config);
        var span = new OtelSpan(activity);

        if (input != null)
        {
            span.SetInput(input);
        }

        configure?.Invoke(span);
        return span;
    }

    /// <summary>
    ///     Creates a generation (LLM call) observation.
    /// </summary>
    /// <param name="name">The generation name.</param>
    /// <param name="model">The model name (e.g., "gpt-4").</param>
    /// <param name="provider">Optional provider name (e.g., "openai").</param>
    /// <param name="input">Optional input (messages or prompt).</param>
    /// <param name="configure">Optional action to configure the generation.</param>
    public OtelGeneration CreateGeneration(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelGeneration>? configure = null)
    {
        if (!HasActiveTrace)
        {
            return new OtelGeneration(null);
        }

        var config = new GenAiChatCompletionConfig
        {
            Model = model,
            Provider = provider
        };

        var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, name, config);
        var generation = new OtelGeneration(activity);

        if (input != null)
        {
            generation.SetInput(input);
        }

        configure?.Invoke(generation);
        return generation;
    }

    /// <summary>
    ///     Creates a tool call observation.
    /// </summary>
    /// <param name="name">The observation name.</param>
    /// <param name="toolName">The tool name.</param>
    /// <param name="toolDescription">Optional tool description.</param>
    /// <param name="toolType">Tool type (default: "function").</param>
    /// <param name="input">Optional input arguments.</param>
    /// <param name="configure">Optional action to configure the tool call.</param>
    public OtelToolCall CreateToolCall(
        string name,
        string toolName,
        string? toolDescription = null,
        string toolType = "function",
        object? input = null,
        Action<OtelToolCall>? configure = null)
    {
        if (!HasActiveTrace)
        {
            return new OtelToolCall(null);
        }

        var activity = GenAiActivityHelper.CreateToolCallActivity(
            _activitySource, name, toolName, toolDescription, toolType);
        var toolCall = new OtelToolCall(activity);

        if (input != null)
        {
            toolCall.SetArguments(input);
        }

        configure?.Invoke(toolCall);
        return toolCall;
    }

    /// <summary>
    ///     Creates an event observation.
    /// </summary>
    /// <param name="name">The event name.</param>
    /// <param name="input">Optional input data.</param>
    /// <param name="output">Optional output data.</param>
    public OtelEvent CreateEvent(string name, object? input = null, object? output = null)
    {
        if (!HasActiveTrace)
        {
            return new OtelEvent(null);
        }

        var activity = _activitySource.StartActivity(name);
        activity?.SetTag(LangfuseAttributes.ObservationType, LangfuseAttributes.ObservationTypeEvent);

        var otelEvent = new OtelEvent(activity);

        if (input != null)
        {
            otelEvent.SetInput(input);
        }

        if (output != null)
        {
            otelEvent.SetOutput(output);
        }

        return otelEvent;
    }

    /// <summary>
    ///     Creates an embedding observation.
    /// </summary>
    /// <param name="name">The embedding name.</param>
    /// <param name="model">The model name.</param>
    /// <param name="provider">Optional provider name.</param>
    /// <param name="input">Optional input text.</param>
    /// <param name="configure">Optional action to configure the embedding.</param>
    public OtelEmbedding CreateEmbedding(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelEmbedding>? configure = null)
    {
        if (!HasActiveTrace)
        {
            return new OtelEmbedding(null);
        }

        var config = new GenAiEmbeddingsConfig
        {
            Model = model,
            Provider = provider
        };

        var activity = GenAiActivityHelper.CreateEmbeddingsActivity(_activitySource, name, config);
        var embedding = new OtelEmbedding(activity);

        if (input != null)
        {
            embedding.SetInput(input);
        }

        configure?.Invoke(embedding);
        return embedding;
    }

    /// <summary>
    ///     Creates an agent observation.
    /// </summary>
    /// <param name="name">The agent name.</param>
    /// <param name="agentId">The agent ID.</param>
    /// <param name="description">Optional agent description.</param>
    /// <param name="input">Optional input data.</param>
    /// <param name="configure">Optional action to configure the agent.</param>
    public OtelAgent CreateAgent(
        string name,
        string agentId,
        string? description = null,
        object? input = null,
        Action<OtelAgent>? configure = null)
    {
        if (!HasActiveTrace)
        {
            return new OtelAgent(null);
        }

        var config = new GenAiAgentConfig
        {
            Id = agentId,
            Name = name,
            Description = description
        };

        var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, name, config);
        var agent = new OtelAgent(activity);

        if (input != null)
        {
            agent.SetInput(input);
        }

        configure?.Invoke(agent);
        return agent;
    }

    /// <summary>
    ///     Creates a detached trace that is NOT managed by this instance.
    ///     Useful for parallel operations or background tasks.
    /// </summary>
    public static OtelLangfuseTrace CreateDetachedTrace(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null)
    {
        var trace = new OtelLangfuseTrace();
        trace.StartTrace(traceName, userId, sessionId, version, release, tags, input, true);
        return trace;
    }

    private static void SetBaggageContext(
        string? userId,
        string? sessionId,
        string? version,
        string? release,
        IEnumerable<string>? tags)
    {
        if (!string.IsNullOrEmpty(userId))
        {
            Baggage.SetBaggage(LangfuseBaggageKeys.UserId, userId);
        }

        if (!string.IsNullOrEmpty(sessionId))
        {
            Baggage.SetBaggage(LangfuseBaggageKeys.SessionId, sessionId);
        }

        if (!string.IsNullOrEmpty(version))
        {
            Baggage.SetBaggage(LangfuseBaggageKeys.Version, version);
        }

        if (!string.IsNullOrEmpty(release))
        {
            Baggage.SetBaggage(LangfuseBaggageKeys.Release, release);
        }

        if (tags != null)
        {
            Baggage.SetBaggage(LangfuseBaggageKeys.Tags, string.Join(",", tags));
        }
    }

    private static void ClearBaggageContext()
    {
        Baggage.RemoveBaggage(LangfuseBaggageKeys.UserId);
        Baggage.RemoveBaggage(LangfuseBaggageKeys.SessionId);
        Baggage.RemoveBaggage(LangfuseBaggageKeys.Version);
        Baggage.RemoveBaggage(LangfuseBaggageKeys.Release);
        Baggage.RemoveBaggage(LangfuseBaggageKeys.Tags);
    }
}