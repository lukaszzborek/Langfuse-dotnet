using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     One evaluator that can be used for scoring. An evaluator describes how to score data; associated
///     evaluation rules describe which live objects should be evaluated. The latest definition and version
///     metadata are flattened into this object. Concrete types: <see cref="LlmAsJudgeEvaluator" /> and
///     <see cref="CodeEvaluator" />.
/// </summary>
[JsonConverter(typeof(EvaluatorTypeConverter<Evaluator, LlmAsJudgeEvaluator, CodeEvaluator>))]
public abstract class Evaluator
{
    /// <summary>
    ///     Stable identifier of this evaluator across all versions.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Human-readable evaluator name. Names are not identifiers and do not need to be unique.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Optional human-readable evaluator description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    ///     Evaluator engine type. Determined by the concrete class.
    /// </summary>
    [JsonPropertyName("type")]
    public abstract EvaluatorType Type { get; }

    /// <summary>
    ///     User who created this evaluator, or null when no user can be resolved.
    /// </summary>
    [JsonPropertyName("createdBy")]
    public Creator? CreatedBy { get; init; }

    /// <summary>
    ///     Effective evaluator status after Langfuse validates its runtime configuration.
    /// </summary>
    [JsonPropertyName("status")]
    public required EvaluatorStatus Status { get; init; }

    /// <summary>
    ///     Timestamp when the evaluator was paused, otherwise null.
    /// </summary>
    [JsonPropertyName("pausedAt")]
    public DateTime? PausedAt { get; init; }

    /// <summary>
    ///     Machine-readable reason when <see cref="Status" /> is paused, otherwise null.
    /// </summary>
    [JsonPropertyName("pausedReason")]
    public string? PausedReason { get; init; }

    /// <summary>
    ///     Human-readable explanation when <see cref="Status" /> is paused, otherwise null.
    /// </summary>
    [JsonPropertyName("pausedMessage")]
    public string? PausedMessage { get; init; }

    /// <summary>
    ///     All modern and legacy evaluation-rule assignments in newest-assignment-first order.
    /// </summary>
    [JsonPropertyName("evaluationRuleAssignments")]
    public required EvaluationRuleAssignment[] EvaluationRuleAssignments { get; init; }

    /// <summary>
    ///     Timestamp when the evaluator was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Timestamp when the evaluator was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }

    /// <summary>
    ///     Stable identifier of the latest evaluator version.
    /// </summary>
    [JsonPropertyName("versionId")]
    public required string VersionId { get; init; }

    /// <summary>
    ///     Monotonically increasing latest evaluator version number.
    /// </summary>
    [JsonPropertyName("version")]
    public required int Version { get; init; }

    /// <summary>
    ///     Timestamp when the latest evaluator version was created.
    /// </summary>
    [JsonPropertyName("versionCreatedAt")]
    public required DateTime VersionCreatedAt { get; init; }

    /// <summary>
    ///     User who created the latest evaluator version, or null when no user can be resolved.
    /// </summary>
    [JsonPropertyName("versionCreatedBy")]
    public Creator? VersionCreatedBy { get; init; }
}