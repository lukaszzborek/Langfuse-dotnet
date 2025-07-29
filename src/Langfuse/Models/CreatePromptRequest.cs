using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Base class for creating prompts.
/// </summary>
[JsonDerivedType(typeof(CreateTextPromptRequest))]
[JsonDerivedType(typeof(CreateChatPromptRequest))]
public abstract class CreatePromptRequest
{
    [JsonPropertyName("type")]
    public PromptType? Type { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("config")]
    public object? Config { get; set; }

    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = [];

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    [JsonPropertyName("commitMessage")]
    public string? CommitMessage { get; set; }
}

/// <summary>
///     Prompt for text input.
/// </summary>
public class CreateTextPromptRequest : CreatePromptRequest
{
    [JsonPropertyName("prompt")]
    public required string Prompt { get; set; }

    public CreateTextPromptRequest()
    {
        Type = PromptType.Text;
    }
}

/// <summary>
///     Prompt for chat messages.
/// </summary>
public class CreateChatPromptRequest : CreatePromptRequest
{
    /// <summary>
    /// Field with chat messages and placeholders.
    /// It might be <see cref="PlaceholderMessage"/> or <see cref="ChatMessageWithPlaceholders"/>.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required ChatMessageWithPlaceholders[] Prompt { get; set; }

    public CreateChatPromptRequest()
    {
        Type = PromptType.Chat;
    }
}

public enum PromptType
{
    Text,
    Chat
}