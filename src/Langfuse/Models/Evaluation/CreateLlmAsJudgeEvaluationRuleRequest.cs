using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an evaluation rule that uses an LLM-as-a-judge evaluator.
/// </summary>
public class CreateLlmAsJudgeEvaluationRuleRequest : CreateEvaluationRuleRequest
{
    /// <summary>
    ///     LLM-as-a-judge evaluator family to use. Langfuse resolves it to its latest version before
    ///     saving the rule.
    /// </summary>
    [JsonPropertyName("evaluator")]
    public required LlmAsJudgeEvaluationRuleEvaluatorReference Evaluator { get; init; }

    /// <summary>
    ///     Required variable mappings. Every evaluator variable must appear exactly once. Build this list
    ///     from the evaluator variables returned by the evaluator endpoints.
    /// </summary>
    [JsonPropertyName("mapping")]
    public required EvaluationRuleMapping[] Mapping { get; init; }
}