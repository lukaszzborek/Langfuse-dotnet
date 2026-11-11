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
    ///     The number of input tokens that were cached and served from a provider-managed cache.
    /// </summary>
    public int? CacheReadInputTokens { get; init; }

    /// <summary>
    ///     The number of input tokens written to a provider-managed cache.
    /// </summary>
    public int? CacheWriteInputTokens { get; init; }

    /// <summary>
    ///     The number of input tokens written to a provider-managed cache.
    /// </summary>
    [Obsolete("Renamed in the GenAI semantic conventions. Use CacheWriteInputTokens.")]
    public int? CacheCreationInputTokens
    {
        get => CacheWriteInputTokens;
        init => CacheWriteInputTokens = value;
    }

    /// <summary>
    ///     The number of output tokens used for reasoning (chain-of-thought, extended thinking).
    /// </summary>
    public int? ReasoningOutputTokens { get; init; }

    /// <summary>
    ///     The number of text input tokens.
    /// </summary>
    public int? TextInputTokens { get; init; }

    /// <summary>
    ///     The number of text output tokens.
    /// </summary>
    public int? TextOutputTokens { get; init; }

    /// <summary>
    ///     The number of text input tokens served from a provider-managed cache.
    /// </summary>
    public int? TextCacheReadInputTokens { get; init; }

    /// <summary>
    ///     The number of audio input tokens.
    /// </summary>
    public int? AudioInputTokens { get; init; }

    /// <summary>
    ///     The number of audio output tokens.
    /// </summary>
    public int? AudioOutputTokens { get; init; }

    /// <summary>
    ///     The number of audio input tokens served from a provider-managed cache.
    /// </summary>
    public int? AudioCacheReadInputTokens { get; init; }

    /// <summary>
    ///     The number of image input tokens.
    /// </summary>
    public int? ImageInputTokens { get; init; }

    /// <summary>
    ///     The number of image output tokens.
    /// </summary>
    public int? ImageOutputTokens { get; init; }

    /// <summary>
    ///     The number of image input tokens served from a provider-managed cache.
    /// </summary>
    public int? ImageCacheReadInputTokens { get; init; }

    /// <summary>
    ///     The lifecycle status of the response (e.g., "queued", "in_progress", "completed", "failed", "cancelled").
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    ///     Time from the start of the operation to the first chunk of a streamed response.
    /// </summary>
    public TimeSpan? TimeToFirstChunk { get; init; }

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