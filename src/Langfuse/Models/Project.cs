using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a project in Langfuse - an organizational unit that contains traces, observations, and other data within an organization.
///     Projects provide isolation and access control for different applications or teams.
/// </summary>
public class Project
{
    /// <summary>
    ///     Unique identifier of the project.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Human-readable name of the project, used for identification and organization.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Additional metadata associated with the project as a JSON object, containing custom properties and configuration.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Number of days to retain data before automatic deletion. Null or 0 means no retention policy is configured.
    ///     Used for compliance and storage management.
    /// </summary>
    [JsonPropertyName("retentionDays")]
    public int? RetentionDays { get; set; }
}