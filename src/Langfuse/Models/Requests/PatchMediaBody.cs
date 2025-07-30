using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request model for patching media record matching OpenAPI PatchMediaBody schema
/// </summary>
public class PatchMediaBody
{
    /// <summary>
    ///     The date and time when the media record was uploaded
    /// </summary>
    [Required]
    [JsonPropertyName("uploadedAt")]
    public required DateTimeOffset UploadedAt { get; set; }

    /// <summary>
    ///     The HTTP status code of the upload
    /// </summary>
    [Required]
    [JsonPropertyName("uploadHttpStatus")]
    public required int UploadHttpStatus { get; set; }

    /// <summary>
    ///     The HTTP error message of the upload
    /// </summary>
    [JsonPropertyName("uploadHttpError")]
    public string? UploadHttpError { get; set; }

    /// <summary>
    ///     The time in milliseconds it took to upload the media record
    /// </summary>
    [JsonPropertyName("uploadTimeMs")]
    public int? UploadTimeMs { get; set; }
}