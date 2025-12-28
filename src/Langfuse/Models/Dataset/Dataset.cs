using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Represents a dataset in Langfuse - a collection of input/output examples used for evaluation, testing, and
///     experimentation.
///     Datasets enable systematic testing of LLM applications against known examples and expected outcomes.
/// </summary>
public class DatasetModel
{
    /// <summary>
    ///     Unique identifier of the dataset.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the dataset, used for identification and organization in the UI.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Optional description explaining the purpose and contents of the dataset.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     ID of the project this dataset belongs to, providing organizational scope.
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    ///     Additional metadata associated with the dataset as a JSON object, containing custom properties and configuration.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     JSON Schema for validating dataset item inputs. When set, all new and existing dataset items will be validated
    ///     against this schema.
    /// </summary>
    [JsonPropertyName("inputSchema")]
    public object? InputSchema { get; set; }

    /// <summary>
    ///     JSON Schema for validating dataset item expected outputs. When set, all new and existing dataset items will be
    ///     validated against this schema.
    /// </summary>
    [JsonPropertyName("expectedOutputSchema")]
    public object? ExpectedOutputSchema { get; set; }

    /// <summary>
    ///     Timestamp when the dataset was first created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the dataset was last updated or modified.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}