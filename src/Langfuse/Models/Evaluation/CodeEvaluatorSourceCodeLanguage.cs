using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Code evaluator runtime language.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<CodeEvaluatorSourceCodeLanguage>))]
public enum CodeEvaluatorSourceCodeLanguage
{
    /// <summary>
    ///     Python runtime.
    /// </summary>
    Python,

    /// <summary>
    ///     TypeScript runtime.
    /// </summary>
    Typescript
}