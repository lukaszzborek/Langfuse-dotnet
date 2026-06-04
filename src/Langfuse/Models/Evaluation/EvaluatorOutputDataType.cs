using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Structured score type returned by an evaluator.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<EvaluatorOutputDataType>))]
public enum EvaluatorOutputDataType
{
    /// <summary>
    ///     A numeric score such as 0.82.
    /// </summary>
    Numeric,

    /// <summary>
    ///     A boolean score such as true.
    /// </summary>
    Boolean,

    /// <summary>
    ///     One or more category labels from a fixed list.
    /// </summary>
    Categorical
}
