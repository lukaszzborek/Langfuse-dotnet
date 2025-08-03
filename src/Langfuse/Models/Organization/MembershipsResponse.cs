using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

public class MembershipsResponse
{
    [JsonPropertyName("memberships")]
    public List<MembershipResponse> Memberships { get; set; } = new();
}