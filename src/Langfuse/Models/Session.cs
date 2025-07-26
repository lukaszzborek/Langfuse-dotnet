using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a session from the Langfuse API
/// </summary>
public class Session
{
    /// <summary>
    ///     Unique identifier for the session
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the session was created/started
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    ///     Timestamp when the session ended
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    ///     User ID associated with the session
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    ///     Metadata associated with the session
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, object>? Metadata { get; set; }

    /// <summary>
    ///     Environment in which the session was created
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    /// <summary>
    ///     Traces associated with this session (when fetching session details)
    /// </summary>
    [JsonPropertyName("traces")]
    public Trace[]? Traces { get; set; }

    /// <summary>
    ///     Count of traces in this session
    /// </summary>
    [JsonPropertyName("traceCount")]
    public int? TraceCount { get; set; }

    /// <summary>
    ///     Public flag indicating if session is visible publicly
    /// </summary>
    [JsonPropertyName("public")]
    public bool? Public { get; set; }
}