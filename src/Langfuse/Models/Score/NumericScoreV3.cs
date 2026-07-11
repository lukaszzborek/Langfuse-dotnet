using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A v3 score with a numeric value
/// </summary>
public class NumericScoreV3 : ScoreV3
{
    /// <inheritdoc />
    [JsonPropertyName("dataType")]
    public override ScoreV3DataType DataType => ScoreV3DataType.Numeric;

    /// <summary>
    ///     The numeric value of the score
    /// </summary>
    [JsonPropertyName("value")]
    public required double Value { get; init; }
}