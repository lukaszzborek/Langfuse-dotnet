using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Data type for score configuration. Defines the type of values that can be stored in scores using this configuration.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<ScoreConfigDataType>))]
public enum ScoreConfigDataType
{
    /// <summary>
    ///     Numeric scores - floating point or integer values.
    /// </summary>
    Numeric,

    /// <summary>
    ///     Boolean scores - true/false values.
    /// </summary>
    Boolean,

    /// <summary>
    ///     Categorical scores - predefined string categories.
    /// </summary>
    Categorical
}
