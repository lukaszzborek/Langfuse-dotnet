using zborek.Langfuse.Models.ObservationV2;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<ObservationsV2Response> GetObservationsV2Async(ObservationsV2Request? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<ObservationsV2Response>($"/api/public/v2/observations{queryString}",
            "Get Observations V2", cancellationToken);
    }
}
