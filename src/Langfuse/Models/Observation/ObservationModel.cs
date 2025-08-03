namespace zborek.Langfuse.Models.Observation;

/// <summary>
///     Represents an observation - a specific activity within a trace that can be a span (duration-based operation),
///     generation (AI model interaction), or event (discrete point-in-time occurrence).
/// </summary>
public class ObservationModel
{
    /// <summary>
    ///     Unique identifier of the observation
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the observation
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Type of observation. Can be SPAN (duration-based operations), GENERATION (AI model interactions), or EVENT
    ///     (discrete point-in-time occurrences).
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the trace this observation belongs to. Links the observation to its parent trace context.
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the parent observation (if any). Creates hierarchical relationships between observations within a trace.
    /// </summary>
    public string? ParentObservationId { get; set; }

    /// <summary>
    ///     Start time of the observation
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    ///     End time of the observation
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    ///     Input data for the observation. Can be any JSON object representing the request, parameters, or initial data.
    /// </summary>
    public object? Input { get; set; }

    /// <summary>
    ///     Output data for the observation. Can be any JSON object representing the result, response, or final data.
    /// </summary>
    public object? Output { get; set; }

    /// <summary>
    ///     Metadata associated with the observation. Additional context and custom properties stored as JSON.
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    ///     Level of the observation indicating importance or severity (DEBUG, DEFAULT, WARNING, ERROR). Used for filtering and
    ///     highlighting in the UI.
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    ///     Status message of the observation. Additional field providing context about the observation state, such as error
    ///     messages.
    /// </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    ///     Version of the observation implementation. Used to understand how changes to the observation type affect metrics.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Model name used for this observation (applicable to generation-type observations). Identifies which AI model was
    ///     used.
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    ///     Model parameters used for this observation (applicable to generation-type observations). Configuration settings
    ///     passed to the AI model.
    /// </summary>
    public object? ModelParameters { get; set; }

    /// <summary>
    ///     Usage statistics for this observation (applicable to generation-type observations). Contains token counts and cost
    ///     information.
    /// </summary>
    public Usage? Usage { get; set; }

    /// <summary>
    ///     Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}