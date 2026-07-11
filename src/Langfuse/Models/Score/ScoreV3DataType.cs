using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Data type of a v3 score, used as the discriminator of the <see cref="ScoreV3" /> union
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<ScoreV3DataType>))]
public enum ScoreV3DataType
{
    /// <summary>
    ///     Numeric score with a number value
    /// </summary>
    Numeric,

    /// <summary>
    ///     Boolean score with a true/false value
    /// </summary>
    Boolean,

    /// <summary>
    ///     Categorical score with a string category value
    /// </summary>
    Categorical,

    /// <summary>
    ///     Text score with a free-form string value
    /// </summary>
    Text,

    /// <summary>
    ///     Correction score that overrides other scores, with a string value
    /// </summary>
    Correction
}