using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Base class for creating prompt templates in Langfuse. Prompts enable reusable, versioned templates for AI model interactions.
/// </summary>
[JsonDerivedType(typeof(CreateTextPromptRequest))]
[JsonDerivedType(typeof(CreateChatPromptRequest))]
public abstract class CreatePromptRequest
{
    /// <summary>
    ///     Type of prompt template being created (Text or Chat). Determines the structure and usage pattern.
    /// </summary>
    [JsonPropertyName("type")]
    public PromptType? Type { get; set; }
    
    /// <summary>
    ///     Name of the prompt template, used for identification and retrieval. Must be unique within the project.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    ///     Optional configuration object containing model parameters, settings, and metadata for the prompt template.
    /// </summary>
    [JsonPropertyName("config")]
    public object? Config { get; set; }

    /// <summary>
    ///     Labels for categorizing and organizing prompt templates. Used for filtering and management in the UI.
    /// </summary>
    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = [];

    /// <summary>
    ///     Tags for additional metadata and organization. Provides flexible categorization and searchability.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = [];

    /// <summary>
    ///     Optional commit message describing the purpose or changes in this version of the prompt template.
    /// </summary>
    [JsonPropertyName("commitMessage")]
    public string? CommitMessage { get; set; }
}

/// <summary>
///     Request to create a text-based prompt template. Suitable for completion-style AI models that expect a single text input.
/// </summary>
public class CreateTextPromptRequest : CreatePromptRequest
{
    /// <summary>
    ///     The text content of the prompt template. Can include placeholder variables (e.g., {{variable_name}}) for dynamic substitution.
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

/// <summary>
///     Request to create a chat-based prompt template. Suitable for conversational AI models that expect structured message sequences.
/// </summary>
public class CreateChatPromptRequest : CreatePromptRequest
{
    /// <summary>
    ///     Array of chat messages and placeholders that define the conversation template.
    ///     Can include <see cref="ChatMessage"/> instances for actual messages or <see cref="PlaceholderMessage"/> instances for dynamic content.
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

/// <summary>
///     Defines the type of prompt template, determining its structure and intended AI model interface.
/// </summary>
public enum PromptType
{
    /// <summary>
    ///     Text-based prompt for completion-style models. Contains a single text string with optional placeholders.
    /// </summary>
    Text,
    
    /// <summary>
    ///     Chat-based prompt for conversational models. Contains structured message sequences with roles and content.
    /// </summary>
    Chat
}