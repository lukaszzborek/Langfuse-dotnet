using System.Text;
using System.Text.Json;
using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<DatasetItem> CreateDatasetItemAsync(CreateDatasetItemRequest request,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/public/dataset-items", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to create dataset item: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<DatasetItem>(responseContent, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<PaginatedDatasetItems> GetDatasetItemListAsync(DatasetItemListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var response = await _httpClient.GetAsync($"/api/public/dataset-items{query}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to list dataset items: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<PaginatedDatasetItems>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<DatasetItem> GetDatasetItemAsync(string id, CancellationToken cancellationToken = default)
    {
        var response =
            await _httpClient.GetAsync($"/api/public/dataset-items/{Uri.EscapeDataString(id)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get dataset item: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<DatasetItem>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<DeleteDatasetItemResponse> DeleteDatasetItemAsync(string id,
        CancellationToken cancellationToken = default)
    {
        var response =
            await _httpClient.DeleteAsync($"/api/public/dataset-items/{Uri.EscapeDataString(id)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to delete dataset item: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<DeleteDatasetItemResponse>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }
}