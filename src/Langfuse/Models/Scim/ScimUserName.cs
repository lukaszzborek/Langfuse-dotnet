using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimUserName
{
    [JsonPropertyName("formatted")]
    public string? Formatted { get; set; }
}