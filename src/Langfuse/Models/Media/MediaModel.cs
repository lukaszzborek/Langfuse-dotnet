using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Media;

/// <summary>
///     Represents a media record in Langfuse - a file attachment associated with traces or observations for multimodal AI
///     applications.
///     Media records enable storing and referencing images, audio, documents, and other files within the observability
///     system.
/// </summary>
public class MediaModel
{
    /// <summary>
    ///     Unique identifier of the media record
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Original filename of the media
    /// </summary>
    [JsonPropertyName("filename")]
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    ///     MIME type of the media file
    /// </summary>
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    ///     Size of the media file in bytes
    /// </summary>
    [JsonPropertyName("contentLength")]
    public long ContentLength { get; set; }

    /// <summary>
    ///     URL to access the media file
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>
    ///     SHA-256 hash of the media content
    /// </summary>
    [JsonPropertyName("sha256Hash")]
    public string? Sha256Hash { get; set; }

    /// <summary>
    ///     Upload status of the media
    /// </summary>
    [JsonPropertyName("uploadStatus")]
    public MediaUploadStatus UploadStatus { get; set; }

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

    /// <summary>
    ///     Project ID this media belongs to
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    ///     User ID of the uploader
    /// </summary>
    [JsonPropertyName("uploadedByUserId")]
    public string? UploadedByUserId { get; set; }
}