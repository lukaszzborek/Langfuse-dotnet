using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimSupported
{
    [JsonPropertyName("supported")]
    public bool Supported { get; set; }
}