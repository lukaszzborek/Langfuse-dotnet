using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
/// Body of the event creation request
/// </summary>
public class CreateEventBody
{
    /// <summary>
    /// Trace identifier
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
    
    /// <summary>
    /// Identifier of the event. Useful for sorting/filtering in the UI
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// The time at which the event started, defaults to the current time
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }
    
    /// <summary>
    /// Additional metadata of the event. Can be any JSON object. Metadata is merged when being updated via the API.
    /// </summary>
    [JsonPropertyName("endTime")]
    public object? Metadata { get; set; }
    
    /// <summary>
    /// The input to the event. Can be any JSON object.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }
    
    /// <summary>
    /// The output to the event. Can be any JSON object.
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }
    
    /// <summary>
    /// The level of the event. Used for sorting/filtering of traces with elevated error levels and for highlighting in the UI
    /// </summary>
    [JsonPropertyName("level")]
    public LangfuseLogLevel Level { get; set; }
    
    /// <summary>
    /// The status message of the event. Additional field for context of the event. E.g. the error message of an error event
    /// </summary>
    [JsonPropertyName("statusMessage")]
    public string? StatusMessage { get; set; }
    
    /// <summary>
    /// Parent observation id
    /// </summary>
    [JsonPropertyName("parentObservationId")]
    public string? ParentObservationId { get; set; }
    
    /// <summary>
    /// The version of the event type. Used to understand how changes to the event type affect metrics. Useful in debugging
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    /// <summary>
    /// The id of the event can be set, otherwise a random id is generated
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    
}