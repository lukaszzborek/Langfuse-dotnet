using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     An immutable version of a skill with its metadata and file manifest.
/// </summary>
public class SkillVersion
{
    /// <summary>
    ///     Unique identifier of the skill version.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the skill version was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the skill version was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     ID of the project the skill belongs to.
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the user or API key that created the version.
    /// </summary>
    [JsonPropertyName("createdBy")]
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the skill.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the skill version.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Frontmatter parsed from the skill definition.
    /// </summary>
    [JsonPropertyName("frontmatter")]
    public object? Frontmatter { get; set; }

    /// <summary>
    ///     Version number of the skill.
    /// </summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>
    ///     Tags shared across all versions of the skill.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    ///     Labels assigned to this version, such as "production".
    /// </summary>
    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = new();

    /// <summary>
    ///     Commit message given when the version was created.
    /// </summary>
    [JsonPropertyName("commitMessage")]
    public string? CommitMessage { get; set; }

    /// <summary>
    ///     Files in this skill version.
    /// </summary>
    [JsonPropertyName("files")]
    public List<SkillFile> Files { get; set; } = new();
}
