using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Responses;

/// <summary>
///     Response model for annotation queue item deletion requests
/// </summary>
public class DeleteAnnotationQueueItemResponse
{
    /// <summary>
    ///     Indicates whether the deletion was successful
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    ///     Optional message about the deletion operation
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}