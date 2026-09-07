using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     One stored evaluator version. Concrete types: <see cref="LlmAsJudgeEvaluatorVersion" /> and
///     <see cref="CodeEvaluatorVersion" />.
/// </summary>
[JsonConverter(typeof(EvaluatorTypeConverter<EvaluatorVersion, LlmAsJudgeEvaluatorVersion, CodeEvaluatorVersion>))]
public abstract class EvaluatorVersion
{
    /// <summary>
    ///     Stable identifier of this evaluator version.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Monotonically increasing evaluator version number.
    /// </summary>
    [JsonPropertyName("version")]
    public required int Version { get; init; }

    /// <summary>
    ///     Evaluator engine type. Determined by the concrete class.
    /// </summary>
    [JsonPropertyName("type")]
    public abstract EvaluatorType Type { get; }

    /// <summary>
    ///     Timestamp when this evaluator version was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     User who created this version, or null when no user can be resolved.
    /// </summary>
    [JsonPropertyName("createdBy")]
    public Creator? CreatedBy { get; init; }
}