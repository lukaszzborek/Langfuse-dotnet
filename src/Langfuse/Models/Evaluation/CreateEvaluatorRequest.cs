using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an evaluator. The returned evaluator starts at version 1. Names are not
///     identifiers and do not need to be unique. Use <see cref="CreateLlmAsJudgeEvaluatorRequest" /> or
///     <see cref="CreateCodeEvaluatorRequest" />.
/// </summary>
public abstract class CreateEvaluatorRequest
{
    /// <summary>
    ///     Human-readable evaluator name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Optional human-readable evaluator description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    ///     Evaluator engine type. Determined by the concrete request class.
    /// </summary>
    [JsonPropertyName("type")]
    public abstract EvaluatorType Type { get; }
}