using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Represents an organization in Langfuse.
///     Organizations are the top-level container that groups projects together.
/// </summary>
public class Organization
{
    /// <summary>
    ///     The unique identifier of the organization.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     The name of the organization.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}
