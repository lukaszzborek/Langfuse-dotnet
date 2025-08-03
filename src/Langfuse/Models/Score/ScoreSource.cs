using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Source of the score
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<ScoreSource>))]
public enum ScoreSource
{
    /// <summary>
    ///     Manual annotation
    /// </summary>
    Annotation,

    /// <summary>
    ///     API-created score
    /// </summary>
    Api,

    /// <summary>
    ///     Evaluation-generated score
    /// </summary>
    Eval
}