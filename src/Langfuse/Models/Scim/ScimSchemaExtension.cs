using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Represents a SCIM schema extension reference.
/// </summary>
public class ScimSchemaExtension
{
    /// <summary>
    ///     URI of the schema extension.
    /// </summary>
    [JsonPropertyName("schema")]
    public string Schema { get; set; } = string.Empty;

    /// <summary>
    ///     Indicates if this extension is required.
    /// </summary>
    [JsonPropertyName("required")]
    public bool Required { get; set; }
}