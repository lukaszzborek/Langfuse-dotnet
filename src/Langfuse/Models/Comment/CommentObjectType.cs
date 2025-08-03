using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Comment;

/// <summary>
///     Defines the types of Langfuse objects that comments can be attached to for annotation and collaboration.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<CommentObjectType>))]
public enum CommentObjectType
{
    /// <summary>
    ///     Comment attached to a trace - used for annotating end-to-end execution flows and overall performance.
    /// </summary>
    Trace = 0,

    /// <summary>
    ///     Comment attached to an observation - used for annotating specific spans, generations, or events within a trace.
    /// </summary>
    Observation = 1,

    /// <summary>
    ///     Comment attached to a session - used for annotating user interaction flows and conversation contexts.
    /// </summary>
    Session = 2,

    /// <summary>
    ///     Comment attached to a prompt - used for annotating prompt templates and their versioning or effectiveness.
    /// </summary>
    Prompt = 3
}