using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class UpdateSpanEvent : IIngestionEvent
{
    /// <summary>
    /// Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "span-update";
    
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
    /// Update span event body
    /// </summary>
    [JsonPropertyName("body")]
    public UpdateSpanEventBody Body { get; set; }

    public UpdateSpanEvent(UpdateSpanEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }
}