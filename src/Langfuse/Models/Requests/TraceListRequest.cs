namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request parameters for listing traces
/// </summary>
public class TraceListRequest
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
    ///     Filter by trace name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Filter by user ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    ///     Filter by session ID
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    ///     Filter by timestamp (from)
    /// </summary>
    public DateTime? FromTimestamp { get; set; }

    /// <summary>
    ///     Filter by timestamp (to)
    /// </summary>
    public DateTime? ToTimestamp { get; set; }

    /// <summary>
    ///     Filter by version
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Filter by release
    /// </summary>
    public string? Release { get; set; }

    /// <summary>
    ///     Order by field (timestamp, name, sessionId)
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    ///     Order direction (ASC, DESC)
    /// </summary>
    public string? Order { get; set; }

    /// <summary>
    ///     Search term for tags
    /// </summary>
    public IList<string>? Tags { get; set; }
}