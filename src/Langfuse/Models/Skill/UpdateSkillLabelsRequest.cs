using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Request to replace the labels on a skill version.
/// </summary>
public class UpdateSkillLabelsRequest
{
    /// <summary>
    ///     Labels to set on the version. They move atomically from other versions.
    /// </summary>
    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = new();
}
