namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for creating span activities within a trace.
/// </summary>
public class SpanConfig
{
    public string? SpanType { get; init; }
    public string? Description { get; init; }
    public Dictionary<string, object>? Attributes { get; init; }
}