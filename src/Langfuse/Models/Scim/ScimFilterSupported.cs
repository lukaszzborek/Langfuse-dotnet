using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimFilterSupported : ScimSupported
{
    [JsonPropertyName("maxResults")]
    public int? MaxResults { get; set; }
}