using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Project;

public class ProjectDeletionResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}