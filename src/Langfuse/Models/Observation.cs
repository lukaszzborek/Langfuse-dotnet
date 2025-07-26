namespace zborek.Langfuse.Models;

/// <summary>
///     Represents an observation (span, generation, or event) in Langfuse
/// </summary>
public class Observation
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
    ///     Type of observation (SPAN, GENERATION, EVENT)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the trace this observation belongs to
    /// </summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the parent observation (if any)
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
    ///     Input data for the observation
    /// </summary>
    public object? Input { get; set; }

    /// <summary>
    ///     Output data for the observation
    /// </summary>
    public object? Output { get; set; }

    /// <summary>
    ///     Metadata associated with the observation
    /// </summary>
    public object? Metadata { get; set; }

    /// <summary>
    ///     Level of the observation (DEBUG, DEFAULT, WARNING, ERROR)
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    ///     Status message of the observation
    /// </summary>
    public string? StatusMessage { get; set; }

    /// <summary>
    ///     Version of the observation
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Model name (for generations)
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    ///     Model parameters (for generations)
    /// </summary>
    public object? ModelParameters { get; set; }

    /// <summary>
    ///     Usage statistics (for generations)
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

/// <summary>
///     Usage statistics for LLM operations
/// </summary>
public class Usage
{
    /// <summary>
    ///     Number of input tokens
    /// </summary>
    public int? InputTokens { get; set; }

    /// <summary>
    ///     Number of output tokens
    /// </summary>
    public int? OutputTokens { get; set; }

    /// <summary>
    ///     Total number of tokens
    /// </summary>
    public int? TotalTokens { get; set; }

    /// <summary>
    ///     Input cost in USD
    /// </summary>
    public decimal? InputCost { get; set; }

    /// <summary>
    ///     Output cost in USD
    /// </summary>
    public decimal? OutputCost { get; set; }

    /// <summary>
    ///     Total cost in USD
    /// </summary>
    public decimal? TotalCost { get; set; }
}