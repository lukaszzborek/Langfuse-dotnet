using zborek.Langfuse.Models.BlobStorageIntegration;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves all blob storage integrations for the organization
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of blob storage integrations with configuration details</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Requires organization-scoped API key</remarks>
    Task<BlobStorageIntegrationsResponse> GetBlobStorageIntegrationsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or updates a blob storage integration for a specific project
    /// </summary>
    /// <param name="request">Blob storage integration configuration including credentials and export settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created or updated blob storage integration with assigned ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when validation fails or an API error occurs</exception>
    /// <remarks>
    ///     The configuration is validated by performing a test upload to the bucket.
    ///     Requires organization-scoped API key.
    /// </remarks>
    Task<BlobStorageIntegrationResponse> UpsertBlobStorageIntegrationAsync(
        CreateBlobStorageIntegrationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a blob storage integration permanently
    /// </summary>
    /// <param name="id">The unique identifier of the blob storage integration to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation response</returns>
    /// <exception cref="LangfuseApiException">Thrown when the integration is not found or deletion fails</exception>
    /// <remarks>
    ///     This action immediately stops all data exports to the storage bucket and removes the integration configuration.
    ///     Requires organization-scoped API key.
    /// </remarks>
    Task<BlobStorageIntegrationDeletionResponse> DeleteBlobStorageIntegrationAsync(
        string id,
        CancellationToken cancellationToken = default);
}