using System.Text.Json;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Metrics;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<MetricsResponse> GetMetricsAsync(MetricsRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var response = await _httpClient.GetAsync($"/api/public/metrics{query}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get metrics: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MetricsResponse>(content, JsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }
}