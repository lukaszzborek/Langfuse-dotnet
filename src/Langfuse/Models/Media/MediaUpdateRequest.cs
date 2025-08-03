using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Media;

/// <summary>
///     Request model for updating media record metadata
/// </summary>
public class MediaUpdateRequest
{
    /// <summary>
    ///     Updated filename (optional)
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>
    ///     SHA-256 hash of the media content (optional)
    /// </summary>
    [JsonPropertyName("sha256Hash")]
    public string? Sha256Hash { get; set; }

    /// <summary>
    ///     Upload status update (optional)
    /// </summary>
    [JsonPropertyName("uploadStatus")]
    public MediaUploadStatus? UploadStatus { get; set; }
}