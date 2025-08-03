using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Represents a trace - the top-level execution unit that contains spans, generations, and events in a hierarchical
///     structure.
///     Traces track end-to-end operations and provide context for all nested observations.
/// </summary>
public class TraceModel
{
    /// <summary>
    ///     Unique identifier for the trace
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the trace was created
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    /// <summary>
    ///     Name of the trace
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Input data for the trace. Can be any JSON object representing the initial request or parameters.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    ///     Output data for the trace. Can be any JSON object representing the final result or response.
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }

    /// <summary>
    ///     Session ID associated with the trace. Links multiple traces together in the same user session.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    ///     Release version of the application when the trace was created. Useful for tracking performance across deployments.
    /// </summary>
    [JsonPropertyName("release")]
    public string? Release { get; set; }

    /// <summary>
    ///     Version of the trace schema or implementation. Used to understand how changes affect metrics.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    ///     User ID associated with the trace. Identifies which user initiated this trace.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    ///     Metadata associated with the trace. Can be any JSON object containing additional context and custom properties.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Tags associated with the trace. Array of strings used for categorization and filtering in the UI.
    /// </summary>
    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    /// <summary>
    ///     Public flag indicating if trace is visible publicly. Public traces are accessible via URL without login.
    /// </summary>
    [JsonPropertyName("public")]
    public bool? Public { get; set; }

    /// <summary>
    ///     Environment from which this trace originated. Must be lowercase alphanumeric with hyphens/underscores, not starting
    ///     with 'langfuse'.
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }
}