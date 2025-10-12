using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Request to create or delete a user assignment to an annotation queue
/// </summary>
public class AnnotationQueueAssignmentRequest
{
    /// <summary>
    ///     User ID to assign or unassign
    /// </summary>
    [Required]
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
}
