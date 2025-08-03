using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Successful ingestion response
/// </summary>
public class IngestionSuccessResponse
{
    /// <summary>
    ///     Observation ID. This is the same as the ID of the event that was ingested.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Status code
    /// </summary>
    [JsonPropertyName("status")]
    public int Status { get; set; }
}