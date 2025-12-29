using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Response containing membership information.
/// </summary>
public class MembershipResponse
{
    /// <summary>
    ///     ID of the user.
    /// </summary>
    [JsonPropertyName("userId")]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    ///     Role of the user in the organization or project.
    /// </summary>
    [JsonPropertyName("role")]
    public MembershipRole Role { get; set; }

    /// <summary>
    ///     Email address of the user.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the user.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}