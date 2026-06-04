using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     One evaluator that can be used for scoring. Describes how to score data: prompt, extracted variables,
///     output schema, and optional explicit model configuration. It does not define which live objects are
///     evaluated; that is the job of evaluation rules.
/// </summary>
public class Evaluator
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
    ///     Evaluator engine type. Currently always llm_as_judge.
    /// </summary>
    [JsonPropertyName("type")]
    public required EvaluatorType Type { get; init; }

    /// <summary>
    ///     Prompt template used during evaluation.
    /// </summary>
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    /// <summary>
    ///     Variables extracted from the evaluator prompt. Every variable must be mapped exactly once when
    ///     creating an evaluation rule.
    /// </summary>
    [JsonPropertyName("variables")]
    public required string[] Variables { get; init; }

    /// <summary>
    ///     Structured output schema returned by this evaluator.
    /// </summary>
    [JsonPropertyName("outputDefinition")]
    public required EvaluatorOutputDefinition OutputDefinition { get; init; }

    /// <summary>
    ///     Explicit model configuration, or null when the project default evaluation model is used.
    /// </summary>
    [JsonPropertyName("modelConfig")]
    public EvaluatorModelConfig? ModelConfig { get; init; }

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
