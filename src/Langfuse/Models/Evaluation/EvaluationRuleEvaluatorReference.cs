using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Evaluator family reference used when creating or updating an evaluation rule.
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
