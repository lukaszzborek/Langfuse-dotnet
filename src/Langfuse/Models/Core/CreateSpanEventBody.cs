using System.Text.Json.Serialization;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Create span event body
/// </summary>
public class CreateSpanEventBody : IDisposable
{
    [JsonIgnore]
    internal bool Scoped { get; set; }

    /// <summary>
    ///     Time provider
    /// </summary>
    [JsonIgnore]
    public TimeProvider TimeProvider { get; init; } = TimeProvider.System;

    /// <summary>
    ///     Langfuse trace object
    /// </summary>
    [JsonIgnore]
    public LangfuseTrace? LangfuseTrace { get; init; }

    /// <summary>
    ///     Trace ID
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    ///     Identifier of the span. Useful for sorting/filtering in the UI.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     The time at which the span started, defaults to the current time
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }

    /// <summary>
    ///     The time at which the span ended
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    ///     Additional metadata of the span. Can be any JSON object. Metadata is merged when being updated via the API.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     The input to the span. Can be any JSON object
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    ///     The output to the span. Can be any JSON object
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }

    /// <summary>
    ///     The level of the span. Used for sorting/filtering of traces with elevated error levels and for highlighting in the
    ///     UI
    /// </summary>
    [JsonPropertyName("level")]
    public LangfuseLogLevel Level { get; set; }

    /// <summary>
    ///     The status message of the span. Additional field for context of the event. E.g. the error message of an error event
    /// </summary>
    [JsonPropertyName("statusMessage")]
    public string? StatusMessage { get; set; }

    /// <summary>
    ///     Parent observation ID. Used to link the span to the parent event or span
    /// </summary>
    [JsonPropertyName("parentObservationId")]
    public string? ParentObservationId { get; set; }

    /// <summary>
    ///     The version of the span type. Used to understand how changes to the span type affect metrics. Useful in debugging
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    ///     The id of the span can be set, otherwise a random id is generated. Spans are upserted on id.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     Remove the last parent id from the trace
    /// </summary>
    public void Dispose()
    {
        if (Scoped)
        {
            LangfuseTrace?.RemoveLastParentId();
        }
    }

    /// <summary>
    ///     Set output and end time
    /// </summary>
    /// <param name="output">Output of span</param>
    public void SetOutput(object output)
    {
        Output = output;
        EndTime = TimeProvider.GetUtcNow().UtcDateTime;
    }

    /// <summary>
    ///     Creates a child event to span event. If <see cref="LangfuseTrace" /> is set, also add this event to
    ///     <see cref="LangfuseTrace.Events" />
    /// </summary>
    /// <param name="eventName">Identifier of the event for sorting/filtering</param>
    /// <param name="input">The input to the event. Can be any JSON object</param>
    /// <param name="output">The output to the event. Can be any JSON object</param>
    /// <param name="eventDate">The time at which the event started, defaults to the current time</param>
    /// <returns>Created span event</returns>
    public CreateEventBody CreateEvent(string eventName, object? input = null, object? output = null,
        DateTime? eventDate = null)
    {
        eventDate ??= TimeProvider.GetUtcNow().UtcDateTime;
        if (LangfuseTrace == null)
        {
            throw new InvalidOperationException("LangfuseTrace is not set");
        }

        return LangfuseTrace.CreateEvent(eventName, input, output, eventDate);
    }

    public CreateEventBody CreateEventScoped(string eventName, object? input = null, object? output = null,
        DateTime? eventDate = null)
    {
        eventDate ??= TimeProvider.GetUtcNow().UtcDateTime;
        if (LangfuseTrace == null)
        {
            throw new InvalidOperationException("LangfuseTrace is not set");
        }

        return LangfuseTrace.CreateEventScoped(eventName, input, output, eventDate);
    }

    /// <summary>
    ///     Creates a child span event to span event. If <see cref="LangfuseTrace" /> is set, also add this event to
    ///     <see cref="LangfuseTrace.Spans" />
    /// </summary>
    /// <param name="spanName">Identifier of the span for sorting/filtering</param>
    /// <param name="metadata">
    ///     Additional metadata of the span. Can be any JSON object. Metadata is merged when being updated
    ///     via the API.
    /// </param>
    /// <param name="input">The input to the span. Can be any JSON object</param>
    /// <param name="startDate">The time at which the span started, defaults to the current time</param>
    /// <returns>Created span event</returns>
    public CreateSpanEventBody CreateSpan(string spanName, object? metadata = null, object? input = null,
        DateTime? startDate = null)
    {
        startDate ??= TimeProvider.GetUtcNow().UtcDateTime;
        if (LangfuseTrace == null)
        {
            throw new InvalidOperationException("LangfuseTrace is not set");
        }

        return LangfuseTrace.CreateSpan(spanName, metadata, input, startDate);
    }

    public CreateSpanEventBody CreateSpanScoped(string spanName, object? metadata = null, object? input = null,
        DateTime? startDate = null)
    {
        startDate ??= TimeProvider.GetUtcNow().UtcDateTime;
        if (LangfuseTrace == null)
        {
            throw new InvalidOperationException("LangfuseTrace is not set");
        }

        return LangfuseTrace.CreateSpanScoped(spanName, metadata, input, startDate);
    }

    /// <summary>
    ///     Creates a child generation event to span event. If <see cref="LangfuseTrace" /> is set, also add this event to
    ///     <see cref="LangfuseTrace.Generations" />
    /// </summary>
    /// <param name="generationName">The name of the generation event.</param>
    /// <param name="input">The input data for the generation event.</param>
    /// <param name="output">The output data for the generation event.</param>
    /// <param name="eventDate">The date and time of the generation event. If not provided, the current time is used.</param>
    /// <returns>A <see cref="CreateGenerationEventBody" /> object representing the details of the created generation event.</returns>
    public CreateGenerationEventBody CreateGeneration(string generationName, object? input = null,
        object? output = null, DateTime? eventDate = null)
    {
        eventDate ??= TimeProvider.GetUtcNow().UtcDateTime;

        if (LangfuseTrace == null)
        {
            throw new InvalidOperationException("LangfuseTrace is not set");
        }

        return LangfuseTrace.CreateGeneration(generationName, input, output, eventDate);
    }

    public CreateGenerationEventBody CreateGenerationScoped(string generationName, object? input = null,
        object? output = null, DateTime? eventDate = null)
    {
        eventDate ??= TimeProvider.GetUtcNow().UtcDateTime;

        if (LangfuseTrace == null)
        {
            throw new InvalidOperationException("LangfuseTrace is not set");
        }

        return LangfuseTrace.CreateGenerationScoped(generationName, input, output, eventDate);
    }
}