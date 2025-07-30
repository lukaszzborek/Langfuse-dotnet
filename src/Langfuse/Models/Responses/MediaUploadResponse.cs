using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Responses;

/// <summary>
///     Response model for media upload requests matching OpenAPI GetMediaUploadUrlResponse schema
/// </summary>
public class MediaUploadResponse
{
    /// <summary>
    ///     The presigned upload URL. If the asset is already uploaded, this will be null
    /// </summary>
    [JsonPropertyName("uploadUrl")]
    public string? UploadUrl { get; set; }

    /// <summary>
    ///     The unique langfuse identifier of a media record
    /// </summary>
    [JsonPropertyName("mediaId")]
    public string MediaId { get; set; } = string.Empty;
}