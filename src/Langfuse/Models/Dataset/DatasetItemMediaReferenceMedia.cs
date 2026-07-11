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
    public required string MediaId { get; init; }

    /// <summary>
    ///     The MIME type of the media record.
    /// </summary>
    [JsonPropertyName("contentType")]
    public required string ContentType { get; init; }

    /// <summary>
    ///     The size of the media record in bytes.
    /// </summary>
    [JsonPropertyName("contentLength")]
    public required int ContentLength { get; init; }

    /// <summary>
    ///     The signed download URL of the media record.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>
    ///     The expiry date and time of the download URL.
    /// </summary>
    [JsonPropertyName("urlExpiry")]
    public required string UrlExpiry { get; init; }
}