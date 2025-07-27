using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request for creating a new score
/// </summary>
public class ScoreCreateRequest
{
    /// <summary>
    ///     Optional custom ID for the score
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     ID of the trace this score belongs to (optional if observationId is provided)
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    ///     ID of the observation this score belongs to (optional if traceId is provided)
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     ID of the session this score belongs to (optional)
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    ///     ID of the dataset run this score belongs to
    /// </summary>
    [JsonPropertyName("datasetRunId")]
    public string? DatasetRunId { get; set; }

    /// <summary>
    ///     Name of the score (required)
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    ///     Value of the score (required)
    /// </summary>
    [JsonPropertyName("value")]
    public required object Value { get; set; } = null!;

    /// <summary>
    ///     Comment associated with the score (optional)
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    ///     Metadata associated with the score (optional)
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Comment associated with the score (optional)
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    /// <summary>
    ///     Data type of the score (optional, will be inferred if not provided)
    /// </summary>
    [JsonPropertyName("dataType")]
    public ScoreDataType? DataType { get; set; }

    /// <summary>
    ///     Configuration ID to use for this score (optional)
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }
}