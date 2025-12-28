namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     OpenTelemetry semantic convention attribute names for Gen AI operations.
///     Based on: https://opentelemetry.io/docs/specs/semconv/registry/attributes/gen-ai/
/// </summary>
public static class GenAiAttributes
{
    // Operation & Provider

    /// <summary>
    ///     The name of the operation being performed (e.g., "chat", "text_completion", "embeddings").
    /// </summary>
    public const string OperationName = "gen_ai.operation.name";

    /// <summary>
    ///     The name of the GenAI provider (e.g., "openai", "anthropic", "cohere").
    /// </summary>
    public const string ProviderName = "gen_ai.provider.name";

    // Request attributes

    /// <summary>
    ///     The name of the model being requested (e.g., "gpt-4", "claude-3-opus").
    /// </summary>
    public const string RequestModel = "gen_ai.request.model";

    /// <summary>
    ///     The temperature parameter controlling randomness (0.0 to 2.0).
    /// </summary>
    public const string RequestTemperature = "gen_ai.request.temperature";

    /// <summary>
    ///     The top_p (nucleus sampling) parameter (0.0 to 1.0).
    /// </summary>
    public const string RequestTopP = "gen_ai.request.top_p";

    /// <summary>
    ///     The top_k sampling parameter limiting token selection.
    /// </summary>
    public const string RequestTopK = "gen_ai.request.top_k";

    /// <summary>
    ///     The maximum number of tokens to generate.
    /// </summary>
    public const string RequestMaxTokens = "gen_ai.request.max_tokens";

    /// <summary>
    ///     The frequency penalty parameter (-2.0 to 2.0).
    /// </summary>
    public const string RequestFrequencyPenalty = "gen_ai.request.frequency_penalty";

    /// <summary>
    ///     The presence penalty parameter (-2.0 to 2.0).
    /// </summary>
    public const string RequestPresencePenalty = "gen_ai.request.presence_penalty";

    /// <summary>
    ///     The number of completions to generate (n parameter).
    /// </summary>
    public const string RequestChoiceCount = "gen_ai.request.choice_count";

    /// <summary>
    ///     The seed for deterministic generation.
    /// </summary>
    public const string RequestSeed = "gen_ai.request.seed";

    /// <summary>
    ///     Stop sequences that halt generation.
    /// </summary>
    public const string RequestStopSequences = "gen_ai.request.stop_sequences";

    /// <summary>
    ///     Requested encoding formats for embeddings.
    /// </summary>
    public const string RequestEncodingFormats = "gen_ai.request.encoding_formats";

    // Response attributes

    /// <summary>
    ///     The model that actually generated the response (may differ from requested).
    /// </summary>
    public const string ResponseModel = "gen_ai.response.model";

    /// <summary>
    ///     Unique identifier for the completion response.
    /// </summary>
    public const string ResponseId = "gen_ai.response.id";

    /// <summary>
    ///     Reasons why the model stopped generating (e.g., "stop", "length", "tool_calls").
    /// </summary>
    public const string ResponseFinishReasons = "gen_ai.response.finish_reasons";

    // Usage attributes

    /// <summary>
    ///     Number of tokens in the input/prompt.
    /// </summary>
    public const string UsageInputTokens = "gen_ai.usage.input_tokens";

    /// <summary>
    ///     Number of tokens in the generated output/completion.
    /// </summary>
    public const string UsageOutputTokens = "gen_ai.usage.output_tokens";

    // Content attributes (Official OTel semantic conventions)

    /// <summary>
    ///     The full chat history/messages sent to the model (JSON array).
    ///     Official OTel semantic convention attribute.
    /// </summary>
    public const string InputMessages = "gen_ai.input.messages";

    /// <summary>
    ///     The messages returned by the model (JSON array).
    ///     Official OTel semantic convention attribute.
    /// </summary>
    public const string OutputMessages = "gen_ai.output.messages";

    /// <summary>
    ///     System instructions/prompt provided to the model.
    /// </summary>
    public const string SystemInstructions = "gen_ai.system_instructions";

    /// <summary>
    ///     The requested output modality (e.g., "text", "json").
    /// </summary>
    public const string OutputType = "gen_ai.output.type";

    // Content attributes (Langfuse compatibility)

