using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Request to create a new dataset.
/// </summary>
public class CreateDatasetRequest
{
    /// <summary>
    ///     Name of the dataset.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the dataset.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Additional metadata for the dataset.
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
}