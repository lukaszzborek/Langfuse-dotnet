using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     LLM-as-a-judge evaluator family reference used when creating an evaluation rule.
/// </summary>
public class LlmAsJudgeEvaluationRuleEvaluatorReference
{
    /// <summary>
    ///     Evaluator family name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Whether the evaluator family is project-owned or Langfuse-managed.
    /// </summary>
    [JsonPropertyName("scope")]
    public required EvaluatorScope Scope { get; init; }

    /// <summary>
    ///     Evaluator engine type. Always llm_as_judge.
    /// </summary>
    [JsonPropertyName("type")]
    public EvaluatorType Type => EvaluatorType.Llm_As_Judge;
}