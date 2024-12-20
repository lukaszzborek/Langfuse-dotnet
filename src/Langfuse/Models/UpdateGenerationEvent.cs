using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class UpdateGenerationEvent : IIngestionEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonPropertyName("body")]
    public GenerationEventBody Body { get; set; }
    
    public UpdateGenerationEvent()
    {
        Type = "generation-update";
    }
}