using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an evaluator. If the same name already exists in your project, Langfuse
///     creates the next version and returns it. Use <see cref="CreateLlmAsJudgeEvaluatorRequest" /> or
///     <see cref="CreateCodeEvaluatorRequest" />.
/// </summary>
public abstract class CreateEvaluatorRequest
{
    /// <summary>
    ///     Evaluator name within the authenticated project.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Evaluator engine type. Determined by the concrete request class.
    /// </summary>
    [JsonPropertyName("type")]
    public abstract EvaluatorType Type { get; }
}