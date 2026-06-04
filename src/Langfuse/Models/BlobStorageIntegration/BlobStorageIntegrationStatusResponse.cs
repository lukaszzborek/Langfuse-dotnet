using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Sync status details of a blob storage integration, intended for ETL polling.
/// </summary>
public class BlobStorageIntegrationStatusResponse
{
    /// <summary>
    ///     Unique identifier of the blob storage integration.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     ID of the project this integration belongs to.
    /// </summary>
    [JsonPropertyName("projectId")]
    public required string ProjectId { get; init; }

    /// <summary>
    ///     Current sync status of the integration.
    /// </summary>
    [JsonPropertyName("syncStatus")]
    public required BlobStorageSyncStatus SyncStatus { get; init; }

    /// <summary>
    ///     Whether the integration is enabled.
    /// </summary>
    [JsonPropertyName("enabled")]
    public required bool Enabled { get; init; }

    /// <summary>
    ///     End of the last successfully exported time window. Null if the integration has never synced.
    /// </summary>
    [JsonPropertyName("lastSyncAt")]
    public DateTime? LastSyncAt { get; init; }

    /// <summary>
    ///     When the next export is scheduled. Null if no sync has occurred yet.
    /// </summary>
    [JsonPropertyName("nextSyncAt")]
    public DateTime? NextSyncAt { get; init; }

    /// <summary>
    ///     Raw error message from the storage provider if the last export failed. Cleared on successful export.
    /// </summary>
    [JsonPropertyName("lastError")]
    public string? LastError { get; init; }

    /// <summary>
    ///     When the last error occurred. Cleared on successful export.
    /// </summary>
    [JsonPropertyName("lastErrorAt")]
    public DateTime? LastErrorAt { get; init; }
}
