using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<ScoreListResponse> GetScoreListAsync(ScoreListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/v2/scores{queryString}";

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching scores from endpoint: {Endpoint}", endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ScoreListResponse>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize score list response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched {Count} scores", result.Data.Length);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch scores was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching scores");
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred while fetching scores", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Score> GetScoreAsync(string scoreId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scoreId))
        {
            throw new ArgumentException("Score ID cannot be null or empty", nameof(scoreId));
        }

        var endpoint = $"/api/public/v2/scores/{Uri.EscapeDataString(scoreId)}";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching score {ScoreId} from endpoint: {Endpoint}", scoreId, endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<Score>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize score response for ID: {scoreId}");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched score {ScoreId}", scoreId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch score {ScoreId} was cancelled", scoreId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching score {ScoreId}", scoreId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while fetching score {scoreId}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Score> CreateScoreAsync(ScoreCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Score name cannot be null or empty", nameof(request));
        }

        if (request.Value == null)
        {
            throw new ArgumentException("Score value cannot be null", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.TraceId) &&
            string.IsNullOrWhiteSpace(request.ObservationId) &&
            string.IsNullOrWhiteSpace(request.SessionId))
        {
            throw new ArgumentException("At least one of TraceId, ObservationId, or SessionId must be provided",
                nameof(request));
        }

        const string endpoint = "/api/public/scores";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Creating score with name {ScoreName} at endpoint: {Endpoint}", request.Name, endpoint);
        }

        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<Score>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize created score response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully created score {ScoreId} with name {ScoreName}", result.Id, request.Name);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to create score was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating score with name {ScoreName}", request.Name);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while creating score with name {request.Name}", ex);
        }
    }

    /// <inheritdoc />
    public async Task DeleteScoreAsync(string scoreId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scoreId))
        {
            throw new ArgumentException("Score ID cannot be null or empty", nameof(scoreId));
        }

        var endpoint = $"/api/public/scores/{Uri.EscapeDataString(scoreId)}";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Deleting score {ScoreId} at endpoint: {Endpoint}", scoreId, endpoint);
        }

        try
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully deleted score {ScoreId}", scoreId);
            }
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to delete score {ScoreId} was cancelled", scoreId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting score {ScoreId}", scoreId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while deleting score {scoreId}", ex);
        }
    }
}