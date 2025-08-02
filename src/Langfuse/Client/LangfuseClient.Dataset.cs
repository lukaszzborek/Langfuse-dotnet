using System.Text;
using System.Text.Json;
using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<PaginatedDatasets> GetDatasetListAsync(DatasetListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var response = await _httpClient.GetAsync($"/api/public/v2/datasets{query}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to list datasets: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<PaginatedDatasets>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<Dataset> GetDatasetAsync(string datasetName, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/api/public/v2/datasets/{Uri.EscapeDataString(datasetName)}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get dataset: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Dataset>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<Dataset> CreateDatasetAsync(CreateDatasetRequest request,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/public/v2/datasets", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to create dataset: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<Dataset>(responseContent, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<DatasetRunWithItems> GetDatasetRunAsync(string datasetName, string runName,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync(
            $"/api/public/datasets/{Uri.EscapeDataString(datasetName)}/runs/{Uri.EscapeDataString(runName)}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get dataset run: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<DatasetRunWithItems>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<DeleteDatasetRunResponse> DeleteDatasetRunAsync(string datasetName, string runName,
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync(
            $"/api/public/datasets/{Uri.EscapeDataString(datasetName)}/runs/{Uri.EscapeDataString(runName)}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to delete dataset run: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<DeleteDatasetRunResponse>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<PaginatedDatasetRuns> GetDatasetRunsAsync(string datasetName, DatasetRunListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var response =
            await _httpClient.GetAsync($"/api/public/datasets/{Uri.EscapeDataString(datasetName)}/runs{query}",
                cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get dataset runs: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<PaginatedDatasetRuns>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }
}