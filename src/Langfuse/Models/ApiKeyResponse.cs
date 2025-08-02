using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class ApiKeyResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("publicKey")]
    public string PublicKey { get; set; } = string.Empty;

    [JsonPropertyName("secretKey")]
    public string SecretKey { get; set; } = string.Empty;

    [JsonPropertyName("displaySecretKey")]
    public string DisplaySecretKey { get; set; } = string.Empty;

    [JsonPropertyName("note")]
    public string? Note { get; set; }
}