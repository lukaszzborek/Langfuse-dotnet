using zborek.Langfuse.Models.BlobStorageIntegration;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <summary>
    ///     Get all blob storage integrations for the organization.
    ///     Requires organization-scoped API key.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of blob storage integrations</returns>
    public async Task<BlobStorageIntegrationsResponse> GetBlobStorageIntegrationsAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<BlobStorageIntegrationsResponse>("/api/public/integrations/blob-storage",
            "Get Blob Storage Integrations", cancellationToken);
    }

    /// <summary>
    ///     Create or update a blob storage integration for a specific project.
    ///     The configuration is validated by performing a test upload to the bucket.
    ///     Requires organization-scoped API key.
    /// </summary>
    /// <param name="request">Blob storage integration configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created or updated blob storage integration</returns>
    public async Task<BlobStorageIntegrationResponse> UpsertBlobStorageIntegrationAsync(
        CreateBlobStorageIntegrationRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PutAsync<BlobStorageIntegrationResponse>("/api/public/integrations/blob-storage", request,
            "Create/Update Blob Storage Integration", cancellationToken);
    }

    /// <summary>
    ///     Delete a blob storage integration by ID.
    ///     Requires organization-scoped API key.
    /// </summary>
    /// <param name="id">The unique identifier of the blob storage integration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    public async Task<BlobStorageIntegrationDeletionResponse> DeleteBlobStorageIntegrationAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Blob storage integration ID cannot be null or empty", nameof(id));
        }

        var endpoint = $"/api/public/integrations/blob-storage/{Uri.EscapeDataString(id)}";
        return await DeleteAsync<BlobStorageIntegrationDeletionResponse>(endpoint,
            "Delete Blob Storage Integration", cancellationToken);
    }
}
