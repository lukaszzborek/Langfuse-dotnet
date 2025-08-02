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
///     Implementation of media service for Langfuse API
/// </summary>
internal class MediaService : IMediaService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseUpper) }
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<MediaService> _logger;

    /// <summary>
    ///     Initializes a new instance of the MediaService class
    /// </summary>
    /// <param name="httpClient">HTTP client configured for Langfuse API</param>
    /// <param name="logger">Logger instance</param>
    public MediaService(HttpClient httpClient, ILogger<MediaService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<GetMediaResponse> GetAsync(string mediaId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
        {
            throw new ArgumentException("Media ID cannot be null or empty", nameof(mediaId));
        }

        var endpoint = $"/api/public/media/{Uri.EscapeDataString(mediaId)}";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching media {MediaId} from endpoint: {Endpoint}", mediaId, endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<GetMediaResponse>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize media response for ID: {mediaId}");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched media {MediaId}", mediaId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch media {MediaId} was cancelled", mediaId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching media {MediaId}", mediaId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while fetching media {mediaId}", ex);
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(string mediaId, PatchMediaBody request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(mediaId))
        {
            throw new ArgumentException("Media ID cannot be null or empty", nameof(mediaId));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"/api/public/media/{Uri.EscapeDataString(mediaId)}";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Updating media {MediaId} at endpoint: {Endpoint}", mediaId, endpoint);
        }

        try
        {
            var json = JsonSerializer.Serialize(request, SerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully updated media {MediaId}", mediaId);
            }
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to update media {MediaId} was cancelled", mediaId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating media {MediaId}", mediaId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while updating media {mediaId}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<MediaUploadResponse> GetUploadUrlAsync(MediaUploadRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.TraceId))
        {
            throw new ArgumentException("Trace ID cannot be null or empty", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.ContentType))
        {
            throw new ArgumentException("Content type cannot be null or empty", nameof(request));
        }

        if (request.ContentLength <= 0)
        {
            throw new ArgumentException("Content length must be greater than zero", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Sha256Hash))
        {
            throw new ArgumentException("SHA-256 hash cannot be null or empty", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Field))
        {
            throw new ArgumentException("Field cannot be null or empty", nameof(request));
        }

        const string endpoint = "/api/public/media";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Requesting presigned upload URL for media trace {TraceId} at endpoint: {Endpoint}",
                request.TraceId, endpoint);
        }

        try
        {
            var json = JsonSerializer.Serialize(request, SerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<MediaUploadResponse>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize media upload response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully generated presigned upload URL for media {MediaId}", result.MediaId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to get media upload URL was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting media upload URL for trace {TraceId}",
                request.TraceId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while getting media upload URL for trace {request.TraceId}", ex);
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
            HttpStatusCode.NotFound => "The requested media was not found",
            HttpStatusCode.Unauthorized => "Authentication failed. Please check your API credentials",
            HttpStatusCode.Forbidden => "Access forbidden. You don't have permission to access this resource",
            HttpStatusCode.TooManyRequests => "Rate limit exceeded. Please retry after some time",
            HttpStatusCode.BadRequest => "Invalid request. Please check your request parameters",
            HttpStatusCode.Conflict => "Media already exists or conflict with existing data",
            HttpStatusCode.RequestEntityTooLarge => "Media file is too large",
            HttpStatusCode.UnsupportedMediaType => "Unsupported media type",
            _ => $"API request failed with status code {statusCode}"
        };

        throw new LangfuseApiException(statusCode, errorMessage, details: new Dictionary<string, object>
        {
            ["responseContent"] = errorContent,
            ["statusCode"] = statusCode
        });
    }
}