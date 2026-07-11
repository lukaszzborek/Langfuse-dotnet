using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for creating a code evaluator.
/// </summary>
public class CreateCodeEvaluatorRequest : CreateEvaluatorRequest
{
    /// <inheritdoc />
    [JsonPropertyName("type")]
    public override EvaluatorType Type => EvaluatorType.Code;

    /// <summary>
    ///     Code executed for each matched observation.
    /// </summary>
    [JsonPropertyName("sourceCode")]
    public required string SourceCode { get; init; }

    /// <summary>
    ///     Runtime language for <see cref="SourceCode" />.
    /// </summary>
    [JsonPropertyName("sourceCodeLanguage")]
    public required CodeEvaluatorSourceCodeLanguage SourceCodeLanguage { get; init; }
}