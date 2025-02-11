using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

/// <summary>
/// Langfuse trace object. Typical single request or operation
/// </summary>
public class LangfuseTrace
{
    private readonly TimeProvider _timeProvider;
    
    /// <summary>
    /// Trace id
    /// </summary>
    public Guid TraceId { get; private set; }
    
    /// <summary>
    /// Trace event information
    /// </summary>
    public TraceEvent Trace { get; private set; }
    
    /// <summary>
    /// List of events. Basic building blocks. They are used to track discrete events in a trace
    /// </summary>
    public List<CreateEvent> Events { get; private set; } = [];
    
    /// <summary>
    /// List of spans. Represent durations of units of work in a trace
    /// </summary>
    public List<CreateSpanEvent> Spans { get; private set; } = [];
    
    /// <summary>
    /// List of generations. Generation are spans used to log generations of AI models. They contain additional attributes about the model, the prompt, and the completion
    /// </summary>
    public List<CreateGenerationEvent> Generations { get; private set; } = [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeProvider"></param>
    public LangfuseTrace(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        TraceId = Guid.NewGuid();
        var date = timeProvider.GetUtcNow().UtcDateTime;
        var traceBody = new TraceBody() { Id = TraceId.ToString(), Timestamp = date };
        Trace = new TraceEvent(traceBody, Guid.NewGuid().ToString(), date.ToString("o"));
    }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name of trace</param>
    /// <param name="timeProvider"></param>
    public LangfuseTrace(string name, TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        TraceId = Guid.NewGuid();
        var date = timeProvider.GetUtcNow().UtcDateTime;
        var traceBody = new TraceBody() { Id = TraceId.ToString(), Timestamp = date, Name = name };
        Trace = new TraceEvent(traceBody, Guid.NewGuid().ToString(), date.ToString("o"));
    }

    
    /// <summary>
    /// Set name of trace visible in langfuse. Used when using dependency injection
    /// </summary>
    /// <param name="name"></param>
    public void SetTraceName(string name)
    {
        Trace.Body.Name = name;
    }
    
    
    /// <summary>
    /// Create event, basic building blocks. Used to track discrete events in a trace
    /// </summary>
    /// <param name="eventName">Name of event</param>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="eventDate"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Create span, represent durations of units of work in a trace
    /// </summary>
    /// <param name="spanName"></param>
    /// <param name="metadata"></param>
    /// <param name="input"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
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