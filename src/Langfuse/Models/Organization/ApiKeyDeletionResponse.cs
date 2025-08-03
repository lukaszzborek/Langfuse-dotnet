using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

public class ApiKeyDeletionResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}