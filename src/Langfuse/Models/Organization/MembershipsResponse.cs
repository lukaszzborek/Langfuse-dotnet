using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Response containing a list of memberships.
/// </summary>
public class MembershipsResponse
{
    /// <summary>
    ///     List of membership details.
    /// </summary>
    [JsonPropertyName("memberships")]
    public List<MembershipResponse> Memberships { get; set; } = new();
}