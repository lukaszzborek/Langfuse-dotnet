using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Metrics;

/// <summary>
///     Request parameters for metrics query
/// </summary>
public class MetricsRequest
{
    /// <summary>
    ///     JSON string containing the query parameters
    /// </summary>
    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;
}