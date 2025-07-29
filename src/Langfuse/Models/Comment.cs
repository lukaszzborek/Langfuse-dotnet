using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a comment in Langfuse
/// </summary>
public class Comment
{
    /// <summary>
    ///     The unique identifier of the comment
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     The project ID the comment belongs to
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    ///     When the comment was created
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     When the comment was last updated
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     The type of object this comment is attached to
    /// </summary>
    [JsonPropertyName("objectType")]
    public CommentObjectType ObjectType { get; set; }

    /// <summary>
    ///     The ID of the object this comment is attached to
    /// </summary>
    [JsonPropertyName("objectId")]
    public string ObjectId { get; set; } = string.Empty;

    /// <summary>
    ///     The content of the comment (may include markdown)
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    ///     The user ID of the comment author
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; set; }
}

/// <summary>
///     The type of object a comment can be attached to
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CommentObjectType
{
    /// <summary>
    ///     Comment attached to a trace
    /// </summary>
    [JsonStringEnumMemberName("TRACE")]
    Trace,

    /// <summary>
    ///     Comment attached to an observation
    /// </summary>
    [JsonStringEnumMemberName("OBSERVATION")]
    Observation,

    /// <summary>
    ///     Comment attached to a session
    /// </summary>
    [JsonStringEnumMemberName("SESSION")]
    Session,

    /// <summary>
    ///     Comment attached to a prompt
    /// </summary>
    [JsonStringEnumMemberName("PROMPT")]
    Prompt
}