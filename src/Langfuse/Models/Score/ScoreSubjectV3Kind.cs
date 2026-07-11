using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Kind of entity a v3 score is attached to, used as the discriminator of the <see cref="ScoreSubjectV3" /> union
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<ScoreSubjectV3Kind>))]
public enum ScoreSubjectV3Kind
{
    /// <summary>
    ///     The score is attached to a trace
    /// </summary>
    Trace,

    /// <summary>
    ///     The score is attached to an observation
    /// </summary>
    Observation,

    /// <summary>
    ///     The score is attached to a session
    /// </summary>
    Session,

    /// <summary>
    ///     The score is attached to a dataset run (experiment)
    /// </summary>
    Experiment
}