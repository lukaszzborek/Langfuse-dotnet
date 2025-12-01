namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     OpenTelemetry semantic convention attribute names for Gen AI operations.
///     Based on: https://opentelemetry.io/docs/specs/semconv/registry/attributes/gen-ai/
/// </summary>
public static class GenAiAttributes
{
    // Operation & Provider
    public const string OperationName = "gen_ai.operation.name";
    public const string ProviderName = "gen_ai.provider.name";

    // Request attributes
    public const string RequestModel = "gen_ai.request.model";
    public const string RequestTemperature = "gen_ai.request.temperature";
    public const string RequestTopP = "gen_ai.request.top_p";
    public const string RequestTopK = "gen_ai.request.top_k";
    public const string RequestMaxTokens = "gen_ai.request.max_tokens";
    public const string RequestFrequencyPenalty = "gen_ai.request.frequency_penalty";
    public const string RequestPresencePenalty = "gen_ai.request.presence_penalty";
    public const string RequestChoiceCount = "gen_ai.request.choice_count";
    public const string RequestSeed = "gen_ai.request.seed";
    public const string RequestStopSequences = "gen_ai.request.stop_sequences";
    public const string RequestEncodingFormats = "gen_ai.request.encoding_formats";

    // Response attributes
    public const string ResponseModel = "gen_ai.response.model";
    public const string ResponseId = "gen_ai.response.id";
    public const string ResponseFinishReasons = "gen_ai.response.finish_reasons";

    // Usage attributes
    public const string UsageInputTokens = "gen_ai.usage.input_tokens";
    public const string UsageOutputTokens = "gen_ai.usage.output_tokens";

    // Content attributes
    public const string ContentPrompt = "gen_ai.content.prompt";
    public const string ContentCompletion = "gen_ai.content.completion";
    public const string SystemInstructions = "gen_ai.system_instructions";
    public const string OutputType = "gen_ai.output.type";

    // Conversation
    public const string ConversationId = "gen_ai.conversation.id";

    // Tool attributes
    public const string ToolName = "gen_ai.tool.name";
    public const string ToolType = "gen_ai.tool.type";
    public const string ToolDescription = "gen_ai.tool.description";
    public const string ToolDefinitions = "gen_ai.tool.definitions";
    public const string ToolCallId = "gen_ai.tool.call.id";
    public const string ToolCallArguments = "gen_ai.tool.call.arguments";
    public const string ToolCallResult = "gen_ai.tool.call.result";

    // Embeddings attributes
    public const string EmbeddingsDimensionCount = "gen_ai.embeddings.dimension.count";

    // Agent attributes
    public const string AgentId = "gen_ai.agent.id";
    public const string AgentName = "gen_ai.agent.name";
    public const string AgentDescription = "gen_ai.agent.description";
    public const string DataSourceId = "gen_ai.data_source.id";

    // Evaluation attributes
    public const string EvaluationName = "gen_ai.evaluation.name";
    public const string EvaluationScoreValue = "gen_ai.evaluation.score.value";
    public const string EvaluationScoreLabel = "gen_ai.evaluation.score.label";
    public const string EvaluationExplanation = "gen_ai.evaluation.explanation";

    // Server attributes
    public const string ServerAddress = "server.address";
    public const string ServerPort = "server.port";

    // Service attributes
    public const string ServiceName = "service.name";
    public const string ServiceVersion = "service.version";

    // Error attributes
    public const string ErrorType = "error.type";
    public const string ErrorMessage = "error.message";
    public const string ErrorStack = "error.stack";
}