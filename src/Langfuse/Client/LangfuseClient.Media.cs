using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Media;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<GetMediaResponse> GetMediaAsync(string mediaId, CancellationToken cancellationToken = default)
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
            var result = JsonSerializer.Deserialize<GetMediaResponse>(responseContent, JsonOptions);

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
    public async Task UpdateMediaAsync(string mediaId, PatchMediaBody request,
        CancellationToken cancellationToken = default)
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
            var json = JsonSerializer.Serialize(request, JsonOptions);
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
    public async Task<MediaUploadResponse> GetMediaUploadUrlAsync(MediaUploadRequest request,
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
            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<MediaUploadResponse>(responseContent, JsonOptions);

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
}