using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Stored version of a code evaluator.
/// </summary>
public class CodeEvaluatorVersion : EvaluatorVersion
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public override EvaluatorType Type => EvaluatorType.Code;

    /// <summary>
    ///     Source code executed for each matched observation.
    /// </summary>
    [JsonPropertyName("sourceCode")]
    public required string SourceCode { get; init; }

    /// <summary>
    ///     Runtime language used to execute the source code.
    /// </summary>
    [JsonPropertyName("sourceCodeLanguage")]
    public required CodeEvaluatorSourceCodeLanguage SourceCodeLanguage { get; init; }
}