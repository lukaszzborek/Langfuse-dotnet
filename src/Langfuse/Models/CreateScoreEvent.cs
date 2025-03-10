using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Create score event
/// </summary>
public class CreateScoreEvent : IIngestionEvent
{
    /// <summary>
    ///     Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "score-create";

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
    ///     Create score event body
    /// </summary>
    [JsonPropertyName("body")]
    public CreateScoreEventBody Body { get; set; }

    /// <summary>
    /// </summary>
    /// <param name="body">Score event body</param>
    /// <param name="id">Score event id</param>
    /// <param name="timestamp">Score event date</param>
    public CreateScoreEvent(CreateScoreEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }

    /// <summary>
    /// </summary>
    /// <param name="body">Score event body</param>
    /// <param name="timestamp">Score event date</param>
    public CreateScoreEvent(CreateScoreEventBody body, string timestamp)
        : this(body, Guid.NewGuid().ToString(), timestamp)
    {
    }
}