using System.Text.Json.Serialization;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Span update event body
/// </summary>
public class UpdateSpanEventBody : IDisposable
{
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
    [JsonPropertyName("endTime")]
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
        LangfuseTrace?.RemoveLastParentId();
    }
}