    /// <summary>
    ///     Input/prompt content. Langfuse maps this to observation input.
    ///     Use this for Langfuse compatibility instead of gen_ai.input.messages.
    ///     See: https://langfuse.com/docs/opentelemetry
    /// </summary>
    public const string Prompt = "gen_ai.prompt";

    /// <summary>
    ///     Output/completion content. Langfuse maps this to observation output.
    ///     Use this for Langfuse compatibility instead of gen_ai.output.messages.
    ///     See: https://langfuse.com/docs/opentelemetry
    /// </summary>
    public const string Completion = "gen_ai.completion";

    // Conversation

    /// <summary>
    ///     Identifier for a conversation/session/thread.
    /// </summary>
    public const string ConversationId = "gen_ai.conversation.id";

    // Tool attributes

    /// <summary>
    ///     The name of the tool being called.
    /// </summary>
    public const string ToolName = "gen_ai.tool.name";

    /// <summary>
    ///     The type/category of the tool (e.g., "function").
    /// </summary>
    public const string ToolType = "gen_ai.tool.type";

    /// <summary>
    ///     Human-readable description of the tool's functionality.
    /// </summary>
    public const string ToolDescription = "gen_ai.tool.description";

    /// <summary>
    ///     Available tool definitions provided to the model (JSON).
    /// </summary>
    public const string ToolDefinitions = "gen_ai.tool.definitions";

    /// <summary>
    ///     Unique identifier for a specific tool call.
    /// </summary>
    public const string ToolCallId = "gen_ai.tool.call.id";

    /// <summary>
    ///     Arguments/parameters passed to the tool (JSON).
    /// </summary>
    public const string ToolCallArguments = "gen_ai.tool.call.arguments";

    /// <summary>
    ///     Result returned from the tool execution (JSON).
    /// </summary>
    public const string ToolCallResult = "gen_ai.tool.call.result";

    // Embeddings attributes

    /// <summary>
    ///     The number of dimensions in the output embeddings.
    /// </summary>
    public const string EmbeddingsDimensionCount = "gen_ai.embeddings.dimension.count";

    // Agent attributes

    /// <summary>
    ///     Unique identifier for the agent.
    /// </summary>
    public const string AgentId = "gen_ai.agent.id";

    /// <summary>
    ///     Human-readable name of the agent.
    /// </summary>
    public const string AgentName = "gen_ai.agent.name";

    /// <summary>
    ///     Description of the agent's functionality and purpose.
    /// </summary>
    public const string AgentDescription = "gen_ai.agent.description";

    /// <summary>
    ///     Identifier for an external data source used by the agent.
    /// </summary>
    public const string DataSourceId = "gen_ai.data_source.id";

    // Evaluation attributes

    /// <summary>
    ///     Name of the evaluation metric being measured.
    /// </summary>
    public const string EvaluationName = "gen_ai.evaluation.name";

    /// <summary>
    ///     Numeric score value from the evaluation.
    /// </summary>
    public const string EvaluationScoreValue = "gen_ai.evaluation.score.value";

    /// <summary>
    ///     Human-readable label for the evaluation score.
    /// </summary>
    public const string EvaluationScoreLabel = "gen_ai.evaluation.score.label";

    /// <summary>
    ///     Explanation or justification for the evaluation score.
    /// </summary>
    public const string EvaluationExplanation = "gen_ai.evaluation.explanation";

    // Server attributes

    /// <summary>
    ///     The server hostname or IP address.
    /// </summary>
    public const string ServerAddress = "server.address";

    /// <summary>
    ///     The server port number.
    /// </summary>
    public const string ServerPort = "server.port";

    // Service attributes

    /// <summary>
    ///     The logical name of the service.
    /// </summary>
    public const string ServiceName = "service.name";

    /// <summary>
    ///     The version of the service.
    /// </summary>
    public const string ServiceVersion = "service.version";

    // Error attributes

    /// <summary>
    ///     The type/category of error that occurred.
    /// </summary>
    public const string ErrorType = "error.type";

    /// <summary>
    ///     Human-readable error message.
    /// </summary>
    public const string ErrorMessage = "error.message";

    /// <summary>
    ///     Stack trace of the error.
    /// </summary>
    public const string ErrorStack = "error.stack";
}
