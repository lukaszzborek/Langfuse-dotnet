using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Generic base class for paginated API responses from Langfuse. Provides consistent structure for responses
///     containing multiple items with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items contained in the data array</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    ///     Array of data items returned in this page of results.
    /// </summary>
    [JsonPropertyName("data")]
    public T[] Data { get; set; } = [];

    /// <summary>
    ///     Metadata containing pagination information including current page, total items, and page size.
    /// </summary>
    [JsonPropertyName("meta")]
    public ApiMetadata Meta { get; set; } = new();
}