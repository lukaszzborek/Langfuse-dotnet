using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Annotation object type enumeration - matches OpenAPI spec
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<AnnotationObjectType>))]
public enum AnnotationObjectType
{
    /// <summary>
    ///     Trace object
    /// </summary>
    Trace,

    /// <summary>
    ///     Observation object (span or generation)
    /// </summary>
    Observation,

    /// <summary>
    ///     Session object
    /// </summary>
    Session
}