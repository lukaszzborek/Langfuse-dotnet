using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public interface IIngestionEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
}