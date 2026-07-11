using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     The resolved media record referenced by a dataset item media reference.
/// </summary>
public class DatasetItemMediaReferenceMedia
{
    /// <summary>
    ///     The unique langfuse identifier of the media record.
    /// </summary>
    [JsonPropertyName("mediaId")]
    public string MediaId { get; set; } = string.Empty;

    /// <summary>
    ///     The MIME type of the media record.
    /// </summary>
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    ///     The size of the media record in bytes.
    /// </summary>
    [JsonPropertyName("contentLength")]
    public int ContentLength { get; set; }

    /// <summary>
    ///     The signed download URL of the media record.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    ///     The expiry date and time of the download URL.
    /// </summary>
    [JsonPropertyName("urlExpiry")]
    public string UrlExpiry { get; set; } = string.Empty;
}