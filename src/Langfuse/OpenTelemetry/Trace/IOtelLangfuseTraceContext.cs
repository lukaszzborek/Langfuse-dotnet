using zborek.Langfuse.OpenTelemetry.Models;

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
    ///     Should be called once at the beginning of a request/operation.
    /// </summary>
    /// <param name="traceName">The name of the trace.</param>
    /// <param name="config">Optional trace configuration.</param>
    /// <returns>The created trace.</returns>
    /// <exception cref="InvalidOperationException">Thrown if a trace is already active.</exception>
    OtelLangfuseTrace StartTrace(string traceName, TraceConfig? config = null);

    /// <summary>
    ///     Creates a generation observation on the current trace.
    /// </summary>
    OtelGeneration CreateGeneration(string name, GenAiChatCompletionConfig config);

    /// <summary>
    ///     Creates a scoped generation observation on the current trace.
    /// </summary>
    OtelGeneration CreateGenerationScoped(string name, GenAiChatCompletionConfig config);

    /// <summary>
    ///     Creates a span observation on the current trace.
    /// </summary>
    OtelSpan CreateSpan(string name, SpanConfig? config = null);

    /// <summary>
    ///     Creates a scoped span observation on the current trace.
    /// </summary>
    OtelSpan CreateSpanScoped(string name, SpanConfig? config = null);

    /// <summary>
    ///     Creates an event observation on the current trace.
    /// </summary>
    OtelEvent CreateEvent(string name, object? input = null, object? output = null);

    /// <summary>
    ///     Creates a tool call observation on the current trace.
    /// </summary>
    OtelToolCall CreateToolCall(string name, string toolName, string? toolDescription = null,
        string toolType = "function", string? toolCallId = null);

    /// <summary>
    ///     Creates an embedding observation on the current trace.
    /// </summary>
    OtelEmbedding CreateEmbedding(string name, GenAiEmbeddingsConfig config);

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
    private OtelLangfuseTrace? _currentTrace;
    private bool _disposed;

    /// <inheritdoc />
    public OtelLangfuseTrace? CurrentTrace => _currentTrace;

    /// <inheritdoc />
    public bool HasActiveTrace => _currentTrace != null;
    
    public OtelLangfuseTraceContext()
    {
        
    }

    public OtelLangfuseTraceContext(string traceName, TraceConfig? config = null)
    {
        StartTrace(traceName, config);
    }
    
    /// <inheritdoc />
    public OtelLangfuseTrace StartTrace(string traceName, TraceConfig? config = null)
    {
        if (_currentTrace != null)
        {
            throw new InvalidOperationException(
                "A trace is already active in this context. Only one trace per scope is allowed.");
        }

        _currentTrace = new OtelLangfuseTrace(traceName, config);
        return _currentTrace;
    }

    /// <inheritdoc />
    public OtelGeneration CreateGeneration(string name, GenAiChatCompletionConfig config)
    {
        EnsureTraceActive();
        return _currentTrace!.CreateGeneration(name, config);
    }

    /// <inheritdoc />
    public OtelGeneration CreateGenerationScoped(string name, GenAiChatCompletionConfig config)
    {
        EnsureTraceActive();
        return _currentTrace!.CreateGenerationScoped(name, config);
    }

    /// <inheritdoc />
    public OtelSpan CreateSpan(string name, SpanConfig? config = null)
    {
        EnsureTraceActive();
        return _currentTrace!.CreateSpan(name, config);
    }

    /// <inheritdoc />
    public OtelSpan CreateSpanScoped(string name, SpanConfig? config = null)
    {
        EnsureTraceActive();
        return _currentTrace!.CreateSpanScoped(name, config);
    }

    /// <inheritdoc />
    public OtelEvent CreateEvent(string name, object? input = null, object? output = null)
    {
        EnsureTraceActive();
        return _currentTrace!.CreateEvent(name, input, output);
    }

    /// <inheritdoc />
    public OtelToolCall CreateToolCall(string name, string toolName, string? toolDescription = null,
        string toolType = "function", string? toolCallId = null)
    {
        EnsureTraceActive();
        return _currentTrace!.CreateToolCall(name, toolName, toolDescription, toolType, toolCallId);
    }

    /// <inheritdoc />
    public OtelEmbedding CreateEmbedding(string name, GenAiEmbeddingsConfig config)
    {
        EnsureTraceActive();
        return _currentTrace!.CreateEmbedding(name, config);
    }

    /// <inheritdoc />
    public void SetInput(object input)
    {
        EnsureTraceActive();
        _currentTrace!.SetInput(input);
    }

    /// <inheritdoc />
    public void SetOutput(object output)
    {
        EnsureTraceActive();
        _currentTrace!.SetOutput(output);
    }

    private void EnsureTraceActive()
    {
        if (_currentTrace == null)
        {
            throw new InvalidOperationException(
                "No active trace in this context. Call StartTrace() first.");
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _currentTrace?.Dispose();
        _currentTrace = null;

        GC.SuppressFinalize(this);
    }
}
