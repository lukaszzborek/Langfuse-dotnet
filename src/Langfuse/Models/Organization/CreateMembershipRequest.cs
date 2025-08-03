using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

public class CreateMembershipRequest
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public MembershipRole Role { get; set; }
}