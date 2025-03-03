using System.Text.Json.Serialization;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Models;

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
    ///     via API. When provided the score’s <see cref="DataType" /> is automatically set based on the config
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }
}