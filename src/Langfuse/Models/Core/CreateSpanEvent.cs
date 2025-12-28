using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Create span event
/// </summary>
public class CreateSpanEvent : IIngestionEvent
{
    /// <summary>
    ///     Create span event body
    /// </summary>
    [JsonPropertyName("body")]
    public CreateSpanEventBody Body { get; set; }

    /// <summary>
    ///     Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "span-create";

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
    /// </summary>
    /// <param name="body">Span event body</param>
    /// <param name="id">Span event id</param>
    /// <param name="timestamp">Span event date</param>
    public CreateSpanEvent(CreateSpanEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }

    /// <summary>
    /// </summary>
    /// <param name="body">Span event body</param>
    /// <param name="timestamp">Span event date</param>
    public CreateSpanEvent(CreateSpanEventBody body, DateTime timestamp)
        : this(body, Guid.NewGuid().ToString(), timestamp.ToString("o"))
    {
    }
}