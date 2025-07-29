using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class DatasetListRequest
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}