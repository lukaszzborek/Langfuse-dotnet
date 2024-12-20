using Langfuse.Models;

namespace Langfuse.Services;

public class LangfuseTrace
{
    private readonly TimeProvider _timeProvider;
    public Guid TraceId { get; private set; }
    public TraceEvent Trace { get; private set; }
    public List<CreateEvent> Events { get; private set; } = [];
    public List<CreateSpanEvent> Spans { get; private set; } = [];
    public List<CreateGenerationEvent> Generations { get; private set; } = [];

    public LangfuseTrace(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        TraceId = Guid.NewGuid();
        var date = timeProvider.GetUtcNow().UtcDateTime;
        var traceBody = new TraceBody() { Id = TraceId.ToString(), Timestamp = date };
        Trace = new TraceEvent(traceBody, Guid.NewGuid().ToString(), date.ToString("o"));
    }
    
    public LangfuseTrace(string name, TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        TraceId = Guid.NewGuid();
        var date = timeProvider.GetUtcNow().UtcDateTime;
        var traceBody = new TraceBody() { Id = TraceId.ToString(), Timestamp = date, Name = name };
        Trace = new TraceEvent(traceBody, Guid.NewGuid().ToString(), date.ToString("o"));
    }

    public void SetTraceName(string name)
    {
        Trace.Body.Name = name;
    }
    
    public CreateEventBody CreateEvent(string eventName, object? input = null, object? output = null, DateTime? eventDate = null)
    {
        eventDate ??= _timeProvider.GetUtcNow().UtcDateTime;
        
        var eventBody = new CreateEventBody()
        {
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId.ToString(),
            Name = eventName,
            Input = input,
            Output = output,
            StartTime = eventDate,
            ParentObservationId = TraceId.ToString()
        };
        
        var createEvent = new CreateEvent(eventBody, Guid.NewGuid().ToString(), eventDate.Value.ToString("o"));
        Events.Add(createEvent);
        return eventBody;
    }

    public CreateSpanEventBody CreateSpan(string spanName, object? metadata = null, object? input = null,
        DateTime? startDate = null)
    {
        startDate ??= _timeProvider.GetUtcNow().UtcDateTime;
        
        var spanBody = new CreateSpanEventBody()
        {
            LangfuseTrace = this,
            TimeProvider = _timeProvider,
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId.ToString(),
            Name = spanName,
            Metadata = metadata,
            Input = input,
            StartTime = startDate,
            ParentObservationId = TraceId.ToString()
        };
        
        var createSpan = new CreateSpanEvent(spanBody, Guid.NewGuid().ToString(), startDate.Value.ToString("o"));
        Spans.Add(createSpan);
        return spanBody;
    }
    
    public GenerationEventBody CreateGeneration(string generationName, object? input = null, object? output = null, DateTime? eventDate = null)
    {
        eventDate ??= _timeProvider.GetUtcNow().UtcDateTime;
        
        var generationBody = new GenerationEventBody()
        {
            TimeProvider = _timeProvider,
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId.ToString(),
            Name = generationName,
            Input = input,
            Output = output,
            StartTime = eventDate,
            ParentObservationId = TraceId.ToString()
        };
        
        var createGeneration = new CreateGenerationEvent(generationBody, Guid.NewGuid().ToString(), eventDate.Value.ToString("o"));
        Generations.Add(createGeneration);
        return generationBody;
    }
    
    public List<IIngestionEvent> GetEvents()
    {
        var ingestionEvents = new List<IIngestionEvent>();
        ingestionEvents.Add(Trace);
        ingestionEvents.AddRange(Events);
        ingestionEvents.AddRange(Spans);
        ingestionEvents.AddRange(Generations);
        return ingestionEvents;
    }
}