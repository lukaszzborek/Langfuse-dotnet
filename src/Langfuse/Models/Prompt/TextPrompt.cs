using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Text-based prompt template for completion-style AI models. Contains a single text string with optional
///     placeholders.
/// </summary>
public class TextPrompt : PromptModel
{
    /// <summary>
    ///     Type discriminator value for text prompts.
    /// </summary>
    public override string Type => "text";

    /// <summary>
    ///     The text content of the prompt template, which may include placeholder variables for dynamic substitution.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string PromptText { get; set; } = string.Empty;
}