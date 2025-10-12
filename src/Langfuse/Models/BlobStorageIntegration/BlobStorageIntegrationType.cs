using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Defines the type of blob storage integration for exporting observability data.
/// </summary>
[JsonConverter(typeof(SnakeCaseUpperEnumConverter<BlobStorageIntegrationType>))]
public enum BlobStorageIntegrationType
{
    /// <summary>
    ///     Amazon S3 storage service
    /// </summary>
    S3,

    /// <summary>
    ///     S3-compatible storage service (e.g., MinIO, DigitalOcean Spaces)
    /// </summary>
    S3Compatible,

    /// <summary>
    ///     Microsoft Azure Blob Storage service
    /// </summary>
    AzureBlobStorage
}