using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     User who created a resource.
/// </summary>
public class Creator
{
    /// <summary>
    ///     User identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     User name, or null when unavailable.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}