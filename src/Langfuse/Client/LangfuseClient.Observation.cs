using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<ObservationListResponse> GetObservationListAsync(ObservationListRequest? request = null,
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
            var result = JsonSerializer.Deserialize<ObservationListResponse>(responseContent, _jsonOptions);

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
    public async Task<Observation> GetObservationAsync(string observationId,
        CancellationToken cancellationToken = default)
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
            var result = JsonSerializer.Deserialize<Observation>(responseContent, _jsonOptions);

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
}