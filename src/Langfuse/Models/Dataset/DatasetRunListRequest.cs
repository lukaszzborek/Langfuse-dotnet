using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

public class DatasetRunListRequest
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}