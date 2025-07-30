using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Responses;

/// <summary>
///     Response model for getting media record information matching OpenAPI GetMediaResponse schema
/// </summary>
public class GetMediaResponse
{
    /// <summary>
    ///     The unique langfuse identifier of a media record
    /// </summary>
    [JsonPropertyName("mediaId")]
    public string MediaId { get; set; } = string.Empty;

    /// <summary>
    ///     The MIME type of the media record
    /// </summary>
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    ///     The size of the media record in bytes
    /// </summary>
    [JsonPropertyName("contentLength")]
    public int ContentLength { get; set; }

    /// <summary>
    ///     The date and time when the media record was uploaded
    /// </summary>
    [JsonPropertyName("uploadedAt")]
    public DateTimeOffset UploadedAt { get; set; }

    /// <summary>
    ///     The download URL of the media record
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    ///     The expiry date and time of the media record download URL
    /// </summary>
    [JsonPropertyName("urlExpiry")]
    public string UrlExpiry { get; set; } = string.Empty;
}