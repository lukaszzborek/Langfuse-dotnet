using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A v3 score subject referencing an observation
/// </summary>
public class ScoreSubjectObservationV3 : ScoreSubjectV3
{
    /// <inheritdoc />
    [JsonPropertyName("kind")]
    public override ScoreSubjectV3Kind Kind => ScoreSubjectV3Kind.Observation;

    /// <summary>
    ///     The parent trace ID, if available
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; init; }
}