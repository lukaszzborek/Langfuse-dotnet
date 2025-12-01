namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Response data for Gen AI operations.
/// </summary>
public class GenAiResponse
{
    public string? ResponseId { get; init; }
    public string? Model { get; init; }
    public string[]? FinishReasons { get; init; }
    public int? InputTokens { get; init; }
    public int? OutputTokens { get; init; }
    public int? TotalTokens { get; init; }
    public decimal? InputCost { get; init; }
    public decimal? OutputCost { get; init; }
    public decimal? TotalCost { get; init; }
    public List<GenAiMessage>? OutputMessages { get; init; }

    /// <summary>
    ///     Convenience property for simple text completions.
    ///     Sets a single assistant message as the output.
    /// </summary>
    public string? Completion { get; init; }
    public DateTimeOffset? CompletionStartTime { get; init; }
    public Dictionary<string, int>? UsageDetails { get; init; }
    public Dictionary<string, decimal>? CostDetails { get; init; }
}