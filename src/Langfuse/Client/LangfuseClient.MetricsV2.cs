using System.Text.Json;
using zborek.Langfuse.Models.MetricsV2;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<MetricsV2Response> GetMetricsV2Async(MetricsV2Request request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Serialize the request to JSON and pass it as a query parameter
        var queryJson = JsonSerializer.Serialize(request, JsonOptions);
        var encodedQuery = Uri.EscapeDataString(queryJson);
        var endpoint = $"/api/public/v2/metrics?query={encodedQuery}";

        return await GetAsync<MetricsV2Response>(endpoint, "Get Metrics V2", cancellationToken);
    }
}
