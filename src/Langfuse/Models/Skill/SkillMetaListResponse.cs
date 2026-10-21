using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Paginated response containing skill metadata.
/// </summary>
public class SkillMetaListResponse
{
    /// <summary>
    ///     Skills in this page of results.
    /// </summary>
    [JsonPropertyName("data")]
    public SkillMeta[] Data { get; set; } = [];

    /// <summary>
    ///     Pagination information for this page of results.
    /// </summary>
    [JsonPropertyName("meta")]
    public SkillListPagination Meta { get; set; } = new();
}
