using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A v3 correction score that overrides other scores
/// </summary>
public class CorrectionScoreV3 : ScoreV3
{
    /// <inheritdoc />
    [JsonPropertyName("dataType")]
    public override ScoreV3DataType DataType => ScoreV3DataType.Correction;

    /// <summary>
    ///     The correction content of the score. Empty string if not set.
    /// </summary>
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}