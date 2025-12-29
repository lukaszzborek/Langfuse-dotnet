namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for creating span activities within a trace.
/// </summary>
public class SpanConfig
{
    /// <summary>
    ///     The type of span (e.g., "http", "db", "custom").
    /// </summary>
    public string? SpanType { get; init; }

    /// <summary>
    ///     A human-readable description of the span's purpose.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     Additional attributes to attach to the span.
    /// </summary>
    public Dictionary<string, object>? Attributes { get; init; }
}