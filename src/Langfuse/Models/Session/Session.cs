using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Trace;

namespace zborek.Langfuse.Models.Session;

/// <summary>
///     Represents a session - a grouping mechanism for related traces, typically representing user interactions or
///     conversation flows.
///     Sessions help organize traces that belong to the same context or user journey.
/// </summary>
public class SessionModel
{
    /// <summary>
    ///     Unique identifier for the session
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    ///     Timestamp when the session was created/started
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Identifier for the project that the session is associated with.
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;
    
    /// <summary>
    ///     Traces associated with this session (when fetching session details)
    /// </summary>
    [JsonPropertyName("traces")]
    public TraceModel[]? Traces { get; set; }
    
    /// <summary>
    ///     Environment from which this session originated. Helps categorize sessions by deployment environment.
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }
}