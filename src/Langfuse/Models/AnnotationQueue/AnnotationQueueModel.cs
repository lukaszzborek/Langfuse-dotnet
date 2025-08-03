using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Represents an annotation queue in Langfuse
/// </summary>
public class AnnotationQueueModel
{
    /// <summary>
    ///     Unique identifier of the annotation queue
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the annotation queue
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the annotation queue
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Array of score configuration IDs associated with this queue
    /// </summary>
    [JsonPropertyName("scoreConfigIds")]
    public string[] ScoreConfigIds { get; set; } = [];

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