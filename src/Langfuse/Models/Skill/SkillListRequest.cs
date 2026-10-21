using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Request parameters for listing skills.
/// </summary>
public class SkillListRequest
{
    /// <summary>
    ///     Filter by exact skill name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Case-insensitive search across skill names and latest-version descriptions.
    /// </summary>
    [JsonPropertyName("search")]
    public string? Search { get; set; }

    /// <summary>
    ///     Filter by a shared skill tag.
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
    ///     Include skills whose latest version was updated at or after this timestamp.
    /// </summary>
    [JsonPropertyName("fromUpdatedAt")]
    public DateTimeOffset? FromUpdatedAt { get; set; }

    /// <summary>
    ///     Include skills whose latest version was updated before this timestamp.
    /// </summary>
    [JsonPropertyName("toUpdatedAt")]
    public DateTimeOffset? ToUpdatedAt { get; set; }
}
