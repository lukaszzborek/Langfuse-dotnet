using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Represents an API key in the organization
/// </summary>
public record OrganizationApiKey
{
    /// <summary>
    ///     Unique identifier for the API key
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Timestamp when the API key was created
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Timestamp when the API key expires (null if no expiration)
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; init; }

    /// <summary>
    ///     Timestamp when the API key was last used
    /// </summary>
    [JsonPropertyName("lastUsedAt")]
    public DateTime? LastUsedAt { get; init; }

    /// <summary>
    ///     Optional note describing the API key's purpose
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; init; }

    /// <summary>
    ///     The public key identifier
    /// </summary>
    [JsonPropertyName("publicKey")]
    public required string PublicKey { get; init; }

    /// <summary>
    ///     Masked version of the secret key for display purposes
    /// </summary>
    [JsonPropertyName("displaySecretKey")]
    public required string DisplaySecretKey { get; init; }
}
