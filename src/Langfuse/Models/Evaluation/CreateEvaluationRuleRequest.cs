using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an evaluation rule. Use <see cref="CreateLlmAsJudgeEvaluationRuleRequest" />
///     or <see cref="CreateCodeEvaluationRuleRequest" /> depending on the evaluator type.
/// </summary>
public abstract class CreateEvaluationRuleRequest
{
    /// <summary>
    ///     Human-readable deployment name. Must be unique within the project for public evaluation rules.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Target object type to evaluate.
    /// </summary>
    [JsonPropertyName("target")]
    public required EvaluationRuleTarget Target { get; init; }

    /// <summary>
    ///     Whether the deployment should be active immediately after creation.
    /// </summary>
    [JsonPropertyName("enabled")]
    public required bool Enabled { get; init; }

    /// <summary>
    ///     Optional sampling fraction. Defaults to 1.
    /// </summary>
    [JsonPropertyName("sampling")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Sampling { get; init; }

    /// <summary>
    ///     Optional filter list. Omit or pass an empty list to evaluate all matching targets.
    /// </summary>
    [JsonPropertyName("filter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EvaluationRuleFilter[]? Filter { get; init; }
}