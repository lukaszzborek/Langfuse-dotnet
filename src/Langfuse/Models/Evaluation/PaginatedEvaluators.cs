using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Paginated list of evaluators.
/// </summary>
public record PaginatedEvaluators
{
    /// <summary>
    ///     Evaluators in the current page.
    /// </summary>
    [JsonPropertyName("data")]
    public required Evaluator[] Data { get; init; }

    /// <summary>
    ///     Standard pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public required ApiMetadata Meta { get; init; }
}
