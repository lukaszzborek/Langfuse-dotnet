using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.LlmConnection;

/// <summary>
/// Paginated response containing LLM connections with pagination metadata
/// </summary>
public record PaginatedLlmConnections
{
    /// <summary>
    /// Array of LLM connections in this page of results
    /// </summary>
    [JsonPropertyName("data")]
    public required LlmConnection[] Data { get; init; }

    /// <summary>
    /// Metadata containing pagination information including current page, total items, and page size
    /// </summary>
    [JsonPropertyName("meta")]
    public required ApiMetadata Meta { get; init; }
}
