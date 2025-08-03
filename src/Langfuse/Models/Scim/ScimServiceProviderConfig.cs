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

public class ScimSupported
{
    [JsonPropertyName("supported")]
    public bool Supported { get; set; }
}

public class ScimBulkSupported : ScimSupported
{
    [JsonPropertyName("maxOperations")]
    public int MaxOperations { get; set; }

    [JsonPropertyName("maxPayloadSize")]
    public int MaxPayloadSize { get; set; }
}

public class ScimFilterSupported : ScimSupported
{
    [JsonPropertyName("maxResults")]
    public int? MaxResults { get; set; }
}

public class ScimResourceMeta
{
    [JsonPropertyName("resourceType")]
    public string ResourceType { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string? Location { get; set; }
}

public class ScimAuthenticationScheme
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("specUri")]
    public string? SpecUri { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}