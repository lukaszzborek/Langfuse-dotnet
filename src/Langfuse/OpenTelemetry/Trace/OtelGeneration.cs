using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a generation (LLM call) observation.
/// </summary>
public class OtelGeneration : OtelObservation
{
    internal OtelGeneration(Activity? activity) : base(activity)
    {
    }

    /// <summary>
    ///     Records input messages for this generation.
    /// </summary>
    public void SetInputMessages(IEnumerable<GenAiMessage> messages)
    {
        GenAiActivityHelper.RecordInputMessages(Activity, messages);
    }

    /// <summary>
    ///     Records a simple prompt as input.
    /// </summary>
    public void SetPrompt(string prompt)
    {
        GenAiActivityHelper.RecordPrompt(Activity, prompt);
    }

    /// <summary>
    ///     Records the response for this generation.
    /// </summary>
    public void SetResponse(GenAiResponse response)
    {
        GenAiActivityHelper.RecordResponse(Activity, response);
    }

    /// <summary>
    ///     Records a simple completion as output.
    /// </summary>
    public void SetCompletion(string completion)
    {
        GenAiActivityHelper.RecordCompletion(Activity, completion);
    }

    /// <summary>
    ///     Sets prompt reference for this generation.
    /// </summary>
    public void SetPromptReference(string promptName, int? version = null)
    {
        GenAiActivityHelper.SetPromptReference(Activity, promptName, version);
    }

    /// <summary>
    ///     Records completion start time (Time to First Token).
    /// </summary>
    public void RecordCompletionStartTime(DateTimeOffset? startTime = null)
    {
        GenAiActivityHelper.RecordCompletionStartTime(Activity, startTime);
    }

    /// <summary>
    ///     Sets the temperature for this generation request.
    /// </summary>
    public void SetTemperature(double temperature)
    {
        Activity?.SetTag(GenAiAttributes.RequestTemperature, temperature);
    }

    /// <summary>
    ///     Sets the max tokens for this generation request.
    /// </summary>
    public void SetMaxTokens(int maxTokens)
    {
        Activity?.SetTag(GenAiAttributes.RequestMaxTokens, maxTokens);
    }

    /// <summary>
    ///     Sets the top P (nucleus sampling) for this generation request.
    /// </summary>
    public void SetTopP(double topP)
    {
        Activity?.SetTag(GenAiAttributes.RequestTopP, topP);
    }

    /// <summary>
    ///     Sets the top K for this generation request.
    /// </summary>
    public void SetTopK(int topK)
    {
        Activity?.SetTag(GenAiAttributes.RequestTopK, topK);
    }

    /// <summary>
    ///     Sets the frequency penalty for this generation request.
    /// </summary>
    public void SetFrequencyPenalty(double frequencyPenalty)
    {
        Activity?.SetTag(GenAiAttributes.RequestFrequencyPenalty, frequencyPenalty);
    }

    /// <summary>
    ///     Sets the presence penalty for this generation request.
    /// </summary>
    public void SetPresencePenalty(double presencePenalty)
    {
        GenAiActivityHelper.SetPresencePenalty(Activity, presencePenalty);
    }

    /// <summary>
    ///     Sets the request model for this generation.
    /// </summary>
    public void SetRequestModel(string model)
    {
        GenAiActivityHelper.SetRequestModel(Activity, model);
    }

    /// <summary>
    ///     Sets the provider for this generation.
    /// </summary>
    public void SetProvider(string provider)
    {
        GenAiActivityHelper.SetProvider(Activity, provider);
    }

    /// <summary>
    ///     Sets the choice count (n) for this generation request.
    /// </summary>
    public void SetChoiceCount(int choiceCount)
    {
        GenAiActivityHelper.SetChoiceCount(Activity, choiceCount);
    }

    /// <summary>
    ///     Sets the seed for this generation request.
    /// </summary>
    public void SetSeed(int seed)
    {
        GenAiActivityHelper.SetSeed(Activity, seed);
    }

    /// <summary>
    ///     Sets the stop sequences for this generation request.
    /// </summary>
    public void SetStopSequences(string[] stopSequences)
    {
        GenAiActivityHelper.SetStopSequences(Activity, stopSequences);
    }

    /// <summary>
    ///     Sets the output type for this generation request.
    /// </summary>
    public void SetOutputType(string outputType)
    {
        GenAiActivityHelper.SetOutputType(Activity, outputType);
    }

    /// <summary>
    ///     Sets the conversation ID for this generation.
    /// </summary>
    public void SetConversationId(string conversationId)
    {
        GenAiActivityHelper.SetConversationId(Activity, conversationId);
    }

    /// <summary>
    ///     Sets the system instructions for this generation.
    /// </summary>
    public void SetSystemInstructions(string systemInstructions)
    {
        GenAiActivityHelper.SetSystemInstructions(Activity, systemInstructions);
    }

    /// <summary>
    ///     Sets the tool definitions for this generation.
    /// </summary>
    public void SetToolDefinitions(List<GenAiToolDefinition> tools)
    {
        GenAiActivityHelper.SetToolDefinitions(Activity, tools);
    }

    /// <summary>
    ///     Sets the tool definitions for this generation from a raw JSON string.
    /// </summary>
    public void SetToolDefinitions(string toolDefinitionsJson)
    {
        GenAiActivityHelper.SetToolDefinitions(Activity, toolDefinitionsJson);
    }

