namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Scoped context for sharing OtelLangfuseTrace across services within a request.
///     Register as scoped service and use to create/access the current trace.
/// </summary>
public interface IOtelLangfuseTraceContext : IDisposable
{
    /// <summary>
    ///     Gets the current trace. Returns null if no trace has been started.
    /// </summary>
    OtelLangfuseTrace? CurrentTrace { get; }

    /// <summary>
    ///     Gets whether a trace is currently active.
    /// </summary>
    bool HasActiveTrace { get; }

    /// <summary>
    ///     Starts a new trace for the current scope.
    /// </summary>
    /// <param name="traceName">The name of the trace.</param>
    /// <param name="userId">Optional user ID.</param>
    /// <param name="sessionId">Optional session ID.</param>
    /// <param name="version">Optional version string.</param>
    /// <param name="release">Optional release string.</param>
    /// <param name="tags">Optional tags.</param>
    /// <param name="input">Optional input data.</param>
    /// <returns>The created trace.</returns>
    OtelLangfuseTrace StartTrace(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null);

    /// <summary>
    ///     Creates a detached trace that is NOT managed by this context.
    ///     Useful for parallel operations or background tasks.
    /// </summary>
    OtelLangfuseTrace CreateDetachedTrace(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null);

    /// <summary>
    ///     Creates a span observation on the current trace.
    /// </summary>
    OtelSpan CreateSpan(
        string name,
        string? type = null,
        string? description = null,
        object? input = null,
        Action<OtelSpan>? configure = null);

    /// <summary>
    ///     Creates a generation (LLM call) observation on the current trace.
    /// </summary>
    OtelGeneration CreateGeneration(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelGeneration>? configure = null);

    /// <summary>
    ///     Creates a tool call observation on the current trace.
    /// </summary>
    OtelToolCall CreateToolCall(
        string name,
        string toolName,
        string? toolDescription = null,
        string toolType = "function",
        object? input = null,
        Action<OtelToolCall>? configure = null);

    /// <summary>
    ///     Creates an event observation on the current trace.
    /// </summary>
    OtelEvent CreateEvent(string name, object? input = null, object? output = null);

    /// <summary>
    ///     Creates an embedding observation on the current trace.
    /// </summary>
    OtelEmbedding CreateEmbedding(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelEmbedding>? configure = null);

    /// <summary>
    ///     Creates an agent observation on the current trace.
    /// </summary>
    OtelAgent CreateAgent(
        string name,
        string agentId,
        string? description = null,
        object? input = null,
        Action<OtelAgent>? configure = null);

    /// <summary>
    ///     Sets the input on the current trace.
    /// </summary>
    void SetInput(object input);

    /// <summary>
    ///     Sets the output on the current trace.
    /// </summary>
    void SetOutput(object output);
}

/// <summary>
///     Default implementation of IOtelLangfuseTraceContext.
///     Manages a single trace per scope (typically per HTTP request).
/// </summary>
public class OtelLangfuseTraceContext : IOtelLangfuseTraceContext
{
    private bool _disposed;

    /// <inheritdoc />
    public OtelLangfuseTrace? CurrentTrace { get; private set; }

    /// <inheritdoc />
    public bool HasActiveTrace => CurrentTrace != null;

    public OtelLangfuseTraceContext()
    {
    }

    public OtelLangfuseTraceContext(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null)
    {
        StartTrace(traceName, userId, sessionId, version, release, tags, input);
    }

    /// <inheritdoc />
    public OtelLangfuseTrace StartTrace(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null)
    {
        if (CurrentTrace != null)
        {
            throw new InvalidOperationException(
                "A trace is already active in this context. Only one trace per scope is allowed.");
        }

        CurrentTrace = new OtelLangfuseTrace(traceName, userId, sessionId, version, release, tags, input);
        return CurrentTrace;
    }

    /// <inheritdoc />
    public OtelLangfuseTrace CreateDetachedTrace(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null)
    {
        return new OtelLangfuseTrace(traceName, userId, sessionId, version, release, tags, input, true);
    }

    /// <inheritdoc />
    public OtelSpan CreateSpan(
        string name,
        string? type = null,
        string? description = null,
        object? input = null,
        Action<OtelSpan>? configure = null)
    {
        EnsureTraceActive();
        return CurrentTrace!.CreateSpan(name, type, description, input, configure);
    }

    /// <inheritdoc />
    public OtelGeneration CreateGeneration(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelGeneration>? configure = null)
    {
        EnsureTraceActive();
        return CurrentTrace!.CreateGeneration(name, model, provider, input, configure);
    }

    /// <inheritdoc />
    public OtelToolCall CreateToolCall(
        string name,
        string toolName,
        string? toolDescription = null,
        string toolType = "function",
        object? input = null,
        Action<OtelToolCall>? configure = null)
    {
        EnsureTraceActive();
        return CurrentTrace!.CreateToolCall(name, toolName, toolDescription, toolType, input, configure);
    }

    /// <inheritdoc />
    public OtelEvent CreateEvent(string name, object? input = null, object? output = null)
    {
        EnsureTraceActive();
        return CurrentTrace!.CreateEvent(name, input, output);
    }

    /// <inheritdoc />
    public OtelEmbedding CreateEmbedding(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelEmbedding>? configure = null)
    {
        EnsureTraceActive();
        return CurrentTrace!.CreateEmbedding(name, model, provider, input, configure);
    }

    /// <inheritdoc />
    public OtelAgent CreateAgent(
        string name,
        string agentId,
        string? description = null,
        object? input = null,
        Action<OtelAgent>? configure = null)
    {
        EnsureTraceActive();
        return CurrentTrace!.CreateAgent(name, agentId, description, input, configure);
    }

    /// <inheritdoc />
    public void SetInput(object input)
    {
        EnsureTraceActive();
        CurrentTrace!.SetInput(input);
    }

    /// <inheritdoc />
    public void SetOutput(object output)
    {
        EnsureTraceActive();
        CurrentTrace!.SetOutput(output);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        CurrentTrace?.Dispose();
        CurrentTrace = null;

        GC.SuppressFinalize(this);
    }

    private void EnsureTraceActive()
    {
        if (CurrentTrace == null)
        {
            throw new InvalidOperationException(
                "No active trace in this context. Call StartTrace() first.");
        }
    }
}