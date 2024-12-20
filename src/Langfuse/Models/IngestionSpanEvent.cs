using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class IngestionSpanEvent : IIngestionEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonPropertyName("body")]
    public UpdateSpanEventBody Body { get; set; }

    public IngestionSpanEvent(UpdateSpanEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
        Type = "span-update";
    }
}