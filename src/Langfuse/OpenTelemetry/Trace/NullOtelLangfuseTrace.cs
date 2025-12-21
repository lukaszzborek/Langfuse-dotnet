using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     No-op implementation of IOtelLangfuseTrace for testing scenarios
///     where Langfuse tracing is not needed.
/// </summary>
public sealed class NullOtelLangfuseTrace : IOtelLangfuseTrace
{
    /// <summary>
    ///     Singleton instance of the null trace.
    /// </summary>
    public static readonly NullOtelLangfuseTrace Instance = new();

    /// <inheritdoc />
    public Activity? TraceActivity => null;

    /// <inheritdoc />
    public bool HasActiveTrace => false;

    /// <inheritdoc />
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
        return this;
    }

    /// <inheritdoc />
    public void SetTraceName(string name)
    {
    }

    /// <inheritdoc />
    public void SetInput(object input)
    {
    }

    /// <inheritdoc />
    public void SetOutput(object output)
    {
    }

    /// <inheritdoc />
    public void Skip()
    {
    }

    /// <inheritdoc />
    public OtelSpan CreateSpan(
        string name,
        string? type = null,
        string? description = null,
        object? input = null,
        Action<OtelSpan>? configure = null)
    {
        return new OtelSpan(null);
    }

    /// <inheritdoc />
    public OtelGeneration CreateGeneration(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelGeneration>? configure = null)
    {
        return new OtelGeneration(null);
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
        return new OtelToolCall(null);
    }

    /// <inheritdoc />
    public OtelEvent CreateEvent(string name, object? input = null, object? output = null)
    {
        return new OtelEvent(null);
    }

    /// <inheritdoc />
    public OtelEmbedding CreateEmbedding(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelEmbedding>? configure = null)
    {
        return new OtelEmbedding(null);
    }

    /// <inheritdoc />
    public OtelAgent CreateAgent(
        string name,
        string agentId,
        string? description = null,
        object? input = null,
        Action<OtelAgent>? configure = null)
    {
        return new OtelAgent(null);
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }
}
