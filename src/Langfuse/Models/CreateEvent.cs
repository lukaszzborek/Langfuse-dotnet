using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Create event
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
    ///     Constructor
    /// </summary>
    /// <param name="body">Create event body</param>
    /// <param name="id">Event id</param>
    /// <param name="timestamp">Event date</param>
    public CreateEvent(CreateEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="body">Create event body</param>
    /// <param name="timestamp">Event date</param>
    public CreateEvent(CreateEventBody body, DateTime timestamp) : this(body, Guid.NewGuid().ToString(),
        timestamp.ToString("o"))
    {
    }
}