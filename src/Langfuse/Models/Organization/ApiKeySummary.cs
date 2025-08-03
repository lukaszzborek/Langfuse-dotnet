using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Summary information about an API key used for authentication with Langfuse services.
///     API keys provide programmatic access to projects and enable trace ingestion and data retrieval.
/// </summary>
public class ApiKeySummary
{
    /// <summary>
    ///     Unique identifier of the API key.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the API key was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Optional expiration timestamp for the API key. If null, the key does not expire.
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    ///     Timestamp when the API key was last used for authentication. Useful for monitoring usage and security.
    /// </summary>
    [JsonPropertyName("lastUsedAt")]
    public DateTime? LastUsedAt { get; set; }

    /// <summary>
    ///     Optional human-readable note or description for the API key, helping identify its purpose.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    ///     Public portion of the API key, safe to display in UI and logs for identification purposes.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    ///     Masked/display version of the secret key showing only partial characters for security. Never contains the full
    ///     secret.
    /// </summary>
    [JsonPropertyName("displaySecretKey")]
    public string DisplaySecretKey { get; set; } = string.Empty;
}