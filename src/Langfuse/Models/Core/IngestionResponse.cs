using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Ingetion response
/// </summary>
public class IngestionResponse
{
    /// <summary>
    ///     Ingestion successes
    /// </summary>
    [JsonPropertyName("successes")]
    public IngestionSuccessResponse[]? Successes { get; set; } = [];

    /// <summary>
    ///     Errors
    /// </summary>
    [JsonPropertyName("errors")]
    public IngestionErrorResponse[]? Errors { get; set; }
}