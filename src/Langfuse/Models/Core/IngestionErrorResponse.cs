using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Error ingestion response
/// </summary>
public class IngestionErrorResponse
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

    /// <summary>
    ///     Message describing the error
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    ///     Error details. Can be any JSON value.
    /// </summary>
    [JsonPropertyName("error")]
    public object? Error { get; set; }
}