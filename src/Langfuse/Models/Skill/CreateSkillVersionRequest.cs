using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Request to create a new skill version.
/// </summary>
public class CreateSkillVersionRequest
{
    /// <summary>
    ///     Files of the new skill version.
    /// </summary>
    [JsonPropertyName("files")]
    public List<SkillVersionFileCreateInput> Files { get; set; } = new();

    /// <summary>
    ///     Optional commit message for the version.
    /// </summary>
    [JsonPropertyName("commitMessage")]
    public string? CommitMessage { get; set; }
}
