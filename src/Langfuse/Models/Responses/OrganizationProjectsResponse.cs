using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Responses;

public class OrganizationProjectsResponse
{
    [JsonPropertyName("projects")]
    public List<OrganizationProject> Projects { get; set; } = new();
}