using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Score field definition of an evaluator output. For CATEGORICAL evaluators, Categories and
///     ShouldAllowMultipleMatches are populated; for NUMERIC and BOOLEAN evaluators only Description applies.
/// </summary>
public class EvaluatorOutputScoreDefinition
{
    /// <summary>
    ///     Human-readable instructions for the score the evaluator should return.
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    /// <summary>
    ///     Allowed category labels. Only applicable for CATEGORICAL evaluators.
    /// </summary>
    [JsonPropertyName("categories")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string[]? Categories { get; init; }

    /// <summary>
    ///     Whether multiple category matches are allowed. Only applicable for CATEGORICAL evaluators.
    /// </summary>
    [JsonPropertyName("shouldAllowMultipleMatches")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ShouldAllowMultipleMatches { get; init; }
}
