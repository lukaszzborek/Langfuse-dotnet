using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<PaginatedDatasets> GetDatasetListAsync(DatasetListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/v2/datasets{query}";
        return await GetAsync<PaginatedDatasets>(endpoint, "List Datasets", cancellationToken);
    }

    public async Task<DatasetModel> GetDatasetAsync(string datasetName, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(datasetName))
        {
            throw new ArgumentException("Dataset name cannot be null or empty", nameof(datasetName));
        }

        var endpoint = $"/api/public/v2/datasets/{Uri.EscapeDataString(datasetName)}";
        return await GetAsync<DatasetModel>(endpoint, "Get Dataset", cancellationToken);
    }

    public async Task<DatasetModel> CreateDatasetAsync(CreateDatasetRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        const string endpoint = "/api/public/v2/datasets";
        return await PostAsync<DatasetModel>(endpoint, request, "Create Dataset", cancellationToken);
    }
}