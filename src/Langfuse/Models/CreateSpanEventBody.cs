using System.Text.Json.Serialization;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Models;

/// <summary>
/// Create span event body
/// </summary>
public class CreateSpanEventBody
{
    /// <summary>
    /// Time provider
    /// </summary>
    [JsonIgnore]
    public TimeProvider TimeProvider { get; init; }
    
    /// <summary>
    /// Langfuse trace object
    /// </summary>
    [JsonIgnore]
    public LangfuseTrace LangfuseTrace { get; init; }
    
    /// <summary>
    /// Trace ID
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
    
    /// <summary>
    /// Identifier of the span. Useful for sorting/filtering in the UI.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// The time at which the span started, defaults to the current time
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }
    
    /// <summary>
    /// The time at which the span ended
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }    
    
    /// <summary>
    /// Additional metadata of the span. Can be any JSON object. Metadata is merged when being updated via the API.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
    
    /// <summary>
    /// The input to the span. Can be any JSON object
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }
    
    /// <summary>
    /// The output to the span. Can be any JSON object
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }
    
    /// <summary>
    /// The level of the span. Used for sorting/filtering of traces with elevated error levels and for highlighting in the UI
    /// </summary>
    [JsonPropertyName("level")]
    public LangfuseLogLevel Level { get; set; }
    
    /// <summary>
    /// The status message of the span. Additional field for context of the event. E.g. the error message of an error event
    /// </summary>
    [JsonPropertyName("statusMessage")]
    public string? StatusMessage { get; set; }
    
    /// <summary>
    /// Parent observation ID. Used to link the span to the parent event or span
    /// </summary>
    [JsonPropertyName("parentObservationId")]
    public string? ParentObservationId { get; set; }
    
    /// <summary>
    /// The version of the span type. Used to understand how changes to the span type affect metrics. Useful in debugging
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    /// <summary>
    /// The id of the span can be set, otherwise a random id is generated. Spans are upserted on id.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    /// <summary>
    /// Set output and end time
    /// </summary>
    /// <param name="output">Output of span</param>
    public void SetOutput(object output)
    {
        Output = output;
        EndTime = TimeProvider.GetUtcNow().UtcDateTime;
    }
    
    public CreateEventBody CreateEvent(string eventName, object? input = null, object? output = null, DateTime? eventDate = null)
    {
        eventDate ??= TimeProvider.GetUtcNow().UtcDateTime;
        
        var eventBody = new CreateEventBody()
        {
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId,
            Name = eventName,
            Input = input,
            Output = output,
            StartTime = eventDate,
            ParentObservationId = Id
        };
        
        var createEvent = new CreateEvent(eventBody, eventDate.Value);
        LangfuseTrace.Events.Add(createEvent);
        return eventBody;
    } 
    
    public CreateSpanEventBody CreateSpan(string spanName, object? metadata = null, object? input = null,
        DateTime? startDate = null)
    {
        startDate ??= TimeProvider.GetUtcNow().UtcDateTime;
        
        var spanBody = new CreateSpanEventBody()
        {
            LangfuseTrace = LangfuseTrace,
            TimeProvider = TimeProvider,
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId,
            Name = spanName,
            Metadata = metadata,
            Input = input,
            StartTime = startDate,
            ParentObservationId = Id
        };
        
        var createSpan = new CreateSpanEvent(spanBody, Guid.NewGuid().ToString(), startDate.Value.ToString("o"));
        LangfuseTrace.Spans.Add(createSpan);
        return spanBody;
    }
    
    public CreateGenerationEventBody CreateGenerationEvent(string generationName, object? input = null, object? output = null, DateTime? eventDate = null)
    {
        eventDate ??= TimeProvider.GetUtcNow().UtcDateTime;
        
        var generationBody = new CreateGenerationEventBody()
        {
            TimeProvider = TimeProvider,
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId,
            Name = generationName,
            Input = input,
            Output = output,
            StartTime = eventDate,
            ParentObservationId = Id
        };
        
        var createGeneration = new CreateGenerationEvent(generationBody, Guid.NewGuid().ToString(), eventDate.Value.ToString("o"));
        LangfuseTrace.Generations.Add(createGeneration);
        return generationBody;
    }
}