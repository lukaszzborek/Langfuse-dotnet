using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Pagination metadata accompanying paginated API responses, providing navigation and count information for result sets.
/// </summary>
public class ApiMetadata
{
    /// <summary>
    ///     Current page number in the result set, using 1-based indexing.
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    ///     Maximum number of items returned per page in this response.
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    ///     Total number of items available across all pages, useful for calculating progress and navigation.
    /// </summary>
    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    /// <summary>
    ///     Total number of pages available based on the limit and total items count.
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}