using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Response from creating scores in a batch. HTTP 202 sets only <see cref="Message" /> ("Accepted");
///     HTTP 207 sets <see cref="Accepted" />, <see cref="Rejected" /> and <see cref="Errors" />.
/// </summary>
public record CreateScoreBatchResponse
{
    /// <summary>
    ///     "Accepted" when the whole batch was accepted (HTTP 202)
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>
    ///     Number of accepted scores (HTTP 207)
    /// </summary>
    [JsonPropertyName("accepted")]
    public int? Accepted { get; init; }

    /// <summary>
    ///     Number of rejected scores (HTTP 207)
    /// </summary>
    [JsonPropertyName("rejected")]
    public int? Rejected { get; init; }

    /// <summary>
    ///     Errors for rejected scores (HTTP 207)
    /// </summary>
    [JsonPropertyName("errors")]
    public List<CreateScoreBatchError> Errors { get; init; } = [];
}

/// <summary>
///     Error for a rejected score in a batch
/// </summary>
public record CreateScoreBatchError
{
    /// <summary>
    ///     Error message
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}
