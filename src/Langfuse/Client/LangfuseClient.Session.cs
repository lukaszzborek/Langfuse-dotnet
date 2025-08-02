using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<SessionListResponse> GetSessionListAsync(SessionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/sessions{queryString}";

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching sessions from endpoint: {Endpoint}", endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<SessionListResponse>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize session list response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched {Count} sessions", result.Data.Length);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch sessions was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching sessions");
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred while fetching sessions", ex);
        }
    }

    /// <inheritdoc />
    public async Task<Session> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            throw new ArgumentException("Session ID cannot be null or empty", nameof(sessionId));
        }

        var endpoint = $"/api/public/sessions/{Uri.EscapeDataString(sessionId)}";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching session {SessionId} from endpoint: {Endpoint}", sessionId, endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<Session>(responseContent, _jsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize session response for ID: {sessionId}");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched session {SessionId} with {TraceCount} traces",
                    sessionId, result.Traces?.Length ?? 0);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch session {SessionId} was cancelled", sessionId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching session {SessionId}", sessionId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while fetching session {sessionId}", ex);
        }
    }
}