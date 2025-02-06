using System.Text.Json.Serialization;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Models;

public class CreateSpanEventBody
{
    [JsonIgnore]
    public TimeProvider TimeProvider { private get; set; }
    
    [JsonIgnore]
    public LangfuseTrace LangfuseTrace { private get; set; }
    
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }
    
    [JsonPropertyName("metadata")]
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
    
    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }
    
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
        
        var createEvent = new CreateEvent(eventBody, Guid.NewGuid().ToString(), eventDate.Value.ToString("o"));
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
    
    public GenerationEventBody CreateGenerationEvent(string generationName, object? input = null, object? output = null, DateTime? eventDate = null)
    {
        eventDate ??= TimeProvider.GetUtcNow().UtcDateTime;
        
        var generationBody = new GenerationEventBody()
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