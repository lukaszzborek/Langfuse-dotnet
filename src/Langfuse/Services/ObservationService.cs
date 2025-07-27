using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

/// <summary>
///     Implementation of observation service for Langfuse API
/// </summary>
internal class ObservationService : IObservationService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseUpper) }
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<ObservationService> _logger;

    /// <summary>
    ///     Initializes a new instance of the ObservationService class
    /// </summary>
    /// <param name="httpClient">HTTP client configured for Langfuse API</param>
    /// <param name="logger">Logger instance</param>
    public ObservationService(HttpClient httpClient, ILogger<ObservationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ObservationListResponse> ListAsync(ObservationListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/observations{queryString}";

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching observations from endpoint: {Endpoint}", endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ObservationListResponse>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize observation list response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched {Count} observations", result.Data.Length);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch observations was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching observations");
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred while fetching observations", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Observation> GetAsync(string observationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(observationId))
        {
            throw new ArgumentException("Observation ID cannot be null or empty", nameof(observationId));
        }

        var endpoint = $"/api/public/observations/{Uri.EscapeDataString(observationId)}";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching observation {ObservationId} from endpoint: {Endpoint}", observationId, endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<Observation>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize observation response for ID: {observationId}");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched observation {ObservationId}", observationId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch observation {ObservationId} was cancelled", observationId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching observation {ObservationId}", observationId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while fetching observation {observationId}", ex);
        }
    }

    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        var statusCode = (int)response.StatusCode;

        var errorMessage = response.StatusCode switch
        {
            HttpStatusCode.NotFound => "The requested observation was not found",
            HttpStatusCode.Unauthorized => "Authentication failed. Please check your API credentials",
            HttpStatusCode.Forbidden => "Access forbidden. You don't have permission to access this resource",
            HttpStatusCode.TooManyRequests => "Rate limit exceeded. Please retry after some time",
            _ => $"API request failed with status code {statusCode}"
        };

        throw new LangfuseApiException(statusCode, errorMessage, details: new Dictionary<string, object>
        {
            ["responseContent"] = errorContent,
            ["statusCode"] = statusCode
        });
    }
}