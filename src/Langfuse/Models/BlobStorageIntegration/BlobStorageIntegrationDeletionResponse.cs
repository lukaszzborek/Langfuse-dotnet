using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Response returned after deleting a blob storage integration.
/// </summary>
public class BlobStorageIntegrationDeletionResponse
{
    /// <summary>
    ///     Confirmation message about the deletion
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}