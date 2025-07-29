using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class UpdatePromptVersionRequest
{
    [JsonPropertyName("newLabels")]
    public List<string> NewLabels { get; set; } = new();
}