using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

public class OrganizationProjectsResponse
{
    [JsonPropertyName("projects")]
    public List<OrganizationProject> Projects { get; set; } = new();
}