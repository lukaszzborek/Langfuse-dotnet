using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Schema definition for prompt templates, providing structure and validation rules for prompt variables.
/// </summary>
public class PromptSchema
{
    /// <summary>
    ///     The schema definition containing validation rules and structure for prompt template variables.
    /// </summary>
    [JsonPropertyName("schema")]
    public SchemaDefinition Schema { get; set; }
}