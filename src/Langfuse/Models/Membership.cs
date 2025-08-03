using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a user's membership in an organization or project, defining their access level and permissions.
///     Memberships control what users can see and do within Langfuse resources.
/// </summary>
public class Membership
{
    /// <summary>
    ///     Unique identifier of the membership record.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the user who holds this membership, linking to their account.
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the organization this membership applies to. Used for organization-level memberships.
    /// </summary>
    [JsonPropertyName("organizationId")]
    public string? OrganizationId { get; set; }

    /// <summary>
    ///     ID of the specific project this membership applies to. Used for project-level memberships within an organization.
    /// </summary>
    [JsonPropertyName("projectId")]
    public string? ProjectId { get; set; }

    /// <summary>
    ///     Role defining the user's permissions and access level (Owner, Admin, Member, or Viewer).
    /// </summary>
    [JsonPropertyName("role")]
    public MembershipRole Role { get; set; }

    /// <summary>
    ///     Timestamp when the membership was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the membership was last updated or modified.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}