using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Base class for creating prompt templates in Langfuse. Prompts enable reusable, versioned templates for AI model
///     interactions.
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