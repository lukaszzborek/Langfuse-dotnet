using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Source values accepted when creating a score via the public REST API.
///     EVAL is reserved for internal evaluator outputs and is intentionally not accepted here;
///     use <see cref="ScoreSource" /> when reading scores.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<CreateScoreSource>))]
public enum CreateScoreSource
{
    /// <summary>
    ///     API-created score
    /// </summary>
    Api,

    /// <summary>
    ///     Manual annotation
    /// </summary>
    Annotation
}
