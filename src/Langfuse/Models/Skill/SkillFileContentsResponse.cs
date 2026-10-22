using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Response containing a batch of skill file contents.
/// </summary>
public class SkillFileContentsResponse
{
    /// <summary>
    ///     File contents for the requested hashes.
    /// </summary>
    [JsonPropertyName("data")]
    public SkillFileContent[] Data { get; set; } = [];
}
