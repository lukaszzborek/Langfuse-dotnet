using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Responses;

public class MembershipsResponse
{
    [JsonPropertyName("memberships")]
    public List<MembershipResponse> Memberships { get; set; } = new();
}