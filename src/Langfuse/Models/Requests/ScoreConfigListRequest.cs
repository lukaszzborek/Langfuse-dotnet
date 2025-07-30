using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

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
    ///     Number of items to skip for pagination
    /// </summary>
    [JsonPropertyName("offset")]
    public int? Offset { get; set; }
}