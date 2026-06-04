using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Where an evaluator comes from.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<EvaluatorScope>))]
public enum EvaluatorScope
{
    /// <summary>
    ///     Created in your project.
    /// </summary>
    Project,

    /// <summary>
    ///     Provided by Langfuse.
    /// </summary>
    Managed
}
