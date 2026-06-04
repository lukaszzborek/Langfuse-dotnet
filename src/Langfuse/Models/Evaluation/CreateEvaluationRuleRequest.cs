using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating an evaluation rule.
/// </summary>
public class CreateEvaluationRuleRequest
{
    /// <summary>
    ///     Human-readable deployment name. Must be unique within the project for public evaluation rules.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Evaluator family to use. Langfuse resolves it to its latest version before saving the rule.
    /// </summary>
    [JsonPropertyName("evaluator")]
    public required EvaluationRuleEvaluatorReference Evaluator { get; init; }

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

    /// <summary>
    ///     Required variable mappings. Every evaluator variable must appear exactly once.
    /// </summary>
    [JsonPropertyName("mapping")]
    public required EvaluationRuleMapping[] Mapping { get; init; }
}
