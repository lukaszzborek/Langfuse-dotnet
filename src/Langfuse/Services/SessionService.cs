using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

/// <summary>
///     Implementation of session service for Langfuse API
/// </summary>
internal class SessionService : ISessionService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseUpper) }
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<SessionService> _logger;

    /// <inheritdoc />
    public async Task<SessionListResponse> ListAsync(SessionListRequest? request = null,
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
            var result = JsonSerializer.Deserialize<SessionListResponse>(responseContent, SerializerOptions);

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
    public async Task<Session> GetAsync(string sessionId, CancellationToken cancellationToken = default)
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
            var result = JsonSerializer.Deserialize<Session>(responseContent, SerializerOptions);

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

    /// <summary>
    ///     Initializes a new instance of the SessionService class
    /// </summary>
    /// <param name="httpClient">HTTP client configured for Langfuse API</param>
    /// <param name="logger">Logger instance</param>
    public SessionService(HttpClient httpClient, ILogger<SessionService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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
            HttpStatusCode.NotFound => "The requested session was not found",
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