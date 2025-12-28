using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Abstract base class for prompt templates with polymorphic type discrimination.
///     Supports both chat-based and text-based prompt formats for different AI model interfaces.
/// </summary>
[JsonConverter(typeof(PromptModelConverter))]
public abstract class PromptModel : BasePrompt
{
    /// <summary>
    ///     Type discriminator indicating the prompt format (chat or text).
    /// </summary>
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}