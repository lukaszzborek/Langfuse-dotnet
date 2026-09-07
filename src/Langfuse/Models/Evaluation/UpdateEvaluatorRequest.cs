using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Request body for updating an evaluator. At least one field must be provided. Use
///     <see cref="UpdateEvaluatorMetadataRequest" /> for name/description-only changes (no new version is
///     created), or <see cref="UpdateLlmAsJudgeEvaluatorRequest" /> / <see cref="UpdateCodeEvaluatorRequest" />
///     to replace the definition as a complete unit (creates a new version). Evaluator type cannot change.
/// </summary>
public abstract class UpdateEvaluatorRequest
{
    /// <summary>
    ///     New human-readable evaluator name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    ///     New description. Omit or leave null to keep the current description. Note: the API accepts an explicit
    ///     null to clear the description, but this client omits null properties when serializing, so the
    ///     description cannot be cleared through <c>UpdateEvaluatorAsync</c>.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }
}