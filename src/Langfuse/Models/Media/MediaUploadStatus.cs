using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Media;

/// <summary>
///     Media upload status enumeration
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<MediaUploadStatus>))]
public enum MediaUploadStatus
{
    /// <summary>
    ///     Upload is pending
    /// </summary>
    Pending,

    /// <summary>
    ///     Upload completed successfully
    /// </summary>
    Completed,

    /// <summary>
    ///     Upload failed
    /// </summary>
    Failed
}