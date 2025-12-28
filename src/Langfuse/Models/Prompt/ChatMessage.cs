using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Represents an actual chat message with a role and content, used in chat prompt templates.
/// </summary>
public class ChatMessage : ChatMessageWithPlaceholders
{
    /// <summary>
    ///     Type discriminator value for chat messages.
    /// </summary>
    public override string Type => "chatmessage";

    /// <summary>
    ///     Role of the message sender (e.g., "system", "user", "assistant"). Determines how the AI model interprets the
    ///     message.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    /// <summary>
    ///     Content of the message. Can include text and placeholder variables for dynamic substitution.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}