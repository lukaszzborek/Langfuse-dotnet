using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

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
    public List<string> Labels { get; set; } = new();

    /// <summary>
    ///     Tags for additional metadata and organization. Provides flexible categorization beyond labels.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

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