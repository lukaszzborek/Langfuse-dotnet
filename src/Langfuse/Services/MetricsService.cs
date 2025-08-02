using System.Text.Json;
using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Services.Interfaces;

namespace zborek.Langfuse.Services;

/// <summary>
///     Implementation of metrics service for Langfuse API
/// </summary>
public class MetricsService : IMetricsService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    ///     Initializes a new instance of the MetricsService class
    /// </summary>
    /// <param name="httpClient">HTTP client configured for Langfuse API</param>
    public MetricsService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <inheritdoc />
    public async Task<MetricsResponse> GetMetricsAsync(MetricsRequest request, CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var response = await _httpClient.GetAsync($"/api/public/metrics{query}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get metrics: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<MetricsResponse>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    /// <summary>
    ///     Helper method to serialize a MetricsQuery to JSON string for use in requests
    /// </summary>
    /// <param name="query">The metrics query object</param>
    /// <returns>JSON string representation of the query</returns>
    public string SerializeQuery(MetricsQuery query)
    {
        return JsonSerializer.Serialize(query, _jsonOptions);
    }
}