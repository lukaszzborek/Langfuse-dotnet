using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Paginated response containing dataset runs.
/// </summary>
public class PaginatedDatasetRuns : PaginatedResponse<DatasetRun>
{
}

/// <summary>
///     Represents a dataset run.
/// </summary>
public class DatasetRun
{
    /// <summary>
    ///     Unique identifier of the dataset run.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the dataset run.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the run.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Metadata of the dataset run.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     ID of the associated dataset.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the associated dataset.
    /// </summary>
    [JsonPropertyName("datasetName")]
    public string DatasetName { get; set; } = string.Empty;

    /// <summary>
    ///     The date and time when the dataset run was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     The date and time when the dataset run was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}