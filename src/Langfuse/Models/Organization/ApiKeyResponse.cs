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
    ///     Expiration timestamp. Null if the key does not expire.
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; set; }

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
    ///     Name of the API key. Null if no name was provided.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Deprecated alias for <see cref="Name" />. Contains the same value.
    /// </summary>
    [Obsolete("Use Name instead.")]
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}