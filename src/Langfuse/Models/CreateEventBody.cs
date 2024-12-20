using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class CreateEventBody
{
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }
    
    [JsonPropertyName("endTime")]
    public object? Metadata { get; set; }
    
    [JsonPropertyName("input")]
    public object? Input { get; set; }
    
    [JsonPropertyName("output")]
    public object? Output { get; set; }
    
    [JsonPropertyName("level")]
    public LangfuseLogLevel Level { get; set; }
    
    [JsonPropertyName("statusMessage")]
    public string? StatusMessage { get; set; }
    
    [JsonPropertyName("parentObservationId")]
    public string? ParentObservationId { get; set; }
    
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    
}