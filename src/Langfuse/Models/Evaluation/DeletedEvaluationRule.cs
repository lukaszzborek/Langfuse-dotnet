using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Confirmation returned after successful evaluation-rule deletion.
/// </summary>
public class DeletedEvaluationRule
{
    /// <summary>
    ///     Identifier of the deleted evaluation rule.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }
}