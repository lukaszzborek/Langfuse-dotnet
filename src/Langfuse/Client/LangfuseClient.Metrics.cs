using zborek.Langfuse.Models.Metrics;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. Use GetMetricsV2Async (GET /api/public/v2/metrics) instead. Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
    public async Task<MetricsResponse> GetMetricsAsync(MetricsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<MetricsResponse>($"/api/public/metrics{query}", "Get Metrics", cancellationToken);
    }
}
