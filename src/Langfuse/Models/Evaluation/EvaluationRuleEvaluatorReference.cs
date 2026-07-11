using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Evaluator family reference used when updating an evaluation rule. A rule's evaluator type cannot
///     be changed, so this reference does not accept a type; the family must match the rule's current
///     evaluator type.
/// </summary>
public class EvaluationRuleEvaluatorReference
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
}