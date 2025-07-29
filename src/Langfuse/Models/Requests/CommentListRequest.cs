using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request parameters for listing comments
/// </summary>
public class CommentListRequest
{
    /// <summary>
    ///     Page number, starts at 1
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    ///     Limit of items per page
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    ///     Filter comments by object type (trace, observation, session, prompt)
    /// </summary>
    [JsonPropertyName("objectType")]
    public string? ObjectType { get; set; }

    /// <summary>
    ///     Filter comments by object id. If objectType is not provided, an error will be thrown
    /// </summary>
    [JsonPropertyName("objectId")]
    public string? ObjectId { get; set; }

    /// <summary>
    ///     Filter comments by author user id
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; set; }
}