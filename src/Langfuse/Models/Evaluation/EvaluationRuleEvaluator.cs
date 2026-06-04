using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Resolved evaluator currently used by an evaluation rule. Id is the exact active evaluator version.
/// </summary>
public class EvaluationRuleEvaluator
{
    /// <summary>
    ///     Identifier of the exact evaluator version currently used by the rule.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

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
}
