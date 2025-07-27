using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a score in Langfuse
/// </summary>
public class Score
{
    /// <summary>
    ///     Unique identifier of the score
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the trace this score belongs to
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    ///     ID of the session this score belongs to
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    ///     ID of the observation this score belongs to
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     Name of the score
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Value of the score
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    ///     String representation of the score value (for categorical and boolean scores)
    /// </summary>
    [JsonPropertyName("stringValue")]
    public string? StringValue { get; set; }

    /// <summary>
    ///     Data type of the score
    /// </summary>
    [JsonPropertyName("dataType")]
    public ScoreDataType DataType { get; set; }

    /// <summary>
    ///     Source of the score (manual, API, etc.)
    /// </summary>
    [JsonPropertyName("source")]
    public ScoreSource? Source { get; set; }

    /// <summary>
    ///     Comment associated with the score
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }

    /// <summary>
    ///     Timestamp when the score was created
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    ///     Author ID who created the score
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; set; }

    /// <summary>
    ///     Configuration ID used for this score
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }

    /// <summary>
    ///     Timestamp when the score was created in the system
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the score was last updated
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     Metadata associated with the score
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}