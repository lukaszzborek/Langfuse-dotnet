using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Response returned after creating an API key.
/// </summary>
public class ApiKeyResponse
{
    /// <summary>
    ///     Unique identifier for the API key.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the API key was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     The public key portion of the API key.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    ///     The secret key portion of the API key. Only returned once on creation.
    /// </summary>
    [JsonPropertyName("secretKey")]
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    ///     Masked version of the secret key for display purposes.
    /// </summary>
    [JsonPropertyName("displaySecretKey")]
    public string DisplaySecretKey { get; set; } = string.Empty;

    /// <summary>
    ///     Optional note describing the API key.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}