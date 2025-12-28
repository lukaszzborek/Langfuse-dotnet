using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Request to create a new API key for a project.
/// </summary>
public class CreateApiKeyRequest
{
    /// <summary>
    ///     Optional note for the API key to help identify its purpose.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

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