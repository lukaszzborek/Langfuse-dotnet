namespace zborek.Langfuse.Models;

/// <summary>
///     Request parameters for listing observations
/// </summary>
public class ObservationListRequest
{
    /// <summary>
    ///     Page number (1-based). Default is 1.
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    ///     Number of items per page. Default is 50.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    ///     Filter by observation name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Filter by user ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    ///     Filter by observation type (e.g., "GENERATION", "SPAN", "EVENT")
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    ///     Filter by trace ID
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    ///     Filter by log level
    /// </summary>
    public LangfuseLogLevel? Level { get; set; }

    /// <summary>
    ///     Filter by parent observation ID
    /// </summary>
    public string? ParentObservationId { get; set; }

    /// <summary>
    ///     Filter by environment
    /// </summary>
    public string? Environment { get; set; }

    /// <summary>
    ///     Filter by observations starting from this time (inclusive)
    /// </summary>
    public DateTime? FromStartTime { get; set; }

    /// <summary>
    ///     Filter by observations starting before this time (exclusive)
    /// </summary>
    public DateTime? ToStartTime { get; set; }

    /// <summary>
    ///     Filter by version
    /// </summary>
    public string? Version { get; set; }
}