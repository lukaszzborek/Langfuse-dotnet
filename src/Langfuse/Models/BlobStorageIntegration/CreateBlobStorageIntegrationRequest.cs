using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Request to create a new blob storage integration for exporting observability data.
/// </summary>
public class CreateBlobStorageIntegrationRequest
{
    /// <summary>
    ///     ID of the project in which to configure the blob storage integration
    /// </summary>
    [JsonPropertyName("projectId")]
    public required string ProjectId { get; init; }

    /// <summary>
    ///     Type of blob storage service
    /// </summary>
    [JsonPropertyName("type")]
    public required BlobStorageIntegrationType Type { get; init; }

    /// <summary>
    ///     Name of the storage bucket
    /// </summary>
    [JsonPropertyName("bucketName")]
    public required string BucketName { get; init; }

    /// <summary>
    ///     Custom endpoint URL for S3-compatible services (required for S3_COMPATIBLE type)
    /// </summary>
    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; init; }

    /// <summary>
    ///     Storage region (e.g., us-east-1, eu-central-1)
    /// </summary>
    [JsonPropertyName("region")]
    public required string Region { get; init; }

    /// <summary>
    ///     Access key ID for authentication
    /// </summary>
    [JsonPropertyName("accessKeyId")]
    public string? AccessKeyId { get; init; }

    /// <summary>
    ///     Secret access key for authentication (will be encrypted when stored)
    /// </summary>
    [JsonPropertyName("secretAccessKey")]
    public string? SecretAccessKey { get; init; }

    /// <summary>
    ///     Optional prefix path for exported files (must end with "/" if provided)
    /// </summary>
    [JsonPropertyName("prefix")]
    public string? Prefix { get; init; }

    /// <summary>
    ///     How frequently exports should occur
    /// </summary>
    [JsonPropertyName("exportFrequency")]
    public required BlobStorageExportFrequency ExportFrequency { get; init; }

    /// <summary>
    ///     Whether the integration is enabled
    /// </summary>
    [JsonPropertyName("enabled")]
    public required bool Enabled { get; init; }

    /// <summary>
    ///     Whether to use path-style addressing for S3 requests
    /// </summary>
    [JsonPropertyName("forcePathStyle")]
    public required bool ForcePathStyle { get; init; }

    /// <summary>
    ///     File format for exported data
    /// </summary>
    [JsonPropertyName("fileType")]
    public required BlobStorageIntegrationFileType FileType { get; init; }

    /// <summary>
    ///     Determines which historical data to export
    /// </summary>
    [JsonPropertyName("exportMode")]
    public required BlobStorageExportMode ExportMode { get; init; }

    /// <summary>
    ///     Start date for exports when ExportMode is FROM_CUSTOM_DATE
    /// </summary>
    [JsonPropertyName("exportStartDate")]
    public DateTime? ExportStartDate { get; init; }
}
