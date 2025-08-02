using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class ScimSchema
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public List<ScimAttribute> Attributes { get; set; } = new();
}

public class ScimAttribute
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("multiValued")]
    public bool MultiValued { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("required")]
    public bool Required { get; set; }

    [JsonPropertyName("caseExact")]
    public bool CaseExact { get; set; }

    [JsonPropertyName("mutability")]
    public string Mutability { get; set; } = string.Empty;

    [JsonPropertyName("returned")]
    public string Returned { get; set; } = string.Empty;

    [JsonPropertyName("uniqueness")]
    public string Uniqueness { get; set; } = string.Empty;

    [JsonPropertyName("subAttributes")]
    public List<ScimAttribute>? SubAttributes { get; set; }
}