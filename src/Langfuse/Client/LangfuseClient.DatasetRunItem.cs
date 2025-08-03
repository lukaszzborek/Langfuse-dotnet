using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<DatasetRunItem> CreateDataSetRunAsync(CreateDatasetRunItemRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<DatasetRunItem>("/api/public/dataset-run-items", request, "Create Dataset Run Item",
            cancellationToken);
    }

    public async Task<PaginatedDatasetRunItems> GetDatasetRunListAsync(DatasetRunItemListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<PaginatedDatasetRunItems>($"/api/public/dataset-run-items{query}",
            "Get Dataset Run Item List", cancellationToken);
    }
}