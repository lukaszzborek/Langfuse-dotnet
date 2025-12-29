using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Response containing a list of organization projects.
/// </summary>
public class OrganizationProjectsResponse
{
    /// <summary>
    ///     List of projects in the organization.
    /// </summary>
    [JsonPropertyName("projects")]
    public List<OrganizationProject> Projects { get; set; } = new();
}