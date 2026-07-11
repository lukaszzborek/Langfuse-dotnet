using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Media;

/// <summary>
///     Request to get a presigned upload URL for media files. Enables secure file uploads to Langfuse for multimodal AI
///     applications. Provide exactly one context: a trace (<see cref="TraceId" />, optionally
///     <see cref="ObservationId" />) or a dataset item (<see cref="DatasetId" /> + <see cref="DatasetItemId" />).
///     <see cref="Field" /> is required and must match the chosen context.
/// </summary>
public class MediaUploadRequest
{
    /// <summary>
    ///     The trace the media is associated with. Null for dataset item media uploads.
    /// </summary>
    [JsonPropertyName("traceId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; set; }

    /// <summary>
    ///     The observation ID associated with the media record. If the media record is associated directly with a trace, this
    ///     will be null.
    /// </summary>
    [JsonPropertyName("observationId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     The dataset the media belongs to. Null for trace/observation media uploads.
    /// </summary>
    [JsonPropertyName("datasetId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DatasetId { get; set; }

    /// <summary>
    ///     The dataset item the media is associated with (need not exist yet). Null for trace/observation media uploads.
    /// </summary>
    [JsonPropertyName("datasetItemId")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DatasetItemId { get; set; }

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
    ///     The item field the media is in: `input`/`output`/`metadata` (trace) or `input`/`expectedOutput`/`metadata`
    ///     (dataset item)
    /// </summary>
    [Required]
    [JsonPropertyName("field")]
    public required string Field { get; set; }
}