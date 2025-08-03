using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Metrics;

/// <summary>
///     Metric definition
/// </summary>
public class Metric
{
    /// <summary>
    ///     What to measure, e.g. "count", "latency", "value"
    /// </summary>
    [JsonPropertyName("measure")]
    public string Measure { get; set; } = string.Empty;

    /// <summary>
    ///     How to aggregate, e.g. "count", "sum", "avg", "p95", "histogram"
    /// </summary>
    [JsonPropertyName("aggregation")]
    public string Aggregation { get; set; } = string.Empty;
}