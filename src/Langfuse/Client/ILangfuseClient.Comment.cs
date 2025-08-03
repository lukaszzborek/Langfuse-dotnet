using zborek.Langfuse.Models.Comment;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Get all comments with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of comments with pagination metadata</returns>
    Task<GetCommentsResponse> GetCommentListAsync(CommentListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get a single comment by ID
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The comment details</returns>
    Task<CommentModel> GetCommentAsync(string commentId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new comment
    /// </summary>
    /// <param name="request">Comment creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created comment response</returns>
    Task<CreateCommentResponse> CreateCommentAsync(CreateCommentRequest request,
        CancellationToken cancellationToken = default);
}