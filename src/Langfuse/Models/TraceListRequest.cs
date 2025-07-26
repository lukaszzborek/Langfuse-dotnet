namespace zborek.Langfuse.Models;

/// <summary>
///     Request parameters for listing traces
/// </summary>
public class TraceListRequest
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
    ///     Filter by user ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    ///     Filter by trace name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Filter by traces created on or after this datetime
    /// </summary>
    public DateTime? FromTimestamp { get; set; }

    /// <summary>
    ///     Filter by traces created before this datetime
    /// </summary>
    public DateTime? ToTimestamp { get; set; }

    /// <summary>
    ///     Filter traces with specific tags
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    ///     Filter by session ID
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    ///     Filter by environment
    /// </summary>
    public string? Environment { get; set; }

    /// <summary>
    ///     Filter by trace status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    ///     Filter by version
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Filter by release
    /// </summary>
    public string? Release { get; set; }
}