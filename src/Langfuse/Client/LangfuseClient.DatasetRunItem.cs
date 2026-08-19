using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. In Langfuse v4, create experiment data via the SDK experiment runner or the OpenTelemetry endpoint (POST /api/public/otel/v1/traces). Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
    public async Task<DatasetRunItem> CreateDataSetRunAsync(CreateDatasetRunItemRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<DatasetRunItem>("/api/public/dataset-run-items", request, "Create Dataset Run Item",
            cancellationToken);
    }

    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. In Langfuse v4, dataset run items are replaced by experiment items; use GetExperimentItemsAsync (GET /api/public/experiment-items) instead. Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
    public async Task<PaginatedDatasetRunItems> GetDatasetRunListAsync(DatasetRunItemListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<PaginatedDatasetRunItems>($"/api/public/dataset-run-items{query}",
            "Get Dataset Run Item List", cancellationToken);
    }
}