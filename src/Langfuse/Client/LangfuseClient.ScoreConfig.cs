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
    public async Task<ScoreConfigListResponse> GetScoreConfigListAsync(ScoreConfigListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/score-configs{queryString}";

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching score configurations from endpoint: {Endpoint}", endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ScoreConfigListResponse>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize score configuration list response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched {Count} score configurations", result.Data.Length);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch score configurations was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching score configurations");
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred while fetching score configurations", ex);
        }
    }

    /// <inheritdoc />
    public async Task<ScoreConfig> GetScoreConfigAsync(string configId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(configId))
        {
            throw new ArgumentException("Score configuration ID cannot be null or empty", nameof(configId));
        }

        var endpoint = $"/api/public/score-configs/{Uri.EscapeDataString(configId)}";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching score configuration {ConfigId} from endpoint: {Endpoint}", configId, endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ScoreConfig>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize score configuration response for ID: {configId}");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched score configuration {ConfigId}", configId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch score configuration {ConfigId} was cancelled", configId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching score configuration {ConfigId}", configId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while fetching score configuration {configId}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<ScoreConfig> CreateScoreConfigAsync(CreateScoreConfigRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Score configuration name cannot be null or empty", nameof(request));
        }

        const string endpoint = "/api/public/score-configs";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Creating score configuration with name {ConfigName} at endpoint: {Endpoint}",
                request.Name, endpoint);
        }

        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ScoreConfig>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize created score configuration response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully created score configuration {ConfigId} with name {ConfigName}",
                    result.Id, request.Name);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to create score configuration was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating score configuration with name {ConfigName}",
                request.Name);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while creating score configuration with name {request.Name}", ex);
        }
    }
}