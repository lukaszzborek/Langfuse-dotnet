using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Annotation queue status enumeration - matches OpenAPI spec
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<AnnotationQueueStatus>))]
public enum AnnotationQueueStatus
{
    /// <summary>
    ///     Queue item is pending
    /// </summary>
    Pending,

    /// <summary>
    ///     Queue item is completed
    /// </summary>
    Completed
}