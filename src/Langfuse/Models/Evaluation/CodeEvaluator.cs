using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Code evaluator: scores data by executing source code for each matched observation.
/// </summary>
public class CodeEvaluator : Evaluator
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
    ///     Runtime language for <see cref="SourceCode" />.
    /// </summary>
    [JsonPropertyName("sourceCodeLanguage")]
    public required CodeEvaluatorSourceCodeLanguage SourceCodeLanguage { get; init; }
}