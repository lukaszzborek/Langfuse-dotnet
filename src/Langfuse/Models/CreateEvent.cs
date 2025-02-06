using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class CreateEvent : IIngestionEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonPropertyName("body")]
    public CreateEventBody Body { get; set; }

    public CreateEvent(CreateEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
        Type = "event-create";
    }
}