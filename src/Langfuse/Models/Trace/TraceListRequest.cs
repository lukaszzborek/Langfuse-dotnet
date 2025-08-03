namespace zborek.Langfuse.Models.Trace;

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
    ///     Filter by session ID
    /// </summary>
    public string? SessionId { get; set; }

    /// <summary>
    ///     Filter by traces created on or after this datetime
    /// </summary>
    public DateTime? FromTimestamp { get; set; }

    /// <summary>
    ///     Filter by traces created before this datetime
    /// </summary>
    public DateTime? ToTimestamp { get; set; }

    /// <summary>
    ///     Format of the string [field].[asc/desc]. Fields: id, timestamp, name, userId, release, version, public, bookmarked,
    ///     sessionId. Example: timestamp.asc
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    ///     Filter traces with specific tags
    /// </summary>
    public string[]? Tags { get; set; }

    /// <summary>
    ///     Optional filter to only include traces with a certain version.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Optional filter to only include traces with a certain release
    /// </summary>
    public string? Release { get; set; }

    /// <summary>
    ///     Filter by environment
    /// </summary>
    public string? Environment { get; set; }

    /// <summary>
    ///     Comma-separated list of fields to include in the response. Available field groups are 'core' (always included),
    ///     'io' (input, output, metadata), 'scores', 'observations', 'metrics'. If not provided, all fields are included.
    ///     Example: 'core,scores,metrics'
    /// </summary>
    public string? Fields { get; set; }
}