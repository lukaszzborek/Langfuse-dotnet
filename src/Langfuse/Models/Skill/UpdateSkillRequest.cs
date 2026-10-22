using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Request to replace the shared tags of a skill.
/// </summary>
public class UpdateSkillRequest
{
    /// <summary>
    ///     The complete set of tags to apply to every version of the skill. Pass an empty list to clear all tags.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();
}
