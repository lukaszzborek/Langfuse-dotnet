using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class IngestionScoreEvent : IIngestionEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonPropertyName("body")]
    public ScoreEventBody Body { get; set; }

    public IngestionScoreEvent(ScoreEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
        Type = "score-create";
    }
}