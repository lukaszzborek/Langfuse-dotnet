using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Field group included in each exported row for OBSERVATIONS_V2 and
///     LEGACY_TRACES_AND_ENRICHED_OBSERVATIONS exports.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<BlobStorageExportFieldGroup>))]
public enum BlobStorageExportFieldGroup
{
    /// <summary>Core fields. Must be included when field groups are provided.</summary>
    Core,

    /// <summary>Basic fields.</summary>
    Basic,

    /// <summary>Time-related fields.</summary>
    Time,

    /// <summary>Input/output fields.</summary>
    Io,

    /// <summary>Metadata fields.</summary>
    Metadata,

    /// <summary>Model fields.</summary>
    Model,

    /// <summary>Usage fields.</summary>
    Usage,

    /// <summary>Prompt fields.</summary>
    Prompt,

    /// <summary>Metrics fields.</summary>
    Metrics,

    /// <summary>Tool-related fields.</summary>
    Tools,

    /// <summary>Trace context fields.</summary>
    Trace_Context
}
