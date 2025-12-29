using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Project;

/// <summary>
///     Request to create a new project.
/// </summary>
public class CreateProjectRequest
{
    /// <summary>
    ///     Project name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    ///     Number of days to retain data. Must be 0 or at least 3 days. Requires data-retention entitlement for non-zero
    ///     values. Optional.
    /// </summary>
    [JsonPropertyName("retention")]
    public int Retention { get; set; } = 0;

    /// <summary>
    ///     Optional metadata for the project.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}