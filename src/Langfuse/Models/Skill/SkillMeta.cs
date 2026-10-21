using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     Metadata about a skill, taken from its latest version, with shared tags and the production version.
/// </summary>
public class SkillMeta
{
    /// <summary>
    ///     Name of the skill.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the latest skill version.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Tags shared across all versions of the skill.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    ///     Number of the latest skill version.
    /// </summary>
    [JsonPropertyName("latestVersion")]
    public int LatestVersion { get; set; }

    /// <summary>
    ///     Timestamp when the latest version was created.
    /// </summary>
    [JsonPropertyName("latestVersionCreatedAt")]
    public DateTime LatestVersionCreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the latest version was last updated.
    /// </summary>
    [JsonPropertyName("latestVersionLastUpdatedAt")]
    public DateTime LatestVersionLastUpdatedAt { get; set; }

    /// <summary>
    ///     Version assigned the production label, or null when no version is assigned.
    /// </summary>
    [JsonPropertyName("productionVersion")]
    public int? ProductionVersion { get; set; }
}
