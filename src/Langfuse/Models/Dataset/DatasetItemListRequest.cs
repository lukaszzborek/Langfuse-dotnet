using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Request parameters for listing dataset items.
/// </summary>
public class DatasetItemListRequest
{
    /// <summary>
    ///     Filter by dataset name.
    /// </summary>
    [JsonPropertyName("datasetName")]
    public string? DatasetName { get; set; }

    /// <summary>
    ///     Filter by source trace ID.
    /// </summary>
    [JsonPropertyName("sourceTraceId")]
    public string? SourceTraceId { get; set; }

    /// <summary>
    ///     Filter by source observation ID.
    /// </summary>
    [JsonPropertyName("sourceObservationId")]
    public string? SourceObservationId { get; set; }

    /// <summary>
    ///     Page number, starts at 1.
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    ///     Limit of items per page.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}