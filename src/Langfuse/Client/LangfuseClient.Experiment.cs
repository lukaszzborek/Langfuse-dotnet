using zborek.Langfuse.Models.Experiment;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<ExperimentsResponse> GetExperimentsAsync(
        ExperimentListRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/experiments{queryString}";
        return await GetAsync<ExperimentsResponse>(endpoint, "Get Experiments", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExperimentItemsResponse> GetExperimentItemsAsync(
        ExperimentItemListRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/experiment-items{queryString}";
        return await GetAsync<ExperimentItemsResponse>(endpoint, "Get Experiment Items", cancellationToken);
    }
}