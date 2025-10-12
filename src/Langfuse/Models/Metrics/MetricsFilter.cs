using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Metrics;

/// <summary>
///     Filter for metrics data
/// </summary>
public class MetricsFilter
{
    /// <summary>
    ///     Column to filter on
    /// </summary>
    [JsonPropertyName("column")]
    public string Column { get; set; } = string.Empty;

    /// <summary>
    ///     Operator, e.g. "=", ">", "<", "contains"
    /// </summary>
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    ///     Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    ///     Data type, e.g. "string", "number", "stringObject"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     Required only when filtering on metadata
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }
}