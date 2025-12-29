namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for creating root trace activities with cross-span attributes.
/// </summary>
public class TraceConfig
{
    /// <summary>
    ///     The display name for the trace.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     A custom trace identifier. If not provided, one will be generated.
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    ///     The identifier of the user associated with this trace.
    /// </summary>
    public string? UserId { get; init; }

    /// <summary>
    ///     The session identifier for grouping related traces.
    /// </summary>
    public string? SessionId { get; init; }

    /// <summary>
    ///     The deployment environment (e.g., "production", "staging").
    /// </summary>
    public string? Environment { get; init; }

    /// <summary>
    ///     The release identifier for the application version.
    /// </summary>
    public string? Release { get; init; }

    /// <summary>
    ///     The version identifier for the application.
    /// </summary>
    public string? Version { get; init; }

    /// <summary>
    ///     The name of the service generating this trace.
    /// </summary>
    public string? ServiceName { get; init; }

    /// <summary>
    ///     The version of the service generating this trace.
    /// </summary>
    public string? ServiceVersion { get; init; }

    /// <summary>
    ///     Whether this trace should be publicly accessible.
    /// </summary>
    public bool? Public { get; init; }

    /// <summary>
    ///     Tags for categorizing and filtering traces.
    /// </summary>
    public List<string>? Tags { get; init; }

    /// <summary>
    ///     The input data for this trace.
    /// </summary>
    public object? Input { get; init; }

    /// <summary>
    ///     The output data for this trace.
    /// </summary>
    public object? Output { get; init; }

    /// <summary>
    ///     Additional metadata associated with this trace.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}