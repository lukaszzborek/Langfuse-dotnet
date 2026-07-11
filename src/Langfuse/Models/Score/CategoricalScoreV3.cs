using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     A v3 score with a string category value
/// </summary>
public class CategoricalScoreV3 : ScoreV3
{
    /// <inheritdoc />
    [JsonPropertyName("dataType")]
    public override ScoreV3DataType DataType => ScoreV3DataType.Categorical;

    /// <summary>
    ///     The string category value of the score
    /// </summary>
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}