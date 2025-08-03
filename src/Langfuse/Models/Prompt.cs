using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
///     Base class for prompt templates in Langfuse, containing common properties shared across all prompt types.
/// </summary>
public abstract class BasePrompt
{
    /// <summary>
    ///     Name of the prompt template, used for identification and retrieval.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Version number of the prompt template. Enables versioning and A/B testing of different prompt iterations.
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>
    ///     Configuration object containing model parameters, settings, and other metadata for the prompt.
    /// </summary>
    [JsonPropertyName("config")]
    public object Config { get; set; }

    /// <summary>
    ///     Labels for categorizing and organizing prompts. Used for filtering and management in the UI.
    /// </summary>
    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = new List<string>();

    /// <summary>
    ///     Tags for additional metadata and organization. Provides flexible categorization beyond labels.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();

    /// <summary>
    ///     Optional commit message describing changes made in this version of the prompt template.
    /// </summary>
    [JsonPropertyName("commitMessage")]
    public string? CommitMessage { get; set; }

    /// <summary>
    ///     Graph structure for resolving placeholder variables and dependencies within the prompt template.
    /// </summary>
    [JsonPropertyName("resolutionGraph")]
    public Dictionary<string, object>? ResolutionGraph { get; set; }
}

/// <summary>
///     Abstract base class for prompt templates with polymorphic type discrimination.
///     Supports both chat-based and text-based prompt formats for different AI model interfaces.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ChatPrompt), "chat")]
[JsonDerivedType(typeof(TextPrompt), "text")]
public abstract class Prompt : BasePrompt
{
    /// <summary>
    ///     Type discriminator indicating the prompt format (chat or text).
    /// </summary>
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}

/// <summary>
///     Chat-based prompt template for conversational AI models. Contains a sequence of messages with roles and content.
/// </summary>
public class ChatPrompt : Prompt
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
    public List<ChatMessageWithPlaceholders> PromptMessages { get; set; } = new List<ChatMessageWithPlaceholders>();
}

/// <summary>
///     Text-based prompt template for completion-style AI models. Contains a single text string with optional placeholders.
/// </summary>
public class TextPrompt : Prompt
{
    /// <summary>
    ///     Type discriminator value for text prompts.
    /// </summary>
    public override string Type => "text";

    /// <summary>
    ///     The text content of the prompt template, which may include placeholder variables for dynamic substitution.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string PromptText { get; set; }
}


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
    ///     Role of the message sender (e.g., "system", "user", "assistant"). Determines how the AI model interprets the message.
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; }

    /// <summary>
    ///     Content of the message. Can include text and placeholder variables for dynamic substitution.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; }
}

/// <summary>
///     Represents a placeholder variable in a chat prompt template that will be replaced with actual values at runtime.
/// </summary>
public class PlaceholderMessage : ChatMessageWithPlaceholders
{
    /// <summary>
    ///     Type discriminator value for placeholder messages.
    /// </summary>
    public override string Type => "placeholder";

    /// <summary>
    ///     Name of the placeholder variable. Used to identify which value should be substituted during prompt resolution.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

/// <summary>
///     Schema definition for prompt templates, providing structure and validation rules for prompt variables.
/// </summary>
public class PromptSchema
{
    /// <summary>
    ///     The schema definition containing validation rules and structure for prompt template variables.
    /// </summary>
    [JsonPropertyName("schema")]
    public SchemaDefinition Schema { get; set; }
}

/// <summary>
///     Defines the structure and validation rules for prompt template variables and parameters.
/// </summary>
public class SchemaDefinition
{
    /// <summary>
    ///     Title or name of the schema definition, used for identification and documentation purposes.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }
}
