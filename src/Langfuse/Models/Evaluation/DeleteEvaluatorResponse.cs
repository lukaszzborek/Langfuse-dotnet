using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Confirmation response returned after successfully deleting an evaluator.
/// </summary>
public class DeleteEvaluatorResponse
{
    /// <summary>
    ///     Confirmation message about the deletion.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}