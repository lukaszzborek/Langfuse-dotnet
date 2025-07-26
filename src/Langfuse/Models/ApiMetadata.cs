using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Metadata information for API responses with pagination
/// </summary>
public class ApiMetadata
{
    /// <summary>
    ///     Current page number (1-based)
    /// </summary>
    [JsonPropertyName("page")]
    public int Page { get; set; }

    /// <summary>
    ///     Number of items per page
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    /// <summary>
    ///     Total number of items available
    /// </summary>
    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    /// <summary>
    ///     Total number of pages available
    /// </summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }
}