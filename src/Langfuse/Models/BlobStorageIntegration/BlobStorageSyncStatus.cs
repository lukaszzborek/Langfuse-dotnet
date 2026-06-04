using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Sync status of a blob storage integration.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<BlobStorageSyncStatus>))]
public enum BlobStorageSyncStatus
{
    /// <summary>
    ///     Enabled but has never exported yet.
    /// </summary>
    Idle,

    /// <summary>
    ///     Next export is overdue and waiting to be picked up by the worker.
    /// </summary>
    Queued,

    /// <summary>
    ///     All available data has been exported; next export is scheduled for the future.
    /// </summary>
    Up_To_Date,

    /// <summary>
    ///     Integration is not enabled.
    /// </summary>
    Disabled,

    /// <summary>
    ///     Last export failed (see LastError for details).
    /// </summary>
    Error
}
