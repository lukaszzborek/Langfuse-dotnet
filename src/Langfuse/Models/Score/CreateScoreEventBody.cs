using System.Text.Json.Serialization;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Objects for storing evaluation metrics in Langfuse
/// </summary>
public class CreateScoreEventBody
{
    /// <summary>
    ///     Langfuse trace object
    /// </summary>
    [JsonIgnore]
    public LangfuseTrace? LangfuseTrace { get; init; }

    /// <summary>
    ///     Unique identifier of the score
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Id of the trace the score relates to
    /// </summary>
    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the score, e.g. user_feedback, hallucination_eval
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    ///     Optional: Numeric value of the score. Always defined for numeric and boolean scores. Optional for categorical
    ///     scores
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    ///     Optional: Observation (e.g. LLM call) the score relates to
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     Optional: Evaluation comment, commonly used for user feedback, eval output or internal notes
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    ///     Automatically set based on the config data type when the <see cref="ConfigId" /> is provided. Otherwise can be
    ///     defined manually as NUMERIC, CATEGORICAL or BOOLEAN
    /// </summary>
    [JsonPropertyName("dataType")]
    public ScoreDataType DataType { get; set; }

    /// <summary>
    ///     Optional: Score config id to ensure that the score follows a specific schema. Can be defined in the Langfuse UI or
    ///     via API. When provided the score's <see cref="DataType" /> is automatically set based on the config
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }

    /// <summary>
    ///     Optional: ID of the session this score belongs to. Used for session-level aggregated evaluations.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    ///     Optional: ID of the dataset run this score belongs to. Used when scoring dataset evaluations and experiments.
    /// </summary>
    [JsonPropertyName("datasetRunId")]
    public string? DatasetRunId { get; set; }

    /// <summary>
    ///     Optional: Additional metadata associated with the score as a JSON object.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Optional: The environment of the score (e.g., production, staging, development).
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    /// <summary>
    ///     Optional: ID of the annotation queue referenced by the score.
    /// </summary>
    [JsonPropertyName("queueId")]
    public string? QueueId { get; set; }
}