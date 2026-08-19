using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Request to create a new SCIM user.
/// </summary>
public class CreateScimUserRequest
{
    /// <summary>
    ///     User's email address (required).
    /// </summary>
    [JsonPropertyName("userName")]
    public required string UserName { get; set; }

    /// <summary>
    ///     User's name information.
    /// </summary>
    [JsonPropertyName("name")]
    public required ScimUserName Name { get; set; } = new();

    /// <summary>
    ///     User's email addresses.
    /// </summary>
    [JsonPropertyName("emails")]
    public List<ScimUserEmail>? Emails { get; set; }

    /// <summary>
    ///     Whether the user is active.
    /// </summary>
    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    /// <summary>
    ///     Ignored. Accepted only for compatibility with identity providers that always send a password on user
    ///     creation (Okta sends a placeholder value even when password sync is disabled). No credential is created
    ///     for the user; provisioned users authenticate via SSO or set a password themselves through the password
    ///     reset flow.
    /// </summary>
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}