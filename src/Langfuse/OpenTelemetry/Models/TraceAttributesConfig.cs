namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for setting trace-level attributes that propagate across spans.
/// </summary>
public class TraceAttributesConfig
{
    public string? UserId { get; init; }
    public string? SessionId { get; init; }
    public string? Environment { get; init; }
    public List<string>? Tags { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}