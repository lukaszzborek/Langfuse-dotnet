using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Request to create an annotation queue
/// </summary>
public class CreateAnnotationQueueRequest
{
    /// <summary>
    ///     Name of the annotation queue
    /// </summary>
    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the annotation queue
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     IDs of score configurations associated with this queue
    /// </summary>
    [Required]
    [JsonPropertyName("scoreConfigIds")]
    public string[] ScoreConfigIds { get; set; } = [];
}
