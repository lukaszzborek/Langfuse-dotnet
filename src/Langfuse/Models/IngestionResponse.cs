using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class IngestionResponse
{
    [JsonPropertyName("successes")]
    public IngestionSuccessResponse[]? Successes { get; set; }
    
    [JsonPropertyName("errors")]
    public IngestionErrorResponse[]? Errors { get; set; }
}

public class IngestionSuccessResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("status")]
    public int Status { get; set; }
}

public class IngestionErrorResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("status")]
    public int Status { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("error")]
    public string Error { get; set; }
}