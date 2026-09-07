using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     One chat message in an evaluator prompt.
/// </summary>
public class EvaluatorChatMessage
{
    /// <summary>
    ///     Message role.
    /// </summary>
    [JsonPropertyName("role")]
    public required EvaluatorChatMessageRole Role { get; init; }

    /// <summary>
    ///     Message content. Evaluator variables use <c>{{variable}}</c> syntax.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }
}