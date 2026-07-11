using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A v3 score with a boolean value
/// </summary>
public class BooleanScoreV3 : ScoreV3
{
    /// <inheritdoc />
    [JsonPropertyName("dataType")]
    public override ScoreV3DataType DataType => ScoreV3DataType.Boolean;

    /// <summary>
    ///     The boolean value of the score
    /// </summary>
    [JsonPropertyName("value")]
    public required bool Value { get; init; }
}