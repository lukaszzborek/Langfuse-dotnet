using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Ingestion event
/// </summary>
public interface IIngestionEvent
{
    /// <summary>
    ///     Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; }

    /// <summary>
    ///     Event ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    ///     Date of the event
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    /// <summary>
    ///     Optional metadata field used by Langfuse SDKs for debugging.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}