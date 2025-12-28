using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Response containing blob storage integration details.
/// </summary>
public class BlobStorageIntegrationResponse
{
    /// <summary>
    ///     Unique identifier for the blob storage integration
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     ID of the project this integration belongs to
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
    ///     Custom endpoint URL for S3-compatible services
    /// </summary>
    [JsonPropertyName("endpoint")]
    public string? Endpoint { get; init; }

    /// <summary>
    ///     Storage region
    /// </summary>
    [JsonPropertyName("region")]
    public required string Region { get; init; }

    /// <summary>
    ///     Access key ID for authentication
    /// </summary>
    [JsonPropertyName("accessKeyId")]
    public string? AccessKeyId { get; init; }

    /// <summary>
    ///     Prefix path for exported files
    /// </summary>
    [JsonPropertyName("prefix")]
    public required string Prefix { get; init; }

    /// <summary>
    ///     How frequently exports occur
    /// </summary>
    [JsonPropertyName("exportFrequency")]
    public required BlobStorageExportFrequency ExportFrequency { get; init; }

    /// <summary>
    ///     Whether the integration is enabled
    /// </summary>
    [JsonPropertyName("enabled")]
    public required bool Enabled { get; init; }

    /// <summary>
    ///     Whether path-style addressing is used for S3 requests
    /// </summary>
    [JsonPropertyName("forcePathStyle")]
    public required bool ForcePathStyle { get; init; }

    /// <summary>
    ///     File format for exported data
    /// </summary>
    [JsonPropertyName("fileType")]
    public required BlobStorageIntegrationFileType FileType { get; init; }

    /// <summary>
    ///     Determines which historical data is exported
    /// </summary>
    [JsonPropertyName("exportMode")]
    public required BlobStorageExportMode ExportMode { get; init; }

    /// <summary>
    ///     Start date for exports when ExportMode is FROM_CUSTOM_DATE
    /// </summary>
    [JsonPropertyName("exportStartDate")]
    public DateTime? ExportStartDate { get; init; }

    /// <summary>
    ///     Timestamp of the next scheduled export
    /// </summary>
    [JsonPropertyName("nextSyncAt")]
    public DateTime? NextSyncAt { get; init; }

    /// <summary>
    ///     Timestamp of the most recent export
    /// </summary>
    [JsonPropertyName("lastSyncAt")]
    public DateTime? LastSyncAt { get; init; }

    /// <summary>
    ///     Timestamp when the integration was created
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Timestamp when the integration was last updated
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }
}