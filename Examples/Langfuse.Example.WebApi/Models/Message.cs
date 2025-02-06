using System.Text.Json.Serialization;

namespace Langfuse.Example.WebApi.Models;

public class Message
{
    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}