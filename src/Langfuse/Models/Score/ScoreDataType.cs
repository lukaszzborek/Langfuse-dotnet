using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Score data type
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<ScoreDataType>))]
public enum ScoreDataType
{
    /// <summary>
    ///     Numeric
    /// </summary>
    Numeric,

    /// <summary>
    ///     Boolean
    /// </summary>
    Boolean,

    /// <summary>
    ///     Categorical
    /// </summary>
    Categorical
}