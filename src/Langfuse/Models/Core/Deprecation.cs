using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Migration signal returned by deprecated endpoints. Optional fields are omitted when they have no value.
/// </summary>
public class Deprecation
{
    /// <summary>
    ///     Human- and agent-readable summary of the deprecation and its replacement.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    ///     The replacement endpoint, e.g. "GET /api/public/v2/observations". Omitted when the endpoint is being removed
    ///     without a direct replacement.
    /// </summary>
    [JsonPropertyName("replacement")]
    public string? Replacement { get; set; }

    /// <summary>
    ///     Link to the migration documentation.
    /// </summary>
    [JsonPropertyName("docsUrl")]
    public string? DocsUrl { get; set; }
}
