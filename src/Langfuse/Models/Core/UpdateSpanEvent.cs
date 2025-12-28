using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Span update event
/// </summary>
public class UpdateSpanEvent : IIngestionEvent
{
    /// <summary>
    ///     Update span event body
    /// </summary>
    [JsonPropertyName("body")]
    public UpdateSpanEventBody Body { get; set; }

    /// <summary>
    ///     Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "span-update";

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
    public UpdateSpanEvent(UpdateSpanEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }

    /// <summary>
    /// </summary>
    /// <param name="body">Span event body</param>
    /// <param name="timestamp">Span event date</param>
    public UpdateSpanEvent(UpdateSpanEventBody body, string timestamp)
        : this(body, Guid.NewGuid().ToString(), timestamp)
    {
    }
}