using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A score returned by the v3 scores API. Base of a discriminated union on <see cref="DataType" /> with the
///     concrete types <see cref="NumericScoreV3" />, <see cref="BooleanScoreV3" />, <see cref="CategoricalScoreV3" />,
///     <see cref="TextScoreV3" /> and <see cref="CorrectionScoreV3" />.
/// </summary>
/// <remarks>
///     The core fields (id, projectId, name, value, dataType, source, timestamp, environment, createdAt, updatedAt)
///     are always returned. The remaining fields are only present when the corresponding field group ("details",
///     "subject" or "annotation") is requested via <see cref="ScoreV3ListRequest.Fields" />.
/// </remarks>
[JsonConverter(typeof(ScoreV3Converter))]
public abstract class ScoreV3
{
    /// <summary>
    ///     Data type discriminator of the score (NUMERIC, BOOLEAN, CATEGORICAL, TEXT or CORRECTION)
    /// </summary>
    [JsonPropertyName("dataType")]
    public abstract ScoreV3DataType DataType { get; }

    /// <summary>
    ///     Unique identifier of the score
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Identifier of the project the score belongs to
    /// </summary>
    [JsonPropertyName("projectId")]
    public required string ProjectId { get; init; }

    /// <summary>
    ///     Name of the score (e.g., "quality", "relevance")
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Source indicating how the score was created (API, ANNOTATION, EVAL)
    /// </summary>
    [JsonPropertyName("source")]
    public required ScoreSource Source { get; init; }

    /// <summary>
    ///     Timestamp of the score
    /// </summary>
    [JsonPropertyName("timestamp")]
    public required DateTime Timestamp { get; init; }

    /// <summary>
    ///     The environment from which this score originated
    /// </summary>
    [JsonPropertyName("environment")]
    public required string Environment { get; init; }

    /// <summary>
    ///     Timestamp when the score was created in Langfuse
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Timestamp when the score was last updated
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }

    /// <summary>
    ///     Optional comment attached to the score. Present when "details" is included in the fields parameter.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; init; }

    /// <summary>
    ///     The score config ID, if this score was created from a config. Present when "details" is included in the
    ///     fields parameter.
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; init; }

    /// <summary>
    ///     Arbitrary metadata attached to the score. Present when "details" is included in the fields parameter.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }

    /// <summary>
    ///     The user who created this score, if available. Present when "annotation" is included in the fields
    ///     parameter.
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; init; }

    /// <summary>
    ///     The annotation queue this score belongs to, if any. Present when "annotation" is included in the fields
    ///     parameter.
    /// </summary>
    [JsonPropertyName("queueId")]
    public string? QueueId { get; init; }

    /// <summary>
    ///     The entity this score is attached to (trace, observation, session or experiment). Present when "subject"
    ///     is included in the fields parameter.
    /// </summary>
    [JsonPropertyName("subject")]
    public ScoreSubjectV3? Subject { get; init; }
}