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