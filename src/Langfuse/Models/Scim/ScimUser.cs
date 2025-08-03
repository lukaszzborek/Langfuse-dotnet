using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimUser
{
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public ScimUserName? Name { get; set; }

    [JsonPropertyName("emails")]
    public List<ScimUserEmail> Emails { get; set; } = new();

    [JsonPropertyName("meta")]
    public ScimMeta? Meta { get; set; }
}

public class ScimUserName
{
    [JsonPropertyName("formatted")]
    public string? Formatted { get; set; }
}

public class ScimUserEmail
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}

public class ScimMeta
{
    [JsonPropertyName("resourceType")]
    public string ResourceType { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonPropertyName("lastModified")]
    public string? LastModified { get; set; }
}