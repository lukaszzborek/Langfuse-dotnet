using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Responses;

public class ScimResourceTypesResponse
{
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("Resources")]
    public List<ScimResourceType> Resources { get; set; } = new();
}