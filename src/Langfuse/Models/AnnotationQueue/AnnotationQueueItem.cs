using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Represents an annotation queue item in Langfuse
/// </summary>
public class AnnotationQueueItem
{
    /// <summary>
    ///     Unique identifier of the annotation queue item
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the annotation queue this item belongs to
    /// </summary>
    [JsonPropertyName("queueId")]
    public string QueueId { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the object being annotated (trace, observation, etc.)
    /// </summary>
    [JsonPropertyName("objectId")]
    public string ObjectId { get; set; } = string.Empty;

    /// <summary>
    ///     Type of the object being annotated
    /// </summary>
    [JsonPropertyName("objectType")]
    public AnnotationObjectType ObjectType { get; set; }

    /// <summary>
    ///     Current status of the annotation item
    /// </summary>
    [JsonPropertyName("status")]
    public AnnotationQueueStatus Status { get; set; }

    /// <summary>
    ///     Completion timestamp
    /// </summary>
    [JsonPropertyName("completedAt")]
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    ///     Creation timestamp
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Last update timestamp
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }
}