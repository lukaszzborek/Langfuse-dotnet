using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Cursor-paginated page of evaluation rules.
/// </summary>
public class EvaluationRulesPage
{
    /// <summary>
    ///     Evaluation rules for this page.
    /// </summary>
    [JsonPropertyName("data")]
    public required EvaluationRule[] Data { get; init; }

    /// <summary>
    ///     Cursor pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public required CursorMeta Meta { get; init; }
}