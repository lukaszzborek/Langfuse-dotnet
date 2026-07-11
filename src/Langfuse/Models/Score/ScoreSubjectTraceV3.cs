using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A v3 score subject referencing a trace
/// </summary>
public class ScoreSubjectTraceV3 : ScoreSubjectV3
{
    /// <inheritdoc />
    [JsonPropertyName("kind")]
    public override ScoreSubjectV3Kind Kind => ScoreSubjectV3Kind.Trace;
}