using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Represents a single item within a dataset - an individual test case consisting of input data and expected output.
///     Dataset items serve as the ground truth examples for evaluation and testing of LLM applications.
/// </summary>
public class DatasetItem
{
    /// <summary>
    ///     Unique identifier of the dataset item.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Status of the dataset item indicating whether it's active or archived.
    /// </summary>
    [JsonPropertyName("status")]
    public DatasetStatus Status { get; set; }

    /// <summary>
    ///     Input data for this test case. Can be any JSON object representing the request or parameters to test.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    ///     Expected output data for this test case. Can be any JSON object representing the ideal response or result.
    /// </summary>
    [JsonPropertyName("expectedOutput")]
    public object? ExpectedOutput { get; set; }

    /// <summary>
    ///     Additional metadata associated with this dataset item as a JSON object, containing custom properties and context.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     ID of the original trace this dataset item was derived from, if applicable. Used for traceability and provenance.
    /// </summary>
    [JsonPropertyName("sourceTraceId")]
    public string? SourceTraceId { get; set; }

    /// <summary>
    ///     ID of the original observation this dataset item was derived from, if applicable. Provides more specific provenance
    ///     information.
    /// </summary>
    [JsonPropertyName("sourceObservationId")]
    public string? SourceObservationId { get; set; }

    /// <summary>
    ///     ID of the dataset this item belongs to, establishing the parent-child relationship.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the dataset this item belongs to, for display and organizational purposes.
    /// </summary>
    [JsonPropertyName("datasetName")]
    public string DatasetName { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the dataset item was first created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the dataset item was last updated or modified.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
///     Represents the status of a dataset item, indicating its lifecycle state.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<DatasetStatus>))]
public enum DatasetStatus
{
    /// <summary>
    ///     The dataset item is active and available for use in evaluations and experiments.
    /// </summary>
    Active,

    /// <summary>
    ///     The dataset item is archived and no longer actively used, but preserved for historical reference.
    /// </summary>
    Archived
}