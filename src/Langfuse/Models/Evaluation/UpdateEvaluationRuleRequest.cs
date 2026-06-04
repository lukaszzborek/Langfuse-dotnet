using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Partial update body for an evaluation rule. Provide only the fields you want to change.
///     An empty body is rejected by the API.
/// </summary>
public class UpdateEvaluationRuleRequest
{
    /// <summary>
    ///     Updated deployment name.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; init; }

    /// <summary>
    ///     Updated evaluator family. Langfuse resolves it to its latest version before saving the rule.
    /// </summary>
    [JsonPropertyName("evaluator")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EvaluationRuleEvaluatorReference? Evaluator { get; init; }

    /// <summary>
    ///     Updated target object type.
    /// </summary>
    [JsonPropertyName("target")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EvaluationRuleTarget? Target { get; init; }

    /// <summary>
    ///     Updated desired enabled state.
    /// </summary>
    [JsonPropertyName("enabled")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? Enabled { get; init; }

    /// <summary>
    ///     Updated sampling fraction.
    /// </summary>
    [JsonPropertyName("sampling")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public double? Sampling { get; init; }

    /// <summary>
    ///     Updated filter list.
    /// </summary>
    [JsonPropertyName("filter")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EvaluationRuleFilter[]? Filter { get; init; }

    /// <summary>
    ///     Updated variable mappings.
    /// </summary>
    [JsonPropertyName("mapping")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public EvaluationRuleMapping[]? Mapping { get; init; }
}
