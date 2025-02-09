using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class CreateTraceEvent : IIngestionEvent
{
    /// <summary>
    /// Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "trace-create";
    
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
    /// Create trace event body
    /// </summary>
    [JsonPropertyName("body")]
    public CreateTraceBody Body { get; set; }

    public CreateTraceEvent(CreateTraceBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }
}