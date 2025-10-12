using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Defines the file format for blob storage exports.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<BlobStorageIntegrationFileType>))]
public enum BlobStorageIntegrationFileType
{
    /// <summary>
    ///     JSON file format
    /// </summary>
    Json,

    /// <summary>
    ///     CSV (Comma-Separated Values) file format
    /// </summary>
    Csv,

    /// <summary>
    ///     JSON Lines file format (newline-delimited JSON)
    /// </summary>
    Jsonl
}
