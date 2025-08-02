using System.Text;
using System.Text.Json;
using zborek.Langfuse.Models;
using zborek.Langfuse.Services.Interfaces;

namespace zborek.Langfuse.Services;

public class DatasetRunItemService : IDatasetRunItemService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public DatasetRunItemService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<DatasetRunItem> CreateAsync(CreateDatasetRunItemRequest request, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/public/dataset-run-items", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to create dataset run item: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<DatasetRunItem>(responseContent, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<PaginatedDatasetRunItems> ListAsync(DatasetRunItemListRequest request, CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var response = await _httpClient.GetAsync($"/api/public/dataset-run-items{query}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to list dataset run items: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<PaginatedDatasetRunItems>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }
}