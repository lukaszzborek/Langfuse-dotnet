using System.Text;
using System.Text.Json;
using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<GetCommentsResponse> GetCommentListAsync(CommentListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var response = await _httpClient.GetAsync($"/api/public/comments{query}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to list comments: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<GetCommentsResponse>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<Comment> GetCommentAsync(string commentId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/public/comments/{Uri.EscapeDataString(commentId)}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get comment: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Comment>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<CreateCommentResponse> CreateCommentAsync(CreateCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/public/comments", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to create comment: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<CreateCommentResponse>(responseContent, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }
}