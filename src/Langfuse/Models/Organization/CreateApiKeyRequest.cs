using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

public class CreateApiKeyRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}