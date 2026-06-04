using zborek.Langfuse.Models.Metrics;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    [Obsolete("V1 metrics endpoint is legacy. Use GetMetricsV2Async for better performance.")]
    public async Task<MetricsResponse> GetMetricsAsync(MetricsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<MetricsResponse>($"/api/public/metrics{query}", "Get Metrics", cancellationToken);
    }
}
