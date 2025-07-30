using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request parameters for listing annotation queues
/// </summary>
public class AnnotationQueueListRequest
{
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