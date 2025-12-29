namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for Gen AI chat completion operations.
/// </summary>
public class GenAiChatCompletionConfig
{
    /// <summary>
    ///     The AI provider name (e.g., "openai", "anthropic").
    /// </summary>
    public required string Provider { get; init; }

    /// <summary>
    ///     The model identifier to use for the completion.
    /// </summary>
    public required string Model { get; init; }

    /// <summary>
    ///     Controls randomness in the output. Higher values make output more random.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    ///     Nucleus sampling parameter. Considers tokens with top_p probability mass.
    /// </summary>
    public double? TopP { get; init; }

    /// <summary>
    ///     Limits the number of highest probability tokens to consider.
    /// </summary>
    public double? TopK { get; init; }

    /// <summary>
    ///     Maximum number of tokens to generate in the completion.
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    ///     Penalizes new tokens based on their frequency in the text so far.
    /// </summary>
    public double? FrequencyPenalty { get; init; }

    /// <summary>
    ///     Penalizes new tokens based on whether they appear in the text so far.
    /// </summary>
    public double? PresencePenalty { get; init; }

    /// <summary>
    ///     Number of completion choices to generate.
    /// </summary>
    public int? ChoiceCount { get; init; }

    /// <summary>
    ///     Random seed for deterministic sampling.
    /// </summary>
    public int? Seed { get; init; }

    /// <summary>
    ///     Sequences where the model will stop generating further tokens.
    /// </summary>
    public string[]? StopSequences { get; init; }

    /// <summary>
    ///     The expected output type format (e.g., "json", "text").
    /// </summary>
    public string? OutputType { get; init; }

    /// <summary>
    ///     Identifier for the conversation context.
    /// </summary>
    public string? ConversationId { get; init; }

    /// <summary>
    ///     System-level instructions for the model.
    /// </summary>
    public string? SystemInstructions { get; init; }

    /// <summary>
    ///     List of tool definitions available to the model.
    /// </summary>
    public List<GenAiToolDefinition>? Tools { get; init; }

    /// <summary>
    ///     The server address for custom API endpoints.
    /// </summary>
    public string? ServerAddress { get; init; }

    /// <summary>
    ///     The server port for custom API endpoints.
    /// </summary>
    public int? ServerPort { get; init; }

    /// <summary>
    ///     The name of the prompt template being used.
    /// </summary>
    public string? PromptName { get; init; }

    /// <summary>
    ///     The version of the prompt template being used.
    /// </summary>
    public int? PromptVersion { get; init; }

    /// <summary>
    ///     The observation level for logging severity.
    /// </summary>
    public LangfuseObservationLevel? Level { get; init; }

    /// <summary>
    ///     Additional metadata associated with this operation.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}