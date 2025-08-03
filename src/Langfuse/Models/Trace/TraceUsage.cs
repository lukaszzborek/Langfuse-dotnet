using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Token usage information for a trace
/// </summary>
public class TraceUsage
{
    /// <summary>
    ///     Total number of prompt tokens used
    /// </summary>
    [JsonPropertyName("promptTokens")]
    public int? PromptTokens { get; set; }

    /// <summary>
    ///     Total number of completion tokens used
    /// </summary>
    [JsonPropertyName("completionTokens")]
    public int? CompletionTokens { get; set; }

    /// <summary>
    ///     Total number of tokens used
    /// </summary>
    [JsonPropertyName("totalTokens")]
    public int? TotalTokens { get; set; }
}