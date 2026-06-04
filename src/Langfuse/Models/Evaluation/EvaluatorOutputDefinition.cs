using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Structured output schema returned by an evaluator. DataType controls how scores should be interpreted.
/// </summary>
public class EvaluatorOutputDefinition
{
    /// <summary>
    ///     Structured score type returned by the evaluator.
    /// </summary>
    [JsonPropertyName("dataType")]
    public required EvaluatorOutputDataType DataType { get; init; }

    /// <summary>
    ///     Reasoning field definition.
    /// </summary>
    [JsonPropertyName("reasoning")]
    public required EvaluatorOutputFieldDefinition Reasoning { get; init; }

    /// <summary>
    ///     Score field definition.
    /// </summary>
    [JsonPropertyName("score")]
    public required EvaluatorOutputScoreDefinition Score { get; init; }
}
