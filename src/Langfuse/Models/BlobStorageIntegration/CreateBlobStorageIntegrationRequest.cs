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
    ///     Access key ID for authentication. Not used for GOOGLE_CLOUD_STORAGE or when AuthType is RoleArn.
    /// </summary>
    [JsonPropertyName("accessKeyId")]
    public string? AccessKeyId { get; init; }

    /// <summary>
    ///     Authentication method. RoleArn needs type S3, a concrete region (not "auto") and RoleArn; access key and secret
    ///     must be omitted, and endpoint and ForcePathStyle are ignored. When omitted, the server picks the method. The
    ///     first save with RoleArn stores the integration disabled: add the trust policy with the returned ExternalId, then
    ///     update with Enabled = true.
    /// </summary>
    [JsonPropertyName("authType")]
    public BlobStorageIntegrationAuthType? AuthType { get; init; }

    /// <summary>
    ///     ARN of the IAM role Langfuse assumes when AuthType is RoleArn. Kept on update when omitted.
    /// </summary>
    [JsonPropertyName("roleArn")]
    public string? RoleArn { get; init; }

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

    /// <summary>
    ///     Enable gzip compression for exported files (.csv.gz, .json.gz, .jsonl.gz). Defaults to true.
    /// </summary>
    [JsonPropertyName("compressed")]
    public bool? Compressed { get; init; }

    /// <summary>
    ///     Data to export. When omitted on update, the existing value is preserved. When omitted on create, the default
    ///     is ObservationsV2 on Langfuse Cloud, and on self-hosted deployments LegacyTracesObservations — or
    ///     ObservationsV2 where the deployment no longer populates the legacy tables. The default is never a source
    ///     the deployment cannot serve. Required when ExportFieldGroups is provided.
    /// </summary>
    [JsonPropertyName("exportSource")]
    public BlobStorageExportSource? ExportSource { get; init; }

    /// <summary>
    ///     Field groups to include in each exported row. Must include Core if provided. Requires ExportSource to be set in the
    ///     same request. Must be omitted for the LEGACY_TRACES_OBSERVATIONS source. When omitted on create, all observation
    ///     groups are selected and no score groups. Score groups add columns to the scores export.
    ///</summary>
    [JsonPropertyName("exportFieldGroups")]
    public IReadOnlyList<BlobStorageExportFieldGroup>? ExportFieldGroups { get; init; }
}