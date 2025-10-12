using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Response after deleting an annotation queue assignment
/// </summary>
public class DeleteAnnotationQueueAssignmentResponse
{
    /// <summary>
    ///     Success indicator
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}