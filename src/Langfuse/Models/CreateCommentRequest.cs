using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Request to create a new comment
/// </summary>
public class CreateCommentRequest
{
    /// <summary>
    ///     The id of the project to attach the comment to
    /// </summary>
    [JsonPropertyName("projectId")]
    public required string ProjectId { get; set; }

    /// <summary>
    ///     The type of the object to attach the comment to (trace, observation, session, prompt)
    /// </summary>
    [JsonPropertyName("objectType")]
    public required CommentObjectType ObjectType { get; set; }

    /// <summary>
    ///     The id of the object to attach the comment to. If this does not reference a valid existing object, an error will be thrown
    /// </summary>
    [JsonPropertyName("objectId")]
    public required string ObjectId { get; set; } = string.Empty;

    /// <summary>
    ///     The content of the comment. May include markdown. Currently limited to 3000 characters
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; set; } = string.Empty;

    /// <summary>
    ///     The id of the user who created the comment
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; set; }
}