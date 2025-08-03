namespace zborek.Langfuse.Models.Session;

/// <summary>
///     Request parameters for listing sessions
/// </summary>
public class SessionListRequest
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
    ///     Filter by sessions created on or after this datetime
    /// </summary>
    public DateTime? FromTimestamp { get; set; }

    /// <summary>
    ///     Filter by sessions created before this datetime
    /// </summary>
    public DateTime? ToTimestamp { get; set; }

    /// <summary>
    ///     Filter by environment
    /// </summary>
    public string? Environment { get; set; }

    /// <summary>
    ///     Filter by user ID
    /// </summary>
    public string? UserId { get; set; }
}