using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Request to update prompt version labels.
/// </summary>
public class UpdatePromptVersionRequest
{
    /// <summary>
    ///     New labels to apply to the prompt version.
    /// </summary>
    [JsonPropertyName("newLabels")]
    public List<string> NewLabels { get; set; } = new();
}