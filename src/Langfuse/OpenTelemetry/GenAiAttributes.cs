namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     OpenTelemetry semantic convention attribute names for Gen AI operations.
///     Based on: https://github.com/open-telemetry/semantic-conventions-genai
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
    public const string RequestChoiceCount = "gen_ai.request.choice.count";

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

    /// <summary>
    ///     Whether the request was made in streaming mode.
    /// </summary>
    public const string RequestStream = "gen_ai.request.stream";

    /// <summary>
    ///     The reasoning or thinking effort level requested (e.g., "low", "medium", "high").
    /// </summary>
    public const string RequestReasoningLevel = "gen_ai.request.reasoning.level";

    /// <summary>
    ///     The identifier of a previous response used to provide context for this request.
    /// </summary>
    public const string RequestPreviousResponseId = "gen_ai.request.previous_response.id";

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

    /// <summary>
    ///     The lifecycle status of the response (e.g., "queued", "in_progress", "completed", "failed", "cancelled").
    /// </summary>
    public const string ResponseStatus = "gen_ai.response.status";

    /// <summary>
    ///     Seconds from the start of the operation to the first chunk of a streamed response.
    /// </summary>
    public const string ResponseTimeToFirstChunk = "gen_ai.response.time_to_first_chunk";

    // Usage attributes

    /// <summary>
    ///     Number of tokens in the input/prompt.
    /// </summary>
    public const string UsageInputTokens = "gen_ai.usage.input_tokens";

    /// <summary>
    ///     Number of tokens in the generated output/completion.
    /// </summary>
    public const string UsageOutputTokens = "gen_ai.usage.output_tokens";

    /// <summary>
    ///     The number of input tokens that were cached and used for the request.
    /// </summary>
    public const string UsageCacheReadInputTokens = "gen_ai.usage.cache_read.input_tokens";

    /// <summary>
    ///     The number of input tokens written to a provider-managed cache.
    /// </summary>
    public const string UsageCacheWriteInputTokens = "gen_ai.usage.cache_write.input_tokens";

    /// <summary>
    ///     The number of input tokens written to a provider-managed cache.
    /// </summary>
    [Obsolete("Renamed in the GenAI semantic conventions. Use UsageCacheWriteInputTokens.")]
    public const string UsageCacheCreationInputTokens = "gen_ai.usage.cache_creation.input_tokens";

    /// <summary>
    ///     The number of output tokens used for reasoning (chain-of-thought, extended thinking).
    /// </summary>
    public const string UsageReasoningOutputTokens = "gen_ai.usage.reasoning.output_tokens";

    /// <summary>
    ///     The number of text input tokens.
    /// </summary>
    public const string UsageTextInputTokens = "gen_ai.usage.text.input_tokens";

    /// <summary>
    ///     The number of text output tokens.
    /// </summary>
    public const string UsageTextOutputTokens = "gen_ai.usage.text.output_tokens";

    /// <summary>
    ///     The number of text input tokens served from a provider-managed cache.
    /// </summary>
    public const string UsageTextCacheReadInputTokens = "gen_ai.usage.text.cache_read.input_tokens";

    /// <summary>
    ///     The number of audio input tokens.
    /// </summary>
    public const string UsageAudioInputTokens = "gen_ai.usage.audio.input_tokens";

    /// <summary>
    ///     The number of audio output tokens.
    /// </summary>
    public const string UsageAudioOutputTokens = "gen_ai.usage.audio.output_tokens";

    /// <summary>
    ///     The number of audio input tokens served from a provider-managed cache.
    /// </summary>
    public const string UsageAudioCacheReadInputTokens = "gen_ai.usage.audio.cache_read.input_tokens";

    /// <summary>
    ///     The number of image input tokens.
    /// </summary>
    public const string UsageImageInputTokens = "gen_ai.usage.image.input_tokens";

    /// <summary>
    ///     The number of image output tokens.
    /// </summary>
    public const string UsageImageOutputTokens = "gen_ai.usage.image.output_tokens";

    /// <summary>
    ///     The number of image input tokens served from a provider-managed cache.
    /// </summary>
    public const string UsageImageCacheReadInputTokens = "gen_ai.usage.image.cache_read.input_tokens";

    /// <summary>
    ///     The total cost of the operation. Not part of the OTel spec; Langfuse maps it to the "total" cost.
    /// </summary>
    public const string UsageCost = "gen_ai.usage.cost";

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

    /// <summary>
    ///     Absolute URI of the input messages stored externally (instead of inline content).
    /// </summary>
    public const string InputMessagesRef = "gen_ai.input.messages_ref";

    /// <summary>
    ///     Absolute URI of the output messages stored externally (instead of inline content).
    /// </summary>
    public const string OutputMessagesRef = "gen_ai.output.messages_ref";

    /// <summary>
    ///     Absolute URI of the system instructions stored externally (instead of inline content).
    /// </summary>
    public const string SystemInstructionsRef = "gen_ai.system_instructions_ref";

    /// <summary>
    ///     Absolute URI of the tool definitions stored externally (instead of inline content).
    /// </summary>
    public const string ToolDefinitionsRef = "gen_ai.tool.definitions_ref";

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

    /// <summary>
    ///     Whether the conversation context used for the operation is a compacted view of a prior conversation.
    /// </summary>
    public const string ConversationCompacted = "gen_ai.conversation.compacted";

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

    // Transfer (agent handoff) attributes

    /// <summary>
    ///     The name of the agent targeted by a transfer tool call.
    /// </summary>
    public const string TransferTargetName = "gen_ai.transfer.target.name";

    /// <summary>
    ///     How control passes to the transfer target ("return_to_caller" or "pass_control").
    /// </summary>
    public const string TransferMode = "gen_ai.transfer.mode";

    // Skill attributes

    /// <summary>
    ///     The name of the Agent Skill.
    /// </summary>
    public const string SkillName = "gen_ai.skill.name";

    /// <summary>
    ///     The description of the Agent Skill.
    /// </summary>
    public const string SkillDescription = "gen_ai.skill.description";

    /// <summary>
    ///     The source URI the skill was loaded from.
    /// </summary>
    public const string SkillSourceUri = "gen_ai.skill.source.uri";

    /// <summary>
    ///     The skill-relative name of the resource (script, reference, or asset) being accessed.
    /// </summary>
    public const string SkillResourceName = "gen_ai.skill.resource.name";

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
    ///     The version of the agent.
    /// </summary>
    public const string AgentVersion = "gen_ai.agent.version";

    /// <summary>
    ///     Identifier for an external data source used by the agent.
    /// </summary>
    public const string DataSourceId = "gen_ai.data_source.id";

    /// <summary>
    ///     Identifier of the top-level agent in the process.
    /// </summary>
    public const string MainAgentId = "gen_ai.main_agent.id";

    /// <summary>
    ///     Human-readable name of the top-level agent in the process.
    /// </summary>
    public const string MainAgentName = "gen_ai.main_agent.name";

    /// <summary>
    ///     Description of the top-level agent in the process.
    /// </summary>
    public const string MainAgentDescription = "gen_ai.main_agent.description";

    // Prompt attributes

    /// <summary>
    ///     The name of the prompt template used.
    /// </summary>
    public const string PromptName = "gen_ai.prompt.name";

    /// <summary>
    ///     The version of the prompt template used.
    /// </summary>
    public const string PromptVersion = "gen_ai.prompt.version";

    /// <summary>
    ///     Prefix for prompt template variables; the full key is the prefix plus the variable name.
    /// </summary>
    public const string PromptVariablePrefix = "gen_ai.prompt.variable.";

    // Retrieval attributes

    /// <summary>
    ///     The documents retrieved for RAG or similar retrieval-augmented operations.
    /// </summary>
    public const string RetrievalDocuments = "gen_ai.retrieval.documents";

    /// <summary>
    ///     The text of the query used for retrieval.
    /// </summary>
    public const string RetrievalQueryText = "gen_ai.retrieval.query.text";

    /// <summary>
    ///     The maximum number of documents the retriever was asked to return.
    /// </summary>
    public const string RetrievalTopK = "gen_ai.retrieval.top_k";

    // Memory attributes

    /// <summary>
    ///     The unique identifier of the memory store.
    /// </summary>
    public const string MemoryStoreId = "gen_ai.memory.store.id";

    /// <summary>
    ///     The unique identifier of the memory record.
    /// </summary>
    public const string MemoryRecordId = "gen_ai.memory.record.id";

    /// <summary>
    ///     The number of memory records relevant to the operation.
    /// </summary>
    public const string MemoryRecordCount = "gen_ai.memory.record.count";

    /// <summary>
    ///     The search query used to retrieve memories.
    /// </summary>
    public const string MemoryQueryText = "gen_ai.memory.query.text";

    /// <summary>
    ///     The memory records stored or retrieved (JSON).
    /// </summary>
    public const string MemoryRecords = "gen_ai.memory.records";

    // Token attributes

    /// <summary>
    ///     The type of token being counted (e.g., "input", "output").
    /// </summary>
    [Obsolete("Removed from the GenAI semantic conventions together with the gen_ai.client.token.usage metric.")]
    public const string TokenType = "gen_ai.token.type";

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
