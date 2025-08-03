using zborek.Langfuse.Models.Session;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<SessionListResponse> GetSessionListAsync(SessionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<SessionListResponse>($"/api/public/sessions{queryString}", "Get Session List",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SessionModel> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
        }

        return await GetAsync<SessionModel>($"/api/public/sessions/{Uri.EscapeDataString(sessionId)}", "Get Session",
            cancellationToken);
    }
}