using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a discrete event creation request for the Langfuse ingestion API. Events capture point-in-time occurrences within a trace.
/// </summary>
public class CreateEvent : IIngestionEvent
{
    /// <summary>
    ///     Create event body
    /// </summary>
    [JsonPropertyName("body")]
    public CreateEventBody Body { get; set; }

    /// <summary>
    ///     Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "event-create";

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
    ///     Creates a new event creation request with specified ID and timestamp.
    /// </summary>
    /// <param name="body">The event body containing event data, metadata, and hierarchy information</param>
    /// <param name="id">Unique identifier for this event</param>
    /// <param name="timestamp">ISO 8601 timestamp string when the event occurred</param>
    public CreateEvent(CreateEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }

    /// <summary>
    ///     Creates a new event creation request with auto-generated ID and specified timestamp.
    /// </summary>
    /// <param name="body">The event body containing event data, metadata, and hierarchy information</param>
    /// <param name="timestamp">DateTime when the event occurred, will be converted to ISO 8601 format</param>
    public CreateEvent(CreateEventBody body, DateTime timestamp) : this(body, Guid.NewGuid().ToString(),
        timestamp.ToString("o"))
    {
    }
}