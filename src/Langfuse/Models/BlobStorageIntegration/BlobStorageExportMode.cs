using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Defines the export mode for blob storage integration, determining which data should be exported.
/// </summary>
[JsonConverter(typeof(SnakeCaseUpperEnumConverter<BlobStorageExportMode>))]
public enum BlobStorageExportMode
{
    /// <summary>
    ///     Export full history of all data
    /// </summary>
    FullHistory,

    /// <summary>
    ///     Export data from today onwards
    /// </summary>
    FromToday,

    /// <summary>
    ///     Export data from a custom specified date
    /// </summary>
    FromCustomDate
}
