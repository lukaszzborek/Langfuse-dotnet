using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a trace from the Langfuse API
/// </summary>
public class Trace
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
    ///     Input data for the trace
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    ///     Output data for the trace
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }
    
    /// <summary>
    ///     Session ID associated with the trace
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }
    
    /// <summary>
    ///     Release version
    /// </summary>
    [JsonPropertyName("release")]
    public string? Release { get; set; }
    
    /// <summary>
    ///     Version of the trace
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    /// <summary>
    ///     User ID associated with the trace
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    ///     Metadata associated with the trace
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Tags associated with the trace
    /// </summary>
    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }
    
    /// <summary>
    ///     Public flag indicating if trace is visible publicly
    /// </summary>
    [JsonPropertyName("public")]
    public bool? Public { get; set; }
    
    /// <summary>
    ///     Environment in which the trace was created
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }
}