using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request model for updating an annotation queue item
/// </summary>
public class UpdateAnnotationQueueItemRequest
{
    /// <summary>
    ///     Updated status of the annotation item
    /// </summary>
    [JsonPropertyName("status")]
    public AnnotationQueueStatus? Status { get; set; }
}