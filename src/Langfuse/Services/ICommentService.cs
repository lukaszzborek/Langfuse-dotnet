using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Services;

/// <summary>
///     Service for interacting with Langfuse comment endpoints
/// </summary>
public interface ICommentService
{
    /// <summary>
    ///     Get all comments with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of comments with pagination metadata</returns>
    Task<GetCommentsResponse> ListAsync(CommentListRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Get a single comment by ID
    /// </summary>
    /// <param name="commentId">The unique identifier of the comment</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The comment details</returns>
    Task<Comment> GetAsync(string commentId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Create a new comment
    /// </summary>
    /// <param name="request">Comment creation details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created comment response</returns>
    Task<CreateCommentResponse> CreateAsync(CreateCommentRequest request, CancellationToken cancellationToken = default);
}