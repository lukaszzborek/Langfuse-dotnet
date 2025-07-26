namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request parameters for listing observations
/// </summary>
public class ObservationListRequest
{
    /// <summary>
    ///     Page number for pagination (1-based)
    /// </summary>
    public int? Page { get; set; }

    /// <summary>
    ///     Number of items per page (max 1000)
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
    ///     Filter by observation type (SPAN, GENERATION, EVENT)
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    ///     Filter by trace ID
    /// </summary>
    public string? TraceId { get; set; }

    /// <summary>
    ///     Filter by parent observation ID
    /// </summary>
    public string? ParentObservationId { get; set; }

    /// <summary>
    ///     Filter by start time (from)
    /// </summary>
    public DateTime? FromStartTime { get; set; }

    /// <summary>
    ///     Filter by start time (to)
    /// </summary>
    public DateTime? ToStartTime { get; set; }

    /// <summary>
    ///     Filter by model name
    /// </summary>
    public string? Model { get; set; }

    /// <summary>
    ///     Filter by version
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Order by field (startTime, endTime, name)
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    ///     Order direction (ASC, DESC)
    /// </summary>
    public string? Order { get; set; }
}