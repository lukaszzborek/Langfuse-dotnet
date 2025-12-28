using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Langfuse ingestion request
/// </summary>
public class IngestionRequest
{
    /// <summary>
    ///     All events to ingest
    /// </summary>
    [JsonPropertyName("batch")]
    public object[] Batch { get; set; } = [];

    /// <summary>
    ///     Optional metadata field used by Langfuse SDKs for debugging.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}