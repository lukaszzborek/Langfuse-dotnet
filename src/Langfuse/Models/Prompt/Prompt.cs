using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Abstract base class for prompt templates with polymorphic type discrimination.
///     Supports both chat-based and text-based prompt formats for different AI model interfaces.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ChatPrompt), "chat")]
[JsonDerivedType(typeof(TextPrompt), "text")]
public abstract class PromptModel : BasePrompt
{
    /// <summary>
    ///     Type discriminator indicating the prompt format (chat or text).
    /// </summary>
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}