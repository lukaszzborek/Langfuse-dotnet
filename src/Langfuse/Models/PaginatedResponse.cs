using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Base class for paginated API responses
/// </summary>
/// <typeparam name="T">The type of items in the response</typeparam>
public class PaginatedResponse<T>
{
    /// <summary>
    ///     The data items in this page
    /// </summary>
    [JsonPropertyName("data")]
    public T[] Data { get; set; } = [];

    /// <summary>
    ///     Pagination metadata
    /// </summary>
    [JsonPropertyName("meta")]
    public ApiMetadata Meta { get; set; } = new();
}