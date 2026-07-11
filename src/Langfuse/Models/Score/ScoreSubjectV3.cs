using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A reference to the entity a v3 score is attached to. Base of a discriminated union on <see cref="Kind" />
///     with the concrete types <see cref="ScoreSubjectTraceV3" />, <see cref="ScoreSubjectObservationV3" />,
///     <see cref="ScoreSubjectSessionV3" /> and <see cref="ScoreSubjectExperimentV3" />.
/// </summary>
[JsonConverter(typeof(ScoreSubjectV3Converter))]
public abstract class ScoreSubjectV3
{
    /// <summary>
    ///     Kind discriminator of the subject (trace, observation, session or experiment)
    /// </summary>
    [JsonPropertyName("kind")]
    public abstract ScoreSubjectV3Kind Kind { get; }

    /// <summary>
    ///     Identifier of the entity the score is attached to
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}