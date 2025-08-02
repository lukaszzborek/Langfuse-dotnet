using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;
using zborek.Langfuse.Services.Interfaces;

namespace zborek.Langfuse.Services;

/// <summary>
///     Implementation of score configuration service for Langfuse API
/// </summary>
internal class ScoreConfigService : IScoreConfigService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseUpper) }
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<ScoreConfigService> _logger;

    /// <summary>
    ///     Initializes a new instance of the ScoreConfigService class
    /// </summary>
    /// <param name="httpClient">HTTP client configured for Langfuse API</param>
    /// <param name="logger">Logger instance</param>
    public ScoreConfigService(HttpClient httpClient, ILogger<ScoreConfigService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ScoreConfigListResponse> ListAsync(ScoreConfigListRequest? request = null,
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
            var result = JsonSerializer.Deserialize<ScoreConfigListResponse>(responseContent, SerializerOptions);

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
    public async Task<ScoreConfig> GetAsync(string configId, CancellationToken cancellationToken = default)
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
            var result = JsonSerializer.Deserialize<ScoreConfig>(responseContent, SerializerOptions);

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
    public async Task<ScoreConfig> CreateAsync(CreateScoreConfigRequest request,
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
            var json = JsonSerializer.Serialize(request, SerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ScoreConfig>(responseContent, SerializerOptions);

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
            HttpStatusCode.NotFound => "The requested score configuration was not found",
            HttpStatusCode.Unauthorized => "Authentication failed. Please check your API credentials",
            HttpStatusCode.Forbidden => "Access forbidden. You don't have permission to access this resource",
            HttpStatusCode.TooManyRequests => "Rate limit exceeded. Please retry after some time",
            HttpStatusCode.BadRequest => "Invalid request. Please check your request parameters",
            HttpStatusCode.Conflict => "Score configuration already exists or conflict with existing data",
            _ => $"API request failed with status code {statusCode}"
        };

        throw new LangfuseApiException(statusCode, errorMessage, details: new Dictionary<string, object>
        {
            ["responseContent"] = errorContent,
            ["statusCode"] = statusCode
        });
    }
}