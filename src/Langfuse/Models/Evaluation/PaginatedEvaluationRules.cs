using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Paginated list of evaluation rules.
/// </summary>
public record PaginatedEvaluationRules
{
    /// <summary>
    ///     Evaluation rules in the current page.
    /// </summary>
    [JsonPropertyName("data")]
    public required EvaluationRule[] Data { get; init; }

    /// <summary>
    ///     Standard pagination metadata.
    /// </summary>
    [JsonPropertyName("meta")]
    public required ApiMetadata Meta { get; init; }
}
