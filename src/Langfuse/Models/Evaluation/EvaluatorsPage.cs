using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Cursor-paginated page of evaluators.
/// </summary>
public class EvaluatorsPage
{
    /// <summary>
    ///     Evaluators for this page.
    /// </summary>
    [JsonPropertyName("data")]
    public required Evaluator[] Data { get; init; }

    /// <summary>
    ///     Cursor pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public required CursorMeta Meta { get; init; }
}