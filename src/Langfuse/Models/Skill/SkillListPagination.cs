using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Pagination metadata for the skill list, with a flag for whether another page exists.
/// </summary>
public class SkillListPagination : ApiMetadata
{
    /// <summary>
    ///     Whether another page of results exists after this one.
    /// </summary>
    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; set; }
}
