namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for Gen AI chat completion operations.
/// </summary>
public class GenAiChatCompletionConfig
{
    public required string Provider { get; init; }
    public required string Model { get; init; }
    public double? Temperature { get; init; }
    public double? TopP { get; init; }
    public double? TopK { get; init; }
    public int? MaxTokens { get; init; }
    public double? FrequencyPenalty { get; init; }
    public double? PresencePenalty { get; init; }
    public int? ChoiceCount { get; init; }
    public int? Seed { get; init; }
    public string[]? StopSequences { get; init; }
    public string? OutputType { get; init; }
    public string? ConversationId { get; init; }
    public string? SystemInstructions { get; init; }
    public List<GenAiToolDefinition>? Tools { get; init; }
    public string? ServerAddress { get; init; }
    public int? ServerPort { get; init; }
    public string? PromptName { get; init; }
    public int? PromptVersion { get; init; }
    public LangfuseObservationLevel? Level { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}