namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request parameters for listing sessions
/// </summary>
public class SessionListRequest
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
    ///     Filter by timestamp (from)
    /// </summary>
    public DateTime? FromTimestamp { get; set; }

    /// <summary>
    ///     Filter by timestamp (to)
    /// </summary>
    public DateTime? ToTimestamp { get; set; }

    /// <summary>
    ///     Filter by environment
    /// </summary>
    public string? Environment { get; set; }

    /// <summary>
    ///     Order by field (createdAt, updatedAt)
    /// </summary>
    public string? OrderBy { get; set; }

    /// <summary>
    ///     Order direction (ASC, DESC)
    /// </summary>
    public string? Order { get; set; }
}