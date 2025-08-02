using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

public class CreateScimUserRequest
{
    [JsonPropertyName("userName")]
    public required string UserName { get; set; }

    [JsonPropertyName("name")]
    public required ScimUserName Name { get; set; } = new();

    [JsonPropertyName("emails")]
    public List<ScimUserEmail>? Emails { get; set; }

    [JsonPropertyName("active")]
    public bool? Active { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}