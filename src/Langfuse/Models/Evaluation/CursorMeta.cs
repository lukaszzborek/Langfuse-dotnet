using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Cursor pagination metadata.
/// </summary>
public class CursorMeta
{
    /// <summary>
    ///     Opaque cursor for the next page. Null when there is no next page.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; init; }
}