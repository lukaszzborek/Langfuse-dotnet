using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Response of the v3 scores list endpoint
/// </summary>
public class GetScoresV3Response
{
    /// <summary>
    ///     List of scores
    /// </summary>
    [JsonPropertyName("data")]
    public required ScoreV3[] Data { get; init; }

    /// <summary>
    ///     Cursor pagination metadata
    /// </summary>
    [JsonPropertyName("meta")]
    public required GetScoresV3Meta Meta { get; init; }
}