using zborek.Langfuse.Models.Comment;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<GetCommentsResponse> GetCommentListAsync(CommentListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/comments{query}";
        return await GetAsync<GetCommentsResponse>(endpoint, "List Comments", cancellationToken);
    }

    public async Task<CommentModel> GetCommentAsync(string commentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(commentId))
        {
            throw new ArgumentException("Comment ID cannot be null or empty", nameof(commentId));
        }

        var endpoint = $"/api/public/comments/{Uri.EscapeDataString(commentId)}";
        return await GetAsync<CommentModel>(endpoint, "Get Comment", cancellationToken);
    }

    public async Task<CreateCommentResponse> CreateCommentAsync(CreateCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        const string endpoint = "/api/public/comments";
        return await PostAsync<CreateCommentResponse>(endpoint, request, "Create Comment", cancellationToken);
    }
}