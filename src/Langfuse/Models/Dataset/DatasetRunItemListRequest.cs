using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

public class DatasetRunItemListRequest
{
    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    [JsonPropertyName("runName")]
    public string RunName { get; set; } = string.Empty;

    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}