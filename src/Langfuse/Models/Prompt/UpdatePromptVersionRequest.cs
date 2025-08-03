using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

public class UpdatePromptVersionRequest
{
    [JsonPropertyName("newLabels")]
    public List<string> NewLabels { get; set; } = new();
}