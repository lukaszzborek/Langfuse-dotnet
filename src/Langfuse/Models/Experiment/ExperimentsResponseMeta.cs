using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Experiment;

/// <summary>
///     Metadata for cursor-based pagination of experiment endpoints.
/// </summary>
public class ExperimentsResponseMeta
{
    /// <summary>
    ///     Versioned base64url cursor for retrieving the next page. Absent (null) when there are no more results.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; init; }
}