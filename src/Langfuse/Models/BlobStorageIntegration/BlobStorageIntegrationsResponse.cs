using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Response containing a collection of blob storage integrations.
/// </summary>
public class BlobStorageIntegrationsResponse
{
    /// <summary>
    ///     Array of blob storage integration configurations
    /// </summary>
    [JsonPropertyName("data")]
    public required BlobStorageIntegrationResponse[] Data { get; init; }
}