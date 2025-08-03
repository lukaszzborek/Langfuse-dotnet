using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimSchemaExtension
{
    [JsonPropertyName("schema")]
    public string Schema { get; set; } = string.Empty;

    [JsonPropertyName("required")]
    public bool Required { get; set; }
}