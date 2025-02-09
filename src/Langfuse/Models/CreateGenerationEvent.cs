using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class CreateGenerationEvent : IIngestionEvent
{
    /// <summary>
    /// Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "generation-create";
    
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
    /// Create generation event body
    /// </summary>
    [JsonPropertyName("body")]
    public CreateGenerationEventBody Body { get; set; }

    public CreateGenerationEvent(CreateGenerationEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }
}