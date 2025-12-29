using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Request to create a dataset item.
/// </summary>
public class CreateDatasetItemRequest
{
    /// <summary>
    ///     Name of the dataset to add the item to.
    /// </summary>
    [JsonPropertyName("datasetName")]
    public required string DatasetName { get; set; }

    /// <summary>
    ///     Input data for the dataset item.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    ///     Expected output for the dataset item.
    /// </summary>
    [JsonPropertyName("expectedOutput")]
    public object? ExpectedOutput { get; set; }

    /// <summary>
    ///     Additional metadata for the dataset item.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Source trace ID associated with this dataset item.
    /// </summary>
    [JsonPropertyName("sourceTraceId")]
    public string? SourceTraceId { get; set; }

    /// <summary>
    ///     Source observation ID associated with this dataset item.
    /// </summary>
    [JsonPropertyName("sourceObservationId")]
    public string? SourceObservationId { get; set; }

    /// <summary>
    ///     Dataset items are upserted on their id. Id needs to be unique (project-level) and cannot be reused across datasets.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     Status of the dataset item. Defaults to ACTIVE for newly created items.
    /// </summary>
    [JsonPropertyName("status")]
    public DatasetStatus? Status { get; set; }
}