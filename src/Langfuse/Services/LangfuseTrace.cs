using zborek.Langfuse.Attributes;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Services;

/// <summary>
///     Langfuse trace object. Typical single request or operation
/// </summary>
public class LangfuseTrace
{
    private readonly ILangfuseClient? _langfuseClient;
    private readonly List<string> _parentIds = [];
    private readonly TimeProvider _timeProvider;

    /// <summary>
    ///     Trace id
    /// </summary>
    public Guid TraceId { get; }

    /// <summary>
    ///     Trace event information
    /// </summary>
    public CreateTraceEvent Trace { get; }

    /// <summary>
    ///     List of events. Basic building blocks. They are used to track discrete events in a trace
    /// </summary>
    public List<CreateEvent> Events { get; } = [];

    /// <summary>
    ///     List of spans. Represent durations of units of work in a trace
    /// </summary>
    public List<CreateSpanEvent> Spans { get; } = [];

    /// <summary>
    ///     List of generations. Generation are spans used to log generations of AI models. They contain additional attributes
    ///     about the model, the prompt, and the completion
    /// </summary>
    public List<CreateGenerationEvent> Generations { get; } = [];

    /// <summary>
    /// </summary>
    /// <param name="timeProvider">Time provider</param>
    /// <param name="langfuseClient">Langfuse client</param>
    public LangfuseTrace(TimeProvider timeProvider, ILangfuseClient langfuseClient)
    {
        _timeProvider = timeProvider;
        _langfuseClient = langfuseClient;
        TraceId = Guid.NewGuid();
        _parentIds.Add(TraceId.ToString());
        var date = timeProvider.GetUtcNow().UtcDateTime;
        var traceBody = new CreateTraceBody { Id = TraceId.ToString(), Timestamp = date };
        Trace = new CreateTraceEvent(traceBody, Guid.NewGuid().ToString(), date.ToString("o"));
    }

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="name">Name of trace</param>
    /// <param name="timeProvider"></param>
    public LangfuseTrace(string name, TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        TraceId = Guid.NewGuid();
        _parentIds.Add(TraceId.ToString());
        var date = timeProvider.GetUtcNow().UtcDateTime;
        var traceBody = new CreateTraceBody { Id = TraceId.ToString(), Timestamp = date, Name = name };
        Trace = new CreateTraceEvent(traceBody, Guid.NewGuid().ToString(), date.ToString("o"));
    }
    
    /// <summary>
    ///     Set name of trace visible in langfuse. Used when using dependency injection
    /// </summary>
    /// <param name="name">Trace name</param>
    public void SetTraceName(string name)
    {
        Trace.Body.Name = name;
    }

    /// <summary>
    ///     Set input prompt of trace
    /// </summary>
    /// <param name="input">Input prompt</param>
    public void SetInput(string input)
    {
        Trace.Body.Input = input;
    }
    
    /// <summary>
    ///     Set output of trace
    /// </summary>
    /// <param name="output">LLM output</param>
    public void SetOutput(string output)
    {
        Trace.Body.Output = output;
    }

    /// <summary>
    ///     Create event, basic building blocks. Used to track discrete events in a trace
    /// </summary>
    /// <param name="eventName">Name of event</param>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="eventDate"></param>
    /// <returns></returns>
    [NonScopedMethod(nameof(CreateEventScoped))]
    public CreateEventBody CreateEvent(string eventName, object? input = null, object? output = null,
        DateTime? eventDate = null)
    {
        eventDate ??= _timeProvider.GetUtcNow().UtcDateTime;

        var eventBody = new CreateEventBody
        {
            LangfuseTrace = this,
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId.ToString(),
            Name = eventName,
            Input = input,
            Output = output,
            StartTime = eventDate,
            ParentObservationId = _parentIds[^1]
        };

        var createEvent = new CreateEvent(eventBody, eventBody.Id, eventDate.Value.ToString("o"));
        Events.Add(createEvent);
        return eventBody;
    }

    /// <summary>
    ///     Create event, basic building blocks. Used to track discrete events in a trace
    /// </summary>
    /// <param name="eventName">Name of event</param>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <param name="eventDate"></param>
    /// <returns></returns>
    [ScopedMethod(nameof(CreateEvent))]
    public CreateEventBody CreateEventScoped(string eventName, object? input = null, object? output = null,
        DateTime? eventDate = null)
    {
        var eventBody = CreateEvent(eventName, input, output, eventDate);
        eventBody.Scoped = true;
        _parentIds.Add(eventBody.Id!);
        return eventBody;
    }

