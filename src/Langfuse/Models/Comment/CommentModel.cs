using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Comment;

/// <summary>
///     Represents a comment in Langfuse - a text annotation that can be attached to traces, observations, sessions, or
///     prompts.
///     Comments enable collaboration and provide context for analysis, debugging, and quality assessment.
/// </summary>
public class CommentModel
{
    /// <summary>
    ///     The unique identifier of the comment
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     The project ID the comment belongs to, providing organizational scope and access control.
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the comment was originally created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the comment was last updated or modified.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     The type of Langfuse object this comment is attached to (trace, observation, session, or prompt).
    /// </summary>
    [JsonPropertyName("objectType")]
    public CommentObjectType ObjectType { get; set; }

    /// <summary>
    ///     The unique identifier of the Langfuse object this comment is attached to. Must reference a valid existing object.
    /// </summary>
    [JsonPropertyName("objectId")]
    public string ObjectId { get; set; } = string.Empty;

    /// <summary>
    ///     The text content of the comment. Supports Markdown formatting and is currently limited to 3000 characters.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    ///     The user ID of the person who authored this comment, used for attribution and audit trails.
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; set; }
}