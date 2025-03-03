using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

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
}