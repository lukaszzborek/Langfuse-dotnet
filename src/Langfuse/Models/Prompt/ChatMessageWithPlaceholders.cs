using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Base class for chat message components that can be either actual messages or placeholder variables.
/// </summary>
[JsonConverter(typeof(ChatMessageConverter))]
public abstract class ChatMessageWithPlaceholders
{
    /// <summary>
    ///     Type discriminator indicating whether this is a chat message or placeholder.
    /// </summary>
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}