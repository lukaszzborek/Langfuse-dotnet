using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     One evaluator that can be used for scoring. Describes how to score data. It does not define which
///     live objects are evaluated; that is the job of evaluation rules. The concrete shape depends on
///     <see cref="Type" />: <see cref="LlmAsJudgeEvaluator" /> or <see cref="CodeEvaluator" />.
/// </summary>
[JsonConverter(typeof(EvaluatorConverter))]
public abstract class Evaluator
{
    /// <summary>
    ///     Identifier of this evaluator version.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Evaluator name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Version number of this evaluator.
    /// </summary>
    [JsonPropertyName("version")]
    public required int Version { get; init; }

    /// <summary>
    ///     Where this evaluator comes from: your project or Langfuse-managed defaults.
    /// </summary>
    [JsonPropertyName("scope")]
    public required EvaluatorScope Scope { get; init; }

    /// <summary>
    ///     Evaluator engine type. Determines the concrete evaluator shape.
    /// </summary>
    [JsonPropertyName("type")]
    public abstract EvaluatorType Type { get; }

    /// <summary>
    ///     Variables that can be mapped when creating an evaluation rule. LLM evaluators require every
    ///     variable to be mapped exactly once. Code evaluators always expose the fixed runtime payload
    ///     fields and Langfuse maps them automatically.
    /// </summary>
    [JsonPropertyName("variables")]
    public required string[] Variables { get; init; }

    /// <summary>
    ///     Number of evaluation rules in the project that currently use this evaluator version.
    /// </summary>
    [JsonPropertyName("evaluationRuleCount")]
    public required int EvaluationRuleCount { get; init; }

    /// <summary>
    ///     Timestamp when this evaluator was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Timestamp when this evaluator was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }
}