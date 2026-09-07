using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an evaluation rule using stable evaluator identifiers. Rules always use the
///     latest version of each associated evaluator. An enabled rule requires at least one evaluator assignment.
/// </summary>
public class CreateEvaluationRuleRequest
{
    /// <summary>
    ///     Human-readable rule name. Names are not identifiers and do not need to be unique.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Whether live execution should start immediately. Enabled rules require at least one evaluator assignment.
    /// </summary>
    [JsonPropertyName("enabled")]
    public required bool Enabled { get; init; }

    /// <summary>
    ///     Fraction of matching observations to evaluate. Omit to use the default of 1, which evaluates every match.
    /// </summary>
    [JsonPropertyName("sampling")]
    public double? Sampling { get; init; }

    /// <summary>
    ///     Conditions used to select observations. Omit to match every incoming observation.
    /// </summary>
    [JsonPropertyName("filter")]
    public EvaluationRuleFilter[]? Filter { get; init; }

    /// <summary>
    ///     Evaluators to attach to this rule. Disabled rules may use an empty list as a draft.
    /// </summary>
    [JsonPropertyName("evaluatorAssignments")]
    public required EvaluationRuleEvaluatorAssignmentInput[] EvaluatorAssignments { get; init; }
}