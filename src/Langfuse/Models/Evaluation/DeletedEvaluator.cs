using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Confirmation returned after successful evaluator deletion.
/// </summary>
public class DeletedEvaluator
{
    /// <summary>
    ///     Identifier of the deleted evaluator.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}