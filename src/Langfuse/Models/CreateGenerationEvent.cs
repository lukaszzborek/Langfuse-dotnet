using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class CreateGenerationEvent : IIngestionEvent
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }
    
    [JsonPropertyName("body")]
    public GenerationEventBody Body { get; set; }

    public CreateGenerationEvent(GenerationEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
        Type = "generation-create";
    }
}