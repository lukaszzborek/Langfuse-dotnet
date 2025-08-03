using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<DatasetItem> CreateDatasetItemAsync(CreateDatasetItemRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<DatasetItem>("/api/public/dataset-items", request, "Create Dataset Item",
            cancellationToken);
    }

    public async Task<PaginatedDatasetItems> GetDatasetItemListAsync(DatasetItemListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<PaginatedDatasetItems>($"/api/public/dataset-items{query}", "Get Dataset Item List",
            cancellationToken);
    }

    public async Task<DatasetItem> GetDatasetItemAsync(string id, CancellationToken cancellationToken = default)
    {
        return await GetAsync<DatasetItem>($"/api/public/dataset-items/{Uri.EscapeDataString(id)}", "Get Dataset Item",
            cancellationToken);
    }

    public async Task<DeleteDatasetItemResponse> DeleteDatasetItemAsync(string id,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Dataset item ID cannot be null or empty", nameof(id));
        }

        var endpoint = $"/api/public/dataset-items/{Uri.EscapeDataString(id)}";
        return await DeleteAsync<DeleteDatasetItemResponse>(endpoint, "Delete Dataset Item", cancellationToken);
    }
}