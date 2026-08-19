using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Defines what data a blob storage integration exports.
/// </summary>
/// <remarks>
///     Which sources a deployment accepts depends on how far it has moved to the v4 data model.
///     <see cref="ObservationsV2" /> and the enriched-observations portion of
///     <see cref="LegacyTracesAndEnrichedObservations" /> read the enriched observations table, so they require a
///     deployment that already populates it. <see cref="LegacyTracesObservations" /> and the legacy portion of
///     <see cref="LegacyTracesAndEnrichedObservations" /> read the legacy traces and observations tables, so they
///     require a deployment that still populates those. A deployment part-way through the migration populates both
///     and accepts every source. Selecting a source the deployment cannot serve is rejected with 400 rather than
///     exporting an empty result. See https://langfuse.com/docs/v4.
/// </remarks>
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
