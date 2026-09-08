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

    private readonly string? _description;

    /// <summary>
    ///     New description. Omit to keep the current description; assign <c>null</c> explicitly to clear it.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description
    {
        get => _description;
        init
        {
            _description = value;
            DescriptionSet = true;
        }
    }

    /// <summary>
    ///     True when <see cref="Description" /> was assigned (including an explicit null).
    /// </summary>
    [JsonIgnore]
    public bool DescriptionSet { get; private init; }
}
