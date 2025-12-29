using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Request to create or update a membership.
/// </summary>
public class CreateMembershipRequest
{
    /// <summary>
    ///     ID of the user to add or update.
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    ///     Role to assign to the user.
    /// </summary>
    [JsonPropertyName("role")]
    public MembershipRole Role { get; set; }
}