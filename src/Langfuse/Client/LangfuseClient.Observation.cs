using zborek.Langfuse.Models.Observation;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. Use GetObservationsV2Async (GET /api/public/v2/observations) instead. Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
    public async Task<ObservationListResponse> GetObservationListAsync(ObservationListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<ObservationListResponse>($"/api/public/observations{queryString}", "Get Observation List",
            cancellationToken);
    }

    /// <inheritdoc />
    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. Use GetObservationsV2Async (GET /api/public/v2/observations) instead. Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
    public async Task<ObservationModel> GetObservationAsync(string observationId, DateTime? startTime = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(observationId))
        {
            throw new ArgumentException("Observation ID cannot be null or empty", nameof(observationId));
        }

        var endpoint = $"/api/public/observations/{Uri.EscapeDataString(observationId)}";
        if (startTime.HasValue)
        {
            endpoint += $"?startTime={Uri.EscapeDataString(startTime.Value.ToString("O"))}";
        }

        return await GetAsync<ObservationModel>(endpoint, "Get Observation", cancellationToken);
    }
}
