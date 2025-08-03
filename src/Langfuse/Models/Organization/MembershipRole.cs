using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Defines the role levels for organization and project memberships, determining access permissions and capabilities.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<MembershipRole>))]
public enum MembershipRole
{
    /// <summary>
    ///     Owner role with full control over the organization or project, including billing, user management, and all data
    ///     operations.
    /// </summary>
    Owner,

    /// <summary>
    ///     Admin role with administrative privileges including user management and configuration, but limited billing access.
    /// </summary>
    Admin,

    /// <summary>
    ///     Member role with standard access to view and create data, but limited administrative capabilities.
    /// </summary>
    Member,

    /// <summary>
    ///     Viewer role with read-only access to data and configurations, cannot make changes or create new content.
    /// </summary>
    Viewer
}