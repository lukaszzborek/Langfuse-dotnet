using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class TraceEvent : IIngestionEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonPropertyName("body")]
    public TraceBody Body { get; set; }

    public TraceEvent(TraceBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
        Type = "trace-create";
    }
}