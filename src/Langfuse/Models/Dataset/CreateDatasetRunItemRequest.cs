using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Request to create a dataset run item.
/// </summary>
public class CreateDatasetRunItemRequest
{
    /// <summary>
    ///     Name of the dataset run.
    /// </summary>
    [JsonPropertyName("runName")]
    public string RunName { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the run. If run exists, description will be updated.
    /// </summary>
    [JsonPropertyName("runDescription")]
    public string? RunDescription { get; set; }

    /// <summary>
    ///     Metadata of the dataset run, updates run if run already exists.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     ID of the dataset item to link to this run item.
    /// </summary>
    [JsonPropertyName("datasetItemId")]
    public required string DatasetItemId { get; set; }

    /// <summary>
    ///     Observation ID associated with this run item.
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     Trace ID should always be provided. For compatibility with older SDK versions it can also be inferred from the
    ///     provided observationId.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
}