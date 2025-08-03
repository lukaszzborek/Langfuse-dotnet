using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Request to create a text-based prompt template. Suitable for completion-style AI models that expect a single text
///     input.
/// </summary>
public class CreateTextPromptRequest : CreatePromptRequest
{
    /// <summary>
    ///     The text content of the prompt template. Can include placeholder variables (e.g., {{variable_name}}) for dynamic
    ///     substitution.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; set; }

    /// <summary>
    ///     Initializes a new text prompt request with the appropriate type.
    /// </summary>
    public CreateTextPromptRequest()
    {
        Type = PromptType.Text;
    }
}