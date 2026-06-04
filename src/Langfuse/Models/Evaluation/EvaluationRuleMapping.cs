using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Maps one evaluator prompt variable to one source field from the target object.
/// </summary>
public class EvaluationRuleMapping
{
    /// <summary>
    ///     Prompt variable name without braces (for example "input").
    /// </summary>
    [JsonPropertyName("variable")]
    public required string Variable { get; init; }

    /// <summary>
    ///     Source field that should populate the prompt variable.
    /// </summary>
    [JsonPropertyName("source")]
    public required EvaluationRuleMappingSource Source { get; init; }

    /// <summary>
    ///     Optional JSONPath selector applied to the selected source before it is passed to the evaluator prompt.
    ///     Must start with "$". Most useful with Source=Metadata.
    /// </summary>
    [JsonPropertyName("jsonPath")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? JsonPath { get; init; }
}
