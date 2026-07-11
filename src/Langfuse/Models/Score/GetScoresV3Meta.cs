using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Cursor pagination metadata of a v3 scores response
/// </summary>
public class GetScoresV3Meta
{
    /// <summary>
    ///     Number of items per page
    /// </summary>
    [JsonPropertyName("limit")]
    public required int Limit { get; init; }

    /// <summary>
    ///     URL-safe base64 (base64url) cursor for the next page. Absent when there are no more results.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; init; }
}