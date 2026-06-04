using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Live evaluation rule for incoming data: which evaluator to use, which targets trigger scoring,
///     how often scoring runs, and how target fields populate evaluator variables.
/// </summary>
public class EvaluationRule
{
    /// <summary>
    ///     Stable evaluation rule identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Human-readable deployment name, independent from the evaluator name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Evaluator currently used by this rule.
    /// </summary>
    [JsonPropertyName("evaluator")]
    public required EvaluationRuleEvaluator Evaluator { get; init; }

    /// <summary>
    ///     Target object type that should trigger scoring.
    /// </summary>
    [JsonPropertyName("target")]
    public required EvaluationRuleTarget Target { get; init; }

    /// <summary>
    ///     Desired enabled state configured by the client.
    /// </summary>
    [JsonPropertyName("enabled")]
    public required bool Enabled { get; init; }

    /// <summary>
    ///     Effective runtime status after Langfuse applies validation and blocking rules.
    /// </summary>
    [JsonPropertyName("status")]
    public required EvaluationRuleStatus Status { get; init; }

    /// <summary>
    ///     Machine-readable reason when Status=Paused, otherwise null.
    /// </summary>
    [JsonPropertyName("pausedReason")]
    public string? PausedReason { get; init; }

    /// <summary>
    ///     Human-readable explanation when Status=Paused, otherwise null.
    /// </summary>
    [JsonPropertyName("pausedMessage")]
    public string? PausedMessage { get; init; }

    /// <summary>
    ///     Fraction of matching target objects that should be evaluated (greater than 0, up to 1).
    /// </summary>
    [JsonPropertyName("sampling")]
    public required double Sampling { get; init; }

    /// <summary>
    ///     Filter conditions used to decide whether a target should be evaluated.
    /// </summary>
    [JsonPropertyName("filter")]
    public required EvaluationRuleFilter[] Filter { get; init; }

    /// <summary>
    ///     Variable mappings used to populate the evaluator prompt from the live target object.
    /// </summary>
    [JsonPropertyName("mapping")]
    public required EvaluationRuleMapping[] Mapping { get; init; }

    /// <summary>
    ///     Timestamp when the evaluation rule was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Timestamp when the evaluation rule was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }
}
