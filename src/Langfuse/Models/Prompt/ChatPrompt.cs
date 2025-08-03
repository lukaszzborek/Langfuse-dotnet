using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Chat-based prompt template for conversational AI models. Contains a sequence of messages with roles and content.
/// </summary>
public class ChatPrompt : PromptModel
{
    /// <summary>
    ///     Type discriminator value for chat prompts.
    /// </summary>
    public override string Type => "chat";

    /// <summary>
    ///     List of chat messages and placeholders that make up the conversation template.
    ///     Can include system messages, user messages, assistant messages, and placeholder variables.
    /// </summary>
    [JsonPropertyName("prompt")]
    public List<ChatMessageWithPlaceholders> PromptMessages { get; set; } = new();
}