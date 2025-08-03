using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Comment;

/// <summary>
///     Response from getting comments
/// </summary>
public class GetCommentsResponse : PaginatedResponse<CommentModel>
{
}