using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Request to create a new comment in Langfuse. Comments provide a way to annotate and collaborate on traces, observations, sessions, and prompts.
/// </summary>
public class CreateCommentRequest
{
    /// <summary>
    ///     The unique identifier of the project where the comment should be created. Determines access scope and organization.
    /// </summary>
    [JsonPropertyName("projectId")]
    public required string ProjectId { get; set; }

    /// <summary>
    ///     The type of Langfuse object to attach the comment to. Must be one of: trace, observation, session, or prompt.
    /// </summary>
    [JsonPropertyName("objectType")]
    public required CommentObjectType ObjectType { get; set; }

    /// <summary>
    ///     The unique identifier of the object to attach the comment to. Must reference a valid existing object or an error will be thrown.
    /// </summary>
    [JsonPropertyName("objectId")]
    public required string ObjectId { get; set; } = string.Empty;

    /// <summary>
    ///     The text content of the comment. Supports Markdown formatting for rich text. Currently limited to 3000 characters maximum.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; } = string.Empty;

    /// <summary>
    ///     Optional user ID of the comment author. Used for attribution and identifying who created the comment.
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; set; }
}