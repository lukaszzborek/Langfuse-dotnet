using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Response from metrics endpoint
/// </summary>
public class MetricsResponse
{
    /// <summary>
    ///     The metrics data. Each item in the list contains the metric values
    ///     and dimensions requested in the query.
    ///     Format varies based on the query parameters.
    ///     Histograms will return an array with [lower, upper, height] tuples.
    /// </summary>
    [JsonPropertyName("data")]
    public object[] Data { get; set; } = [];
}