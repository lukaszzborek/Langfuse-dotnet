namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for creating root trace activities with cross-span attributes.
/// </summary>
public class TraceConfig
{
    // Trace identification
    public string? Name { get; init; }
    public string? TraceId { get; init; }

    // User & Session
    public string? UserId { get; init; }
    public string? SessionId { get; init; }

    // Environment & Versioning
    public string? Environment { get; init; }
    public string? Release { get; init; }
    public string? Version { get; init; }

    // Service info
    public string? ServiceName { get; init; }
    public string? ServiceVersion { get; init; }

    // Langfuse-specific
    public bool? Public { get; init; }
    public List<string>? Tags { get; init; }
    public object? Input { get; init; }
    public object? Output { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}