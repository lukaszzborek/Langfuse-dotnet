using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Complete definition replacement for a code evaluator.
/// </summary>
public class UpdateCodeEvaluatorRequest : UpdateEvaluatorRequest
{
    /// <summary>
    ///     Evaluator type. The type of an existing evaluator cannot change.
    /// </summary>
    [JsonPropertyName("type")]
    public EvaluatorType Type => EvaluatorType.Code;

    /// <summary>
    ///     Complete replacement source code.
    /// </summary>
    [JsonPropertyName("sourceCode")]
    public required string SourceCode { get; init; }

    /// <summary>
    ///     Runtime language used to execute the source code.
    /// </summary>
    [JsonPropertyName("sourceCodeLanguage")]
    public required CodeEvaluatorSourceCodeLanguage SourceCodeLanguage { get; init; }
}