    /// <summary>
    ///     Sets the server address for this generation.
    /// </summary>
    public void SetServerAddress(string serverAddress)
    {
        GenAiActivityHelper.SetServerAddress(Activity, serverAddress);
    }

    /// <summary>
    ///     Sets the server port for this generation.
    /// </summary>
    public void SetServerPort(int serverPort)
    {
        GenAiActivityHelper.SetServerPort(Activity, serverPort);
    }

    /// <summary>
    ///     Sets the usage details for this generation.
    /// </summary>
    public void SetUsageDetails(Dictionary<string, long> usageDetails)
    {
        GenAiActivityHelper.SetUsageDetails(Activity, usageDetails);
    }

    /// <summary>
    ///     Sets the number of cached input tokens read from a provider-managed cache.
    /// </summary>
    public void SetCacheReadInputTokens(int cacheReadInputTokens)
    {
        GenAiActivityHelper.SetCacheReadInputTokens(Activity, cacheReadInputTokens);
    }

    /// <summary>
    ///     Sets the number of input tokens written to a provider-managed cache.
    /// </summary>
    public void SetCacheWriteInputTokens(int cacheWriteInputTokens)
    {
        GenAiActivityHelper.SetCacheWriteInputTokens(Activity, cacheWriteInputTokens);
    }

    /// <summary>
    ///     Sets the number of input tokens written to a provider-managed cache.
    /// </summary>
    [Obsolete("Renamed in the GenAI semantic conventions. Use SetCacheWriteInputTokens.")]
    public void SetCacheCreationInputTokens(int cacheCreationInputTokens)
    {
        SetCacheWriteInputTokens(cacheCreationInputTokens);
    }

    /// <summary>
    ///     Sets the number of output tokens used for reasoning.
    /// </summary>
    public void SetReasoningOutputTokens(int reasoningOutputTokens)
    {
        GenAiActivityHelper.SetReasoningOutputTokens(Activity, reasoningOutputTokens);
    }

    /// <summary>
    ///     Sets the total cost of this generation. Langfuse stores it as the "total" cost.
    /// </summary>
    public void SetTotalCost(decimal totalCost)
    {
        GenAiActivityHelper.SetTotalCost(Activity, totalCost);
    }

    /// <summary>
    ///     Sets whether this generation request was made in streaming mode.
    /// </summary>
    public void SetStream(bool stream)
    {
        Activity?.SetTag(GenAiAttributes.RequestStream, stream);
    }

    /// <summary>
    ///     Sets the requested reasoning or thinking effort level (e.g., "low", "medium", "high").
    /// </summary>
    public void SetReasoningLevel(string reasoningLevel)
    {
        Activity?.SetTag(GenAiAttributes.RequestReasoningLevel, reasoningLevel);
    }

    /// <summary>
    ///     Sets the identifier of a previous response used to provide context for this request.
    /// </summary>
    public void SetPreviousResponseId(string previousResponseId)
    {
        Activity?.SetTag(GenAiAttributes.RequestPreviousResponseId, previousResponseId);
    }

    /// <summary>
    ///     Sets the lifecycle status of the response (e.g., "queued", "in_progress", "completed", "failed", "cancelled").
    /// </summary>
    public void SetResponseStatus(string status)
    {
        Activity?.SetTag(GenAiAttributes.ResponseStatus, status);
    }

    /// <summary>
    ///     Sets the time from the start of the operation to the first chunk of a streamed response.
    /// </summary>
    public void SetTimeToFirstChunk(TimeSpan timeToFirstChunk)
    {
        Activity?.SetTag(GenAiAttributes.ResponseTimeToFirstChunk, timeToFirstChunk.TotalSeconds);
    }

    /// <summary>
    ///     Marks whether the conversation context is a compacted view of a prior conversation.
    /// </summary>
    public void SetConversationCompacted(bool compacted)
    {
        Activity?.SetTag(GenAiAttributes.ConversationCompacted, compacted);
    }

    /// <summary>
    ///     Sets the prompt template variables used for this generation.
    /// </summary>
    public void SetPromptVariables(IEnumerable<KeyValuePair<string, string>> variables)
    {
        GenAiActivityHelper.SetPromptVariables(Activity, variables);
    }

    /// <summary>
    ///     Sets the URI of input messages stored externally (instead of inline content).
    /// </summary>
    public void SetInputMessagesRef(string uri)
    {
        Activity?.SetTag(GenAiAttributes.InputMessagesRef, uri);
    }

    /// <summary>
    ///     Sets the URI of output messages stored externally (instead of inline content).
    /// </summary>
    public void SetOutputMessagesRef(string uri)
    {
        Activity?.SetTag(GenAiAttributes.OutputMessagesRef, uri);
    }

    /// <summary>
    ///     Sets the URI of system instructions stored externally (instead of inline content).
    /// </summary>
    public void SetSystemInstructionsRef(string uri)
    {
        Activity?.SetTag(GenAiAttributes.SystemInstructionsRef, uri);
    }

    /// <summary>
    ///     Sets the URI of tool definitions stored externally (instead of inline content).
    /// </summary>
    public void SetToolDefinitionsRef(string uri)
    {
        Activity?.SetTag(GenAiAttributes.ToolDefinitionsRef, uri);
    }

    /// <summary>
    ///     Sets the cost details for this generation.
    /// </summary>
    public void SetCostDetails(Dictionary<string, decimal> costDetails)
    {
        GenAiActivityHelper.SetCostDetails(Activity, costDetails);
    }
}