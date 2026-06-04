using zborek.Langfuse.Models.BlobStorageIntegration;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<BlobStorageIntegrationsResponse> GetBlobStorageIntegrationsAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<BlobStorageIntegrationsResponse>("/api/public/integrations/blob-storage",
            "Get Blob Storage Integrations", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<BlobStorageIntegrationStatusResponse> GetBlobStorageIntegrationStatusAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Blob storage integration ID cannot be null or empty", nameof(id));
        }

        var endpoint = $"/api/public/integrations/blob-storage/{Uri.EscapeDataString(id)}";
        return await GetAsync<BlobStorageIntegrationStatusResponse>(endpoint,
            "Get Blob Storage Integration Status", cancellationToken);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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