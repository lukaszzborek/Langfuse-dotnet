using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.MetricsV2;

/// <summary>
///     Response from the V2 metrics API endpoint.
/// </summary>
public class MetricsV2Response
{
    /// <summary>
    ///     The metrics data. Each item in the list contains the metric values and dimensions requested in the query.
    ///     Format varies based on the query parameters.
    ///     Histograms will return an array with [lower, upper, height] tuples.
    /// </summary>
    [JsonPropertyName("data")]
    public List<Dictionary<string, object?>> Data { get; set; } = [];
}
