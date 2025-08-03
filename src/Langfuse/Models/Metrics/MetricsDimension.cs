using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Metrics;

/// <summary>
///     Dimension for grouping metrics data
/// </summary>
public class MetricsDimension
{
    /// <summary>
    ///     Field to group by, e.g. "name", "userId", "sessionId"
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;
}