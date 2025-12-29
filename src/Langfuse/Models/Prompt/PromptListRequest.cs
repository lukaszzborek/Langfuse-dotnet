using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Request parameters for listing prompts.
/// </summary>
public class PromptListRequest
{
    /// <summary>
    ///     Filter by prompt name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Filter by label.
    /// </summary>
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    /// <summary>
    ///     Filter by tag.
    /// </summary>
    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    /// <summary>
    ///     Page number, starts at 1.
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    ///     Limit of items per page.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    ///     Filter to only include prompt versions created/updated on or after a certain datetime (ISO 8601).
    /// </summary>
    [JsonPropertyName("fromUpdatedAt")]
    public DateTime? FromUpdatedAt { get; set; }

    /// <summary>
    ///     Filter to only include prompt versions created/updated before a certain datetime (ISO 8601).
    /// </summary>
    [JsonPropertyName("toUpdatedAt")]
    public DateTime? ToUpdatedAt { get; set; }
}