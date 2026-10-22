using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     A file of a new skill version. Set exactly one of <see cref="Content" /> or <see cref="Sha256Hash" />.
/// </summary>
public class SkillVersionFileCreateInput
{
    /// <summary>
    ///     Path of the file within the skill.
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; set; }

    /// <summary>
    ///     UTF-8 text content. The server computes the hash and byte length.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    ///     Canonical base64-encoded SHA-256 hash from a skill file manifest. The content must already exist in this
    ///     project.
    /// </summary>
    [JsonPropertyName("sha256Hash")]
    public string? Sha256Hash { get; set; }
}
