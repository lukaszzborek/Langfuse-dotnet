using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Request model for creating a new annotation queue item
/// </summary>
public class CreateAnnotationQueueItemRequest
{
    /// <summary>
    ///     ID of the object being annotated (trace, observation, etc.)
    /// </summary>
    [Required]
    [JsonPropertyName("objectId")]
    public string ObjectId { get; set; } = string.Empty;

    /// <summary>
    ///     Type of the object being annotated
    /// </summary>
    [Required]
    [JsonPropertyName("objectType")]
    public AnnotationObjectType ObjectType { get; set; }

    /// <summary>
    ///     Status of the annotation item (defaults to PENDING for new queue items)
    /// </summary>
    [JsonPropertyName("status")]
    public AnnotationQueueStatus? Status { get; set; }
}