using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class UpdateGenerationEvent : IIngestionEvent
{
    /// <summary>
    /// Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "generation-update";
    
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
    /// Update generation event body
    /// </summary>
    [JsonPropertyName("body")]
    public CreateGenerationEventBody Body { get; set; }
    
    public UpdateGenerationEvent()
    {
    }
}