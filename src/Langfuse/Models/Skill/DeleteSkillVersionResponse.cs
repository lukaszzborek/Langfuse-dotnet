using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Response returned after deleting a skill version.
/// </summary>
public class DeleteSkillVersionResponse
{
    /// <summary>
    ///     Whether the version was deleted.
    /// </summary>
    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}
