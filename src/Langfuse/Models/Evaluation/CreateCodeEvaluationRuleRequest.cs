using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an evaluation rule that uses a code evaluator. Do not send variable
///     mappings; Langfuse stores the fixed code runtime mapping automatically.
/// </summary>
public class CreateCodeEvaluationRuleRequest : CreateEvaluationRuleRequest
{
    /// <summary>
    ///     Code evaluator family to use. Langfuse resolves it to its latest version before saving the rule.
    /// </summary>
    [JsonPropertyName("evaluator")]
    public required CodeEvaluationRuleEvaluatorReference Evaluator { get; init; }
}