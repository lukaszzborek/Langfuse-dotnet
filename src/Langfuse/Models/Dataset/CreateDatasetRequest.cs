using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

public class CreateDatasetRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

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