    /// <summary>
    ///     Create span, represent durations of units of work in a trace
    /// </summary>
    /// <param name="spanName"></param>
    /// <param name="metadata"></param>
    /// <param name="input"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    [NonScopedMethod(nameof(CreateSpanScoped))]
    public CreateSpanEventBody CreateSpan(string spanName, object? metadata = null, object? input = null,
        DateTime? startDate = null)
    {
        startDate ??= _timeProvider.GetUtcNow().UtcDateTime;

        var spanBody = new CreateSpanEventBody
        {
            LangfuseTrace = this,
            TimeProvider = _timeProvider,
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId.ToString(),
            Name = spanName,
            Metadata = metadata,
            Input = input,
            StartTime = startDate,
            ParentObservationId = _parentIds[^1]
        };

        var createSpan = new CreateSpanEvent(spanBody, spanBody.Id, startDate.Value.ToString("o"));
        Spans.Add(createSpan);
        return spanBody;
    }

    /// <summary>
    ///     Create span, represent durations of units of work in a trace
    /// </summary>
    /// <param name="spanName"></param>
    /// <param name="metadata"></param>
    /// <param name="input"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    [ScopedMethod(nameof(CreateSpan))]
    public CreateSpanEventBody CreateSpanScoped(string spanName, object? metadata = null, object? input = null,
        DateTime? startDate = null)
    {
        var spanBody = CreateSpan(spanName, metadata, input, startDate);
        spanBody.Scoped = true;
        _parentIds.Add(spanBody.Id!);
        return spanBody;
    }

    /// <summary>
    ///     Creates a new generation event and associates it with the current trace.
    /// </summary>
    /// <param name="generationName">The name of the generation event.</param>
    /// <param name="input">The input data for the generation event. Optional.</param>
    /// <param name="output">The output data for the generation event. Optional.</param>
    /// <param name="eventDate">
    ///     The timestamp for when the generation event occurred. If not provided, the current UTC time
    ///     will be used.
    /// </param>
    /// <returns>A newly created <see cref="CreateGenerationEventBody" /> object representing the generation event.</returns>
    [NonScopedMethod(nameof(CreateGenerationScoped))]
    public CreateGenerationEventBody CreateGeneration(string generationName, object? input = null,
        object? output = null, DateTime? eventDate = null)
    {
        eventDate ??= _timeProvider.GetUtcNow().UtcDateTime;

        var generationBody = new CreateGenerationEventBody
        {
            LangfuseTrace = this,
            TimeProvider = _timeProvider,
            Id = Guid.NewGuid().ToString(),
            TraceId = TraceId.ToString(),
            Name = generationName,
            Input = input,
            Output = output,
            StartTime = eventDate,
            ParentObservationId = _parentIds[^1]
        };

        var createGeneration =
            new CreateGenerationEvent(generationBody, generationBody.Id, eventDate.Value.ToString("o"));
        Generations.Add(createGeneration);

        return generationBody;
    }
    
    /// <summary>
    ///     Creates a new generation event and associates it with the current trace.
    /// </summary>
    /// <param name="generationName">The name of the generation event.</param>
    /// <param name="input">The input data for the generation event. Optional.</param>
    /// <param name="output">The output data for the generation event. Optional.</param>
    /// <param name="eventDate">
    ///     The timestamp for when the generation event occurred. If not provided, the current UTC time
    ///     will be used.
    /// </param>
    /// <returns>A newly created <see cref="CreateGenerationEventBody" /> object representing the generation event.</returns>
    [ScopedMethod(nameof(CreateGeneration))]
    public CreateGenerationEventBody CreateGenerationScoped(string generationName, object? input = null,
        object? output = null, DateTime? eventDate = null)
    {
        var generationBody = CreateGeneration(generationName, input, output, eventDate);
        generationBody.Scoped = true;
        _parentIds.Add(generationBody.Id!);
        return generationBody;
    }

    /// <summary>
    ///     Retrieves a list of all ingestion events associated with the trace, including trace, events,
    ///     spans, and generations.
    /// </summary>
    /// <returns>A list of ingestion events implementing the <c>IIngestionEvent</c> interface.</returns>
    public List<IIngestionEvent> GetEvents()
    {
        var ingestionEvents = new List<IIngestionEvent>();
        ingestionEvents.Add(Trace);
        ingestionEvents.AddRange(Events);
        ingestionEvents.AddRange(Spans);
        ingestionEvents.AddRange(Generations);
        return ingestionEvents;
    }

    /// <summary>
    ///     Removes the last parent ID from the list of parent IDs.
    /// </summary>
    internal void RemoveLastParentId()
    {
        _parentIds.RemoveAt(_parentIds.Count - 1);
    }

    /// <summary>
    ///     Ingest trace to langfuse
    /// </summary>
    public async Task IngestAsync()
    {
        if (_langfuseClient == null)
        {
            return;
        }

        await _langfuseClient.IngestAsync(this);
    }
}