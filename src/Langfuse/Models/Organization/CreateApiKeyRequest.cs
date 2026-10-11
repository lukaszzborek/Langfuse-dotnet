using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Request to create a new API key for a project.
/// </summary>
public class CreateApiKeyRequest
{
    /// <summary>
    ///     Optional name for the API key. Cannot be set together with <see cref="Note" />.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Deprecated alias for <see cref="Name" />. Cannot be set together with <see cref="Name" />.
    /// </summary>
    [Obsolete("Use Name instead.")]
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    ///     Optional expiration timestamp. Must be in the future. Null for a key that does not expire.
    /// </summary>
    [JsonPropertyName("expiresAt")]
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    ///     Optional predefined public key. If not provided, one will be generated.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public string? PublicKey { get; set; }

    /// <summary>
    ///     Optional predefined secret key. If not provided, one will be generated.
    /// </summary>
    [JsonPropertyName("secretKey")]
    public string? SecretKey { get; set; }
}