using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Text content of a skill file, identified by its hash.
/// </summary>
public class SkillFileContent
{
    /// <summary>
    ///     Canonical base64-encoded SHA-256 hash of the content.
    /// </summary>
    [JsonPropertyName("sha256Hash")]
    public string Sha256Hash { get; set; } = string.Empty;

    /// <summary>
    ///     UTF-8 text content.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;
}
