using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Represents a score in Langfuse - a quantitative or qualitative evaluation measurement attached to traces,
///     observations, or sessions.
///     Scores can be numeric values, categorical labels, or boolean judgments used for quality assessment and analytics.
/// </summary>
public class ScoreModel
{
    /// <summary>
    ///     Unique identifier of the score
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the trace this score belongs to. Used for trace-level evaluations and quality metrics.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    ///     ID of the session this score belongs to. Used for session-level evaluations across multiple traces.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    ///     ID of the observation this score belongs to. Used for evaluating specific spans, generations, or events.
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     Name of the score that identifies the evaluation metric (e.g., "quality", "relevance", "toxicity").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     The actual value of the score. For numeric scores this is a number, for categorical/boolean it may be the numeric
    ///     mapping.
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    ///     String representation of the score value. For categorical scores, this is the category label. For boolean scores,
    ///     "True" or "False".
    /// </summary>
    [JsonPropertyName("stringValue")]
    public string? StringValue { get; set; }

    /// <summary>
    ///     Data type of the score indicating how the value should be interpreted (NUMERIC, CATEGORICAL, BOOLEAN).
    /// </summary>
    [JsonPropertyName("dataType")]
    public ScoreDataType DataType { get; set; }

    /// <summary>
    ///     Source indicating how this score was created (API, manual annotation, automated evaluation, etc.).
    /// </summary>
    [JsonPropertyName("source")]
    public ScoreSource? Source { get; set; }

    /// <summary>
    ///     Optional comment providing additional context or explanation for the score value.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    ///     Timestamp when the score was created
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    ///     ID of the user who created this score, useful for tracking manual annotations and audit trails.
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; set; }

    /// <summary>
    ///     ID of the score configuration that defines validation rules, categories, and ranges for this score type.
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }

    /// <summary>
    ///     Timestamp when the score was first created in the Langfuse system.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the score was last updated or modified.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     Additional metadata associated with the score as a JSON object, containing custom properties and context.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     ID of the dataset run this score belongs to. Used when scoring dataset evaluations and experiments.
    /// </summary>
    [JsonPropertyName("datasetRunId")]
    public string? DatasetRunId { get; set; }

    /// <summary>
    ///     ID of the annotation queue referenced by the score. Indicates if score was initially created while processing
    ///     annotation queue.
    /// </summary>
    [JsonPropertyName("queueId")]
    public string? QueueId { get; set; }

    /// <summary>
    ///     The environment of the score (e.g., production, staging, development).
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }
}