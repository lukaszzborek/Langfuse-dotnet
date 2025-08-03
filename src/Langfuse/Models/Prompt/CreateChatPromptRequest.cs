using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Request to create a chat-based prompt template. Suitable for conversational AI models that expect structured
///     message sequences.
/// </summary>
public class CreateChatPromptRequest : CreatePromptRequest
{
    /// <summary>
    ///     Array of chat messages and placeholders that define the conversation template.
    ///     Can include <see cref="ChatMessage" /> instances for actual messages or <see cref="PlaceholderMessage" /> instances
    ///     for dynamic content.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required ChatMessageWithPlaceholders[] Prompt { get; set; }

    /// <summary>
    ///     Initializes a new chat prompt request with the appropriate type.
    /// </summary>
    public CreateChatPromptRequest()
    {
        Type = PromptType.Chat;
    }
}