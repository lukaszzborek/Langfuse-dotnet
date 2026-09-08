using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Cursor-paginated page of evaluator versions in newest-first order.
/// </summary>
public class EvaluatorVersionsPage
{
    /// <summary>
    ///     Evaluator versions for this page.
    /// </summary>
    [JsonPropertyName("data")]
    public required EvaluatorVersion[] Data { get; init; }

    /// <summary>
    ///     Cursor pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public required CursorMeta Meta { get; init; }
}