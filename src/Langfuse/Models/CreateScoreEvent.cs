using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class CreateScoreEvent : IIngestionEvent
{
    /// <summary>
    /// Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "score-create";
    
    /// <summary>
    /// Event ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    /// <summary>
    /// Date of the event
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    /// <summary>
    /// Create score event body
    /// </summary>
    [JsonPropertyName("body")]
    public ScoreEventBody Body { get; set; }

    public CreateScoreEvent(ScoreEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }
}