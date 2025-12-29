using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Represents a SCIM user resource.
/// </summary>
public class ScimUser
{
    /// <summary>
    ///     SCIM schema URIs for this user resource.
    /// </summary>
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    /// <summary>
    ///     Unique identifier of the user.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Username (typically the email address).
    /// </summary>
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    ///     User's name information.
    /// </summary>
    [JsonPropertyName("name")]
    public ScimUserName Name { get; set; } = new();

    /// <summary>
    ///     List of user's email addresses.
    /// </summary>
    [JsonPropertyName("emails")]
    public List<ScimUserEmail> Emails { get; set; } = new();

    /// <summary>
    ///     Metadata about the user resource.
    /// </summary>
    [JsonPropertyName("meta")]
    public ScimMeta Meta { get; set; } = new();
}