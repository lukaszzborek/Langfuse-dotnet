using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A v3 score with a free-form text value
/// </summary>
public class TextScoreV3 : ScoreV3
{
    /// <inheritdoc />
    [JsonPropertyName("dataType")]
    public override ScoreV3DataType DataType => ScoreV3DataType.Text;

    /// <summary>
    ///     The text content of the score
    /// </summary>
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}