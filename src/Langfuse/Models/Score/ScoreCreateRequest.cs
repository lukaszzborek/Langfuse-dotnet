using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Request for creating a new score in Langfuse. Scores are evaluation measurements that can be attached to traces,
///     observations, or sessions.
/// </summary>
public class ScoreCreateRequest
{
    /// <summary>
    ///     Optional custom ID for the score. If not provided, a unique ID will be generated automatically.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     ID of the trace this score belongs to. Required unless observationId is provided. Used for trace-level evaluations.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    ///     ID of the observation this score belongs to. Required unless traceId is provided. Used for observation-level
    ///     evaluations.
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     ID of the session this score belongs to. Optional, used for session-level aggregated evaluations.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    ///     ID of the dataset run this score belongs to. Used when scoring dataset evaluations and experiments.
    /// </summary>
    [JsonPropertyName("datasetRunId")]
    public string? DatasetRunId { get; set; }

    /// <summary>
    ///     Name identifying the type of score (e.g., "quality", "relevance", "toxicity"). Must match a score configuration if
    ///     one exists.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    ///     The score value. Can be a number (for numeric scores), string (for categorical), or boolean. Type should match the
    ///     data type configuration.
    /// </summary>
    [JsonPropertyName("value")]
    public required object Value { get; set; } = null!;

    /// <summary>
    ///     Optional comment providing additional context or explanation for the score value.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    ///     Optional metadata associated with the score as a JSON object, containing custom properties and context.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Environment from which this score originated. Helps categorize scores by deployment environment.
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    /// <summary>
    ///     Data type of the score (NUMERIC, CATEGORICAL, BOOLEAN). Will be inferred from the value if not provided.
    /// </summary>
    [JsonPropertyName("dataType")]
    public ScoreDataType? DataType { get; set; }

    /// <summary>
    ///     ID of the score configuration to validate this score against. Ensures the score follows predefined rules and
    ///     categories.
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }

    /// <summary>
    ///     ID of the annotation queue referenced by the score. Indicates if score was initially created while processing
    ///     annotation queue.
    /// </summary>
    [JsonPropertyName("queueId")]
    public string? QueueId { get; set; }
}