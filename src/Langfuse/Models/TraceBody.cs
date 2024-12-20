using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class TraceBody
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    
    [JsonPropertyName("input")]
    public object Input { get; set; }
    
    [JsonPropertyName("output")]
    public object Output { get; set; }
    
    [JsonPropertyName("sessionId")]
    public string SessionId { get; set; }
    
    [JsonPropertyName("release")]
    public string Release { get; set; }
    
    [JsonPropertyName("Version")]
    public string Version { get; set; }
    
    [JsonPropertyName("metadata")]
    public object Metadata { get; set; }
    
    [JsonPropertyName("tags")]
    public string[] Tags { get; set; }
    
    [JsonPropertyName("Public")]
    public bool Public { get; set; }
}