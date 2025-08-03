using zborek.Langfuse.Models.Observation;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<ObservationListResponse> GetObservationListAsync(ObservationListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<ObservationListResponse>($"/api/public/observations{queryString}", "Get Observation List",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ObservationModel> GetObservationAsync(string observationId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(observationId))
        {
            throw new ArgumentException("Observation ID cannot be null or empty", nameof(observationId));
        }

        return await GetAsync<ObservationModel>($"/api/public/observations/{Uri.EscapeDataString(observationId)}",
            "Get Observation", cancellationToken);
    }
}