using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Defines the structure and validation rules for prompt template variables and parameters.
/// </summary>
public class SchemaDefinition
{
    /// <summary>
    ///     Title or name of the schema definition, used for identification and documentation purposes.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}