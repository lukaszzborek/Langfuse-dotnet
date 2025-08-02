using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a media record by its ID
    /// </summary>
    /// <param name="mediaId">Unique identifier of the media record</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The media record with the specified ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when the media record is not found or an API error occurs</exception>
    Task<GetMediaResponse> GetMediaAsync(string mediaId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates a media record's metadata
    /// </summary>
    /// <param name="mediaId">Unique identifier of the media record</param>
    /// <param name="request">Media patch request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    /// <exception cref="LangfuseApiException">Thrown when media update fails</exception>
    Task UpdateMediaAsync(string mediaId, PatchMediaBody request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a presigned upload URL for media file upload
    /// </summary>
    /// <param name="request">Media upload request containing file metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Presigned upload URL and media record information</returns>
    /// <exception cref="LangfuseApiException">Thrown when presigned URL generation fails</exception>
    Task<MediaUploadResponse> GetMediaUploadUrlAsync(MediaUploadRequest request,
        CancellationToken cancellationToken = default);
}