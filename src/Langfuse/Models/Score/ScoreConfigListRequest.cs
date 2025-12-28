using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Request parameters for listing score configurations
/// </summary>
public class ScoreConfigListRequest
{
    /// <summary>
    ///     Number of items to return (default: 50, max: 1000)
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    ///     Page number (1-based). Default is 1.
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }
}