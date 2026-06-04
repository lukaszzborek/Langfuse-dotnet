using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Defines what data a blob storage integration exports.
/// </summary>
[JsonConverter(typeof(SnakeCaseUpperEnumConverter<BlobStorageExportSource>))]
public enum BlobStorageExportSource
{
    /// <summary>
    ///     Traces, observations, and scores tables with a fixed column set. ExportFieldGroups is not applicable.
    /// </summary>
    LegacyTracesObservations,

    /// <summary>
    ///     Same data model as the /api/public/v2/observations endpoint, plus scores. Columns controlled by ExportFieldGroups.
    /// </summary>
    ObservationsV2,

    /// <summary>
    ///     Both legacy and enriched-observation sets. Enriched portion columns controlled by ExportFieldGroups.
    /// </summary>
    LegacyTracesAndEnrichedObservations
}
