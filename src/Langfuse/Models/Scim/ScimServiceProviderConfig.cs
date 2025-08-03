using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimServiceProviderConfig
{
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    [JsonPropertyName("documentationUri")]
    public string? DocumentationUri { get; set; }

    [JsonPropertyName("patch")]
    public ScimSupported Patch { get; set; } = new();

    [JsonPropertyName("bulk")]
    public ScimBulkSupported Bulk { get; set; } = new();

    [JsonPropertyName("filter")]
    public ScimFilterSupported Filter { get; set; } = new();

    [JsonPropertyName("changePassword")]
    public ScimSupported ChangePassword { get; set; } = new();

    [JsonPropertyName("sort")]
    public ScimSupported Sort { get; set; } = new();

    [JsonPropertyName("etag")]
    public ScimSupported Etag { get; set; } = new();

    [JsonPropertyName("authenticationSchemes")]
    public List<ScimAuthenticationScheme> AuthenticationSchemes { get; set; } = new();

    [JsonPropertyName("meta")]
    public ScimResourceMeta? Meta { get; set; }
}