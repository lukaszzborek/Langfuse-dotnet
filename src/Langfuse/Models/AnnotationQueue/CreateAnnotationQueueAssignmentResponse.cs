using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Response after creating an annotation queue assignment
/// </summary>
public class CreateAnnotationQueueAssignmentResponse
{
    /// <summary>
    ///     User ID
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    ///     Queue ID
    /// </summary>
    [JsonPropertyName("queueId")]
    public string QueueId { get; set; } = string.Empty;

    /// <summary>
    ///     Project ID
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;
}