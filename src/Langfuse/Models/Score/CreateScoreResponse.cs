using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Response from creating a score
/// </summary>
public record CreateScoreResponse
{
    /// <summary>
    ///     The id of the created score in Langfuse
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}
