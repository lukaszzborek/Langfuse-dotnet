using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class CreateSpanEvent : IIngestionEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonPropertyName("body")]
    public CreateSpanEventBody Body { get; set; }

    public CreateSpanEvent(CreateSpanEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
        Type = "span-create";
    }
}