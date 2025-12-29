using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     SCIM service provider configuration.
/// </summary>
public class ScimServiceProviderConfig
{
    /// <summary>
    ///     SCIM schema URIs for this response.
    /// </summary>
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    /// <summary>
    ///     URI to the service provider's documentation.
    /// </summary>
    [JsonPropertyName("documentationUri")]
    public string DocumentationUri { get; set; } = string.Empty;

    /// <summary>
    ///     PATCH operation support configuration.
    /// </summary>
    [JsonPropertyName("patch")]
    public ScimSupported Patch { get; set; } = new();

    /// <summary>
    ///     Bulk operations support configuration.
    /// </summary>
    [JsonPropertyName("bulk")]
    public ScimBulkSupported Bulk { get; set; } = new();

    /// <summary>
    ///     Filter operations support configuration.
    /// </summary>
    [JsonPropertyName("filter")]
    public ScimFilterSupported Filter { get; set; } = new();

    /// <summary>
    ///     Password change support configuration.
    /// </summary>
    [JsonPropertyName("changePassword")]
    public ScimSupported ChangePassword { get; set; } = new();

    /// <summary>
    ///     Sort operations support configuration.
    /// </summary>
    [JsonPropertyName("sort")]
    public ScimSupported Sort { get; set; } = new();

    /// <summary>
    ///     ETag support configuration.
    /// </summary>
    [JsonPropertyName("etag")]
    public ScimSupported Etag { get; set; } = new();

    /// <summary>
    ///     List of supported authentication schemes.
    /// </summary>
    [JsonPropertyName("authenticationSchemes")]
    public List<ScimAuthenticationScheme> AuthenticationSchemes { get; set; } = new();

    /// <summary>
    ///     Metadata about the service provider config resource.
    /// </summary>
    [JsonPropertyName("meta")]
    public ScimResourceMeta Meta { get; set; } = new();
}