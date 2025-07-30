using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class DatasetItemListRequest
{
    [JsonPropertyName("datasetName")]
    public string? DatasetName { get; set; }

    [JsonPropertyName("sourceTraceId")]
    public string? SourceTraceId { get; set; }

    [JsonPropertyName("sourceObservationId")]
    public string? SourceObservationId { get; set; }

    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}