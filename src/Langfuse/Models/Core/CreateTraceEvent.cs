using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Represents a trace creation event for the Langfuse ingestion API. This is the top-level container that groups
///     related observations.
/// </summary>
public class CreateTraceEvent : IIngestionEvent
{
    /// <summary>
    ///     Create trace event body
    /// </summary>
    [JsonPropertyName("body")]
    public CreateTraceBody Body { get; set; }

    /// <summary>
    ///     Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "trace-create";

    /// <summary>
    ///     Event ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    ///     Date of the event
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    /// <summary>
    ///     Optional metadata field used by Langfuse SDKs for debugging.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Creates a new trace creation event with specified ID and timestamp.
    /// </summary>
    /// <param name="body">The trace event body containing trace data and metadata</param>
    /// <param name="id">Unique identifier for this trace event</param>
    /// <param name="timestamp">ISO 8601 timestamp string when the trace event occurred</param>
    public CreateTraceEvent(CreateTraceBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }

    /// <summary>
    ///     Creates a new trace creation event with auto-generated ID and specified timestamp.
    /// </summary>
    /// <param name="body">The trace event body containing trace data and metadata</param>
    /// <param name="timestamp">DateTime when the trace event occurred, will be converted to ISO 8601 format</param>
    public CreateTraceEvent(CreateTraceBody body, DateTime timestamp)
        : this(body, Guid.NewGuid().ToString(), timestamp.ToString("o"))
    {
    }
}