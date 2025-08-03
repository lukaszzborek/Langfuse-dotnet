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
        return await GetAsync<GetMediaResponse>(endpoint, "Get Media", cancellationToken);
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
        await PatchAsync(endpoint, request, "Update Media", cancellationToken);
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
        return await PostAsync<MediaUploadResponse>(endpoint, request, "Get Media Upload URL", cancellationToken);
    }
}