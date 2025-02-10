using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
/// Ingetion response
/// </summary>
public class IngestionResponse
{
    /// <summary>
    /// Ingestion successes
    /// </summary>
    [JsonPropertyName("successes")]
    public IngestionSuccessResponse[]? Successes { get; set; } = [];
    
    /// <summary>
    /// Errors
    /// </summary>
    [JsonPropertyName("errors")]
    public IngestionErrorResponse[]? Errors { get; set; }
}

/// <summary>
/// Successful ingestion response
/// </summary>
public class IngestionSuccessResponse
{
    /// <summary>
    /// Observation ID. This is the same as the ID of the event that was ingested.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    /// <summary>
    /// Status code
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}

/// <summary>
/// Error ingestion response
/// </summary>
public class IngestionErrorResponse
{
    /// <summary>
    /// Observation ID. This is the same as the ID of the event that was ingested.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    /// <summary>
    /// Status code
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
    
    /// <summary>
    /// Message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    /// <summary>
    /// Error message
    /// </summary>
    [JsonPropertyName("error")]
    public string Error { get; set; }
}