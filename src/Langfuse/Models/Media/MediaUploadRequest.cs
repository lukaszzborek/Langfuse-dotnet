using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Media;

/// <summary>
///     Request to get a presigned upload URL for media files. Enables secure file uploads to Langfuse for multimodal AI
///     applications and trace attachments.
/// </summary>
public class MediaUploadRequest
{
    /// <summary>
    ///     The trace ID associated with the media record
    /// </summary>
    [Required]
    [JsonPropertyName("traceId")]
    public required string TraceId { get; set; }

    /// <summary>
    ///     The observation ID associated with the media record. If the media record is associated directly with a trace, this
    ///     will be null.
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     MIME type of the media file
    /// </summary>
    [Required]
    [JsonPropertyName("contentType")]
    public required string ContentType { get; set; }

    /// <summary>
    ///     Size of the media file in bytes
    /// </summary>
    [Required]
    [JsonPropertyName("contentLength")]
    public required int ContentLength { get; set; }

    /// <summary>
    ///     The SHA-256 hash of the media record
    /// </summary>
    [Required]
    [JsonPropertyName("sha256Hash")]
    public required string Sha256Hash { get; set; }

    /// <summary>
    ///     The trace / observation field the media record is associated with. This can be one of `input`, `output`, `metadata`
    /// </summary>
    [Required]
    [JsonPropertyName("field")]
    public required string Field { get; set; }
}