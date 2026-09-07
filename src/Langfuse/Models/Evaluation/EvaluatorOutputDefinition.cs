using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Structured output definition of an evaluator, discriminated by <see cref="DataType" />. Used both on
///     create/update requests and on evaluators/versions returned by the API. Concrete types:
///     <see cref="NumericEvaluatorOutputDefinition" />, <see cref="BooleanEvaluatorOutputDefinition" /> and
///     <see cref="CategoricalEvaluatorOutputDefinition" />.
/// </summary>
[JsonConverter(typeof(EvaluatorOutputDefinitionConverter))]
public abstract class EvaluatorOutputDefinition
{
    /// <summary>
    ///     Score type produced by the evaluator. Determined by the concrete class.
    /// </summary>
    [JsonPropertyName("dataType")]
    public abstract EvaluatorOutputScoreType DataType { get; }

    /// <summary>
    ///     Optional instructions for deriving the reasoning returned with the score.
    /// </summary>
    [JsonPropertyName("scoreReasoningInstructions")]
    public string? ScoreReasoningInstructions { get; init; }

    /// <summary>
    ///     Optional instructions for deriving the score value.
    /// </summary>
    [JsonPropertyName("scoreValueInstructions")]
    public string? ScoreValueInstructions { get; init; }
}

/// <summary>
///     Numeric output definition: the evaluator produces a numeric score such as 0.82.
/// </summary>
public class NumericEvaluatorOutputDefinition : EvaluatorOutputDefinition
{
    /// <inheritdoc />
    [JsonPropertyName("dataType")]
    public override EvaluatorOutputScoreType DataType => EvaluatorOutputScoreType.Numeric;

    /// <summary>
    ///     Optional inclusive minimum value. If both <see cref="MinValue" /> and <see cref="MaxValue" /> are set,
    ///     min must not exceed max.
    /// </summary>
    [JsonPropertyName("minValue")]
    public double? MinValue { get; init; }

    /// <summary>
    ///     Optional inclusive maximum value. If both <see cref="MinValue" /> and <see cref="MaxValue" /> are set,
    ///     min must not exceed max.
    /// </summary>
    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; init; }
}

/// <summary>
///     Boolean output definition: the evaluator produces a boolean score such as true.
/// </summary>
public class BooleanEvaluatorOutputDefinition : EvaluatorOutputDefinition
{
    /// <inheritdoc />
    [JsonPropertyName("dataType")]
    public override EvaluatorOutputScoreType DataType => EvaluatorOutputScoreType.Boolean;
}

/// <summary>
///     Categorical output definition: the evaluator produces one or more category labels from a fixed list.
/// </summary>
public class CategoricalEvaluatorOutputDefinition : EvaluatorOutputDefinition
{
    /// <inheritdoc />
    [JsonPropertyName("dataType")]
    public override EvaluatorOutputScoreType DataType => EvaluatorOutputScoreType.Categorical;

    /// <summary>
    ///     Allowed category values. At least two unique values are required.
    /// </summary>
    [JsonPropertyName("categories")]
    public required string[] Categories { get; init; }

    /// <summary>
    ///     Whether the evaluator may return more than one category.
    /// </summary>
    [JsonPropertyName("shouldAllowMultipleMatches")]
    public required bool ShouldAllowMultipleMatches { get; init; }
}