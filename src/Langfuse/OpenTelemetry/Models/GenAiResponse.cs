namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Response data for Gen AI operations.
/// </summary>
public class GenAiResponse
{
    /// <summary>
    ///     The unique identifier for this response.
    /// </summary>
    public string? ResponseId { get; init; }

    /// <summary>
    ///     The model that generated this response.
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    ///     The reasons why the model stopped generating tokens.
    /// </summary>
    public string[]? FinishReasons { get; init; }

    /// <summary>
    ///     The number of tokens in the input prompt.
    /// </summary>
    public int? InputTokens { get; init; }

    /// <summary>
    ///     The number of tokens in the generated output.
    /// </summary>
    public int? OutputTokens { get; init; }

    /// <summary>
    ///     The cost for processing the input tokens.
    /// </summary>
    public decimal? InputCost { get; init; }

    /// <summary>
    ///     The cost for generating the output tokens.
    /// </summary>
    public decimal? OutputCost { get; init; }

    /// <summary>
    ///     The total cost for this operation.
    /// </summary>
    public decimal? TotalCost { get; init; }

    /// <summary>
    ///     The generated output messages from the model.
    /// </summary>
    public List<GenAiMessage>? OutputMessages { get; init; }

    /// <summary>
    ///     Convenience property for simple text completions.
    ///     Sets a single assistant message as the output.
    /// </summary>
    public string? Completion { get; init; }

    /// <summary>
    ///     The time when the model started generating the completion.
    /// </summary>
    public DateTimeOffset? CompletionStartTime { get; init; }

    /// <summary>
    ///     Detailed breakdown of token usage by category.
    /// </summary>
    public Dictionary<string, int>? UsageDetails { get; init; }

    /// <summary>
    ///     Detailed breakdown of costs by category.
    /// </summary>
    public Dictionary<string, decimal>? CostDetails { get; init; }
}