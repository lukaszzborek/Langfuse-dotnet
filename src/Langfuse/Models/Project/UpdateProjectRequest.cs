using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Project;

/// <summary>
///     Request to update an existing project.
/// </summary>
public class UpdateProjectRequest
{
    /// <summary>
    ///     Updated project name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Optional metadata for the project.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Number of days to retain data. Must be 0 or at least 3 days.
    /// </summary>
    [JsonPropertyName("retention")]
    public int Retention { get; set; }
}