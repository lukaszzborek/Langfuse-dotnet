using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class ApiKeyDeletionResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}