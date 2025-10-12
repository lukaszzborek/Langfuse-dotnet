using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Defines how frequently blob storage exports should occur.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<BlobStorageExportFrequency>))]
public enum BlobStorageExportFrequency
{
    /// <summary>
    ///     Export data every hour
    /// </summary>
    Hourly,

    /// <summary>
    ///     Export data once per day
    /// </summary>
    Daily,

    /// <summary>
    ///     Export data once per week
    /// </summary>
    Weekly
}
