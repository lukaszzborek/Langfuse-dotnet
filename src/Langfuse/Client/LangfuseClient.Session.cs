using zborek.Langfuse.Models.Session;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. In Langfuse v4, read session data via GetObservationsV2Async with a sessionId filter. Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
    public async Task<SessionListResponse> GetSessionListAsync(SessionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<SessionListResponse>($"/api/public/sessions{queryString}", "Get Session List",
            cancellationToken);
    }

    /// <inheritdoc />
    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. In Langfuse v4, read session data via GetObservationsV2Async with a sessionId filter. Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
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