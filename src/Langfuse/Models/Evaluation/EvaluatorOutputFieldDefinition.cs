using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Instructions for what the evaluator should return in a given output field.
/// </summary>
public class EvaluatorOutputFieldDefinition
{
    /// <summary>
    ///     Human-readable instructions for what the evaluator should return in this field.
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; init; }
}
