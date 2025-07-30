using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request parameters for listing annotation queue items
/// </summary>
public class AnnotationQueueItemListRequest
{
    /// <summary>
    ///     Filter by item status
    /// </summary>
    [JsonPropertyName("status")]
    public AnnotationQueueStatus? Status { get; set; }

    /// <summary>
    ///     Page number, starts at 1
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    ///     Limit of items per page
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}