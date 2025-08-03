using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

public class ModelListRequest
{
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}