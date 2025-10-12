using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
/// Request to delete a membership
/// </summary>
public class DeleteMembershipRequest
{
    /// <summary>
    /// User ID of the member to remove
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;
}
