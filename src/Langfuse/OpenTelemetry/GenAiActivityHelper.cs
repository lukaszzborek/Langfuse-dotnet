using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Helper class for creating OpenTelemetry activities with Gen AI semantic conventions.
///     Based on: https://opentelemetry.io/docs/specs/semconv/registry/attributes/gen-ai/
/// </summary>
public static class GenAiActivityHelper
{
    // JSON serializer options for message serialization
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    /// <summary>
    ///     Creates and configures an activity for a Gen AI chat completion operation.
    /// </summary>
    public static Activity? CreateChatCompletionActivity(
        ActivitySource activitySource,
        string operationName,
        GenAiChatCompletionConfig config)
    {
        var activity = activitySource.StartActivity(operationName, ActivityKind.Client);

        if (activity == null)
        {
            return null;
        }

        // Langfuse observation type - explicitly mark as generation
        activity.SetTag(LangfuseObservationType, "generation");

        // Operation metadata
        activity.SetTag(GenAiOperationName, "chat");
        activity.SetTag(GenAiProviderName, config.Provider);
        activity.SetTag(GenAiRequestModel, config.Model);

        // Request parameters
        if (config.Temperature.HasValue)
        {
            activity.SetTag(GenAiRequestTemperature, config.Temperature.Value);
        }

        if (config.TopP.HasValue)
        {
            activity.SetTag(GenAiRequestTopP, config.TopP.Value);
        }

        if (config.TopK.HasValue)
        {
            activity.SetTag(GenAiRequestTopK, config.TopK.Value);
        }

        if (config.MaxTokens.HasValue)
        {
            activity.SetTag(GenAiRequestMaxTokens, config.MaxTokens.Value);
        }

        if (config.FrequencyPenalty.HasValue)
        {
            activity.SetTag(GenAiRequestFrequencyPenalty, config.FrequencyPenalty.Value);
        }

        if (config.PresencePenalty.HasValue)
        {
            activity.SetTag(GenAiRequestPresencePenalty, config.PresencePenalty.Value);
        }

        if (config.ChoiceCount.HasValue)
        {
            activity.SetTag(GenAiRequestChoiceCount, config.ChoiceCount.Value);
        }

        if (config.Seed.HasValue)
        {
            activity.SetTag(GenAiRequestSeed, config.Seed.Value);
        }

        if (config.StopSequences != null && config.StopSequences.Length > 0)
        {
            activity.SetTag(GenAiRequestStopSequences, JsonSerializer.Serialize(config.StopSequences, JsonOptions));
        }

        if (!string.IsNullOrEmpty(config.OutputType))
        {
            activity.SetTag(GenAiOutputType, config.OutputType);
        }

        if (!string.IsNullOrEmpty(config.ConversationId))
        {
            activity.SetTag(GenAiConversationId, config.ConversationId);
        }

        if (!string.IsNullOrEmpty(config.SystemInstructions))
        {
            activity.SetTag(GenAiSystemInstructions, config.SystemInstructions);
        }

        // Tool definitions
        if (config.Tools != null && config.Tools.Count > 0)
        {
            activity.SetTag(GenAiToolDefinitions, JsonSerializer.Serialize(config.Tools, JsonOptions));
        }

        // Server info
        if (!string.IsNullOrEmpty(config.ServerAddress))
        {
            activity.SetTag(ServerAddress, config.ServerAddress);
        }

        if (config.ServerPort.HasValue)
        {
            activity.SetTag(ServerPort, config.ServerPort.Value);
        }

        // Langfuse-specific attributes
        if (!string.IsNullOrEmpty(config.PromptName))
        {
            activity.SetTag(LangfuseObservationPromptName, config.PromptName);
        }

        if (config.PromptVersion.HasValue)
        {
            activity.SetTag(LangfuseObservationPromptVersion, config.PromptVersion.Value);
        }

        if (config.Level.HasValue)
        {
            activity.SetTag(LangfuseObservationLevel, config.Level.Value.ToString().ToUpperInvariant());
        }

        if (config.Metadata != null && config.Metadata.Count > 0)
        {
            foreach (KeyValuePair<string, object> kvp in config.Metadata)
            {
                activity.SetTag($"{LangfuseObservationMetadataPrefix}{kvp.Key}", kvp.Value);
            }
        }

        return activity;
    }

    /// <summary>
    ///     Creates and configures an activity for a Gen AI text completion operation.
    /// </summary>
    public static Activity? CreateTextCompletionActivity(
        ActivitySource activitySource,
        string operationName,
        GenAiTextCompletionConfig config)
    {
        var activity = activitySource.StartActivity(operationName, ActivityKind.Client);

        if (activity == null)
        {
            return null;
        }

        // Langfuse observation type - explicitly mark as generation
        activity.SetTag(LangfuseObservationType, "generation");

        activity.SetTag(GenAiOperationName, "text_completion");
        activity.SetTag(GenAiProviderName, config.Provider);
        activity.SetTag(GenAiRequestModel, config.Model);

        if (config.Temperature.HasValue)
        {
            activity.SetTag(GenAiRequestTemperature, config.Temperature.Value);
        }

        if (config.MaxTokens.HasValue)
        {
            activity.SetTag(GenAiRequestMaxTokens, config.MaxTokens.Value);
        }

        return activity;
    }

    /// <summary>
    ///     Creates and configures an activity for a Gen AI embeddings operation.
    /// </summary>
    public static Activity? CreateEmbeddingsActivity(
        ActivitySource activitySource,
        string operationName,
        string provider,
        string model)
    {
        return CreateEmbeddingsActivity(activitySource, operationName, new GenAiEmbeddingsConfig
        {
            Provider = provider,
            Model = model
        });
    }

    /// <summary>
    ///     Creates and configures an activity for a Gen AI embeddings operation with full configuration.
    /// </summary>
    public static Activity? CreateEmbeddingsActivity(
        ActivitySource activitySource,
        string operationName,
        GenAiEmbeddingsConfig config)
    {
        var activity = activitySource.StartActivity(operationName, ActivityKind.Client);

        if (activity == null)
        {
            return null;
        }

        // Langfuse observation type - embeddings have their own type
        activity.SetTag(LangfuseObservationType, "embedding");

        activity.SetTag(GenAiOperationName, "embeddings");
        activity.SetTag(GenAiProviderName, config.Provider);
        activity.SetTag(GenAiRequestModel, config.Model);

        if (config.Dimensions.HasValue)
        {
            activity.SetTag(GenAiEmbeddingsDimensionCount, config.Dimensions.Value);
        }

        if (config.EncodingFormats != null && config.EncodingFormats.Length > 0)
        {
            activity.SetTag(GenAiRequestEncodingFormats, JsonSerializer.Serialize(config.EncodingFormats, JsonOptions));
        }

        if (!string.IsNullOrEmpty(config.ServerAddress))
        {
            activity.SetTag(ServerAddress, config.ServerAddress);
        }

        if (config.ServerPort.HasValue)
        {
            activity.SetTag(ServerPort, config.ServerPort.Value);
        }

        return activity;
    }

    /// <summary>
    ///     Creates and configures an activity for a Gen AI tool/function call operation.
    /// </summary>
    public static Activity? CreateToolCallActivity(
        ActivitySource activitySource,
        string operationName,
        string toolName,
        string? toolDescription = null,
        string toolType = "function",
        string? toolCallId = null)
    {
        var activity = activitySource.StartActivity(operationName);

        if (activity == null)
        {
            return null;
        }

        // Langfuse observation type - tool calls have their own type
        activity.SetTag(LangfuseObservationType, "tool");

        activity.SetTag(GenAiOperationName, "execute_tool");
        activity.SetTag(GenAiToolName, toolName);
        activity.SetTag(GenAiToolType, toolType);

        if (!string.IsNullOrEmpty(toolDescription))
        {
            activity.SetTag(GenAiToolDescription, toolDescription);
        }

        if (!string.IsNullOrEmpty(toolCallId))
        {
            activity.SetTag(GenAiToolCallId, toolCallId);
        }

        return activity;
    }

    /// <summary>
    ///     Records tool call arguments on an activity.
    /// </summary>
    public static void RecordToolCallArguments(Activity? activity, string arguments)
    {
        activity?.SetTag(GenAiToolCallArguments, arguments);
    }

    /// <summary>
    ///     Records tool call arguments on an activity (serializes object to JSON).
    /// </summary>
    public static void RecordToolCallArguments(Activity? activity, object arguments)
    {
        if (activity == null)
        {
            return;
        }

        var json = JsonSerializer.Serialize(arguments, JsonOptions);
        activity.SetTag(GenAiToolCallArguments, json);
    }

    /// <summary>
    ///     Records tool call result on an activity.
    /// </summary>
    public static void RecordToolCallResult(Activity? activity, string result)
    {
        activity?.SetTag(GenAiToolCallResult, result);
    }

    /// <summary>
    ///     Records tool call result on an activity (serializes object to JSON).
    /// </summary>
    public static void RecordToolCallResult(Activity? activity, object result)
    {
        if (activity == null)
        {
            return;
        }

        var json = JsonSerializer.Serialize(result, JsonOptions);
        activity.SetTag(GenAiToolCallResult, json);
    }

    /// <summary>
    ///     Creates and configures an activity for a Gen AI agent operation.
    /// </summary>
    public static Activity? CreateAgentActivity(
        ActivitySource activitySource,
        string operationName,
        GenAiAgentConfig config)
    {
        var activity = activitySource.StartActivity(operationName);

        if (activity == null)
        {
            return null;
        }

        // Langfuse observation type - agents have their own type
        activity.SetTag(LangfuseObservationType, "agent");

        activity.SetTag(GenAiOperationName, "create_agent");
        activity.SetTag(GenAiAgentId, config.Id);
        activity.SetTag(GenAiAgentName, config.Name);

        if (!string.IsNullOrEmpty(config.Description))
        {
            activity.SetTag(GenAiAgentDescription, config.Description);
        }

        return activity;
    }

    /// <summary>
    ///     Creates and configures an activity for invoking a Gen AI agent.
    /// </summary>
    public static Activity? CreateInvokeAgentActivity(
        ActivitySource activitySource,
        string operationName,
        string agentId,
        string? agentName = null)
    {
        var activity = activitySource.StartActivity(operationName, ActivityKind.Client);

        if (activity == null)
        {
            return null;
        }

        // Langfuse observation type - agents have their own type
        activity.SetTag(LangfuseObservationType, "agent");

        activity.SetTag(GenAiOperationName, "invoke_agent");
        activity.SetTag(GenAiAgentId, agentId);

        if (!string.IsNullOrEmpty(agentName))
        {
            activity.SetTag(GenAiAgentName, agentName);
        }

        return activity;
    }

    /// <summary>
    ///     Sets the data source ID on an activity (for RAG/knowledge base operations).
    /// </summary>
    public static void SetDataSource(Activity? activity, string dataSourceId)
    {
        activity?.SetTag(GenAiDataSourceId, dataSourceId);
    }

    /// <summary>
    ///     Records an evaluation/score on a Gen AI activity.
    /// </summary>
    public static void RecordEvaluation(Activity? activity, GenAiEvaluation evaluation)
    {
        if (activity == null)
        {
            return;
        }

        activity.SetTag(GenAiEvaluationName, evaluation.Name);

        if (evaluation.ScoreValue.HasValue)
        {
            activity.SetTag(GenAiEvaluationScoreValue, evaluation.ScoreValue.Value);
        }

        if (!string.IsNullOrEmpty(evaluation.ScoreLabel))
        {
            activity.SetTag(GenAiEvaluationScoreLabel, evaluation.ScoreLabel);
        }

        if (!string.IsNullOrEmpty(evaluation.Explanation))
        {
            activity.SetTag(GenAiEvaluationExplanation, evaluation.Explanation);
        }
    }

    /// <summary>
    ///     Records an evaluation/score on a Gen AI activity (convenience overload).
    /// </summary>
    public static void RecordEvaluation(
        Activity? activity,
        string name,
        double score,
        string? label = null,
        string? explanation = null)
    {
        RecordEvaluation(activity, new GenAiEvaluation
        {
            Name = name,
            ScoreValue = score,
            ScoreLabel = label,
            Explanation = explanation
        });
    }

    /// <summary>
    ///     Sets observation-level input on an activity (Langfuse).
    /// </summary>
    public static void SetObservationInput(Activity? activity, object input)
    {
        if (activity == null)
        {
            return;
        }

        var json = input is string s ? s : JsonSerializer.Serialize(input, JsonOptions);
        activity.SetTag(LangfuseObservationInput, json);
    }

    /// <summary>
    ///     Sets observation-level output on an activity (Langfuse).
    /// </summary>
    public static void SetObservationOutput(Activity? activity, object output)
    {
        if (activity == null)
        {
            return;
        }

        var json = output is string s ? s : JsonSerializer.Serialize(output, JsonOptions);
        activity.SetTag(LangfuseObservationOutput, json);
    }

    /// <summary>
    ///     Sets the observation level/severity on an activity (Langfuse).
    /// </summary>
    public static void SetObservationLevel(Activity? activity, LangfuseObservationLevel level)
    {
        activity?.SetTag(LangfuseObservationLevel, level.ToString().ToUpperInvariant());
    }

    /// <summary>
    ///     Sets observation metadata on an activity (Langfuse).
    /// </summary>
    public static void SetObservationMetadata(Activity? activity, string key, object value)
    {
        activity?.SetTag($"{LangfuseObservationMetadataPrefix}{key}", value);
    }

    /// <summary>
    ///     Sets a prompt reference on an activity (Langfuse).
    /// </summary>
    public static void SetPromptReference(Activity? activity, string promptName, int? version = null)
    {
        if (activity == null)
        {
            return;
        }

        activity.SetTag(LangfuseObservationPromptName, promptName);

        if (version.HasValue)
        {
            activity.SetTag(LangfuseObservationPromptVersion, version.Value);
        }
    }

    /// <summary>
    ///     Records the completion start time (Time to First Token) on an activity.
    /// </summary>
    public static void RecordCompletionStartTime(Activity? activity, DateTimeOffset? startTime = null)
    {
        if (activity == null)
        {
            return;
        }

        var time = startTime ?? DateTimeOffset.UtcNow;
        activity.SetTag(LangfuseObservationCompletionStartTime, time.ToString("O"));
    }

    /// <summary>
    ///     Sets trace-level input on an activity (Langfuse).
    /// </summary>
    public static void SetTraceInput(Activity? activity, object input)
    {
        if (activity == null)
        {
            return;
        }

        var json = input is string s ? s : JsonSerializer.Serialize(input, JsonOptions);
        activity.SetTag(LangfuseTraceInput, json);
    }

    /// <summary>
    ///     Sets trace-level output on an activity (Langfuse).
    /// </summary>
    public static void SetTraceOutput(Activity? activity, object output)
    {
        if (activity == null)
        {
            return;
        }

        var json = output is string s ? s : JsonSerializer.Serialize(output, JsonOptions);
        activity.SetTag(LangfuseTraceOutput, json);
    }

    /// <summary>
    ///     Sets trace tags on an activity (Langfuse).
    /// </summary>
    public static void SetTraceTags(Activity? activity, IEnumerable<string> tags)
    {
        if (activity == null)
        {
            return;
        }

        List<string> tagList = tags.ToList();
        if (tagList.Count > 0)
        {
            activity.SetTag(LangfuseTraceTags, JsonSerializer.Serialize(tagList, JsonOptions));
        }
    }

    /// <summary>
    ///     Records response information on an existing Gen AI activity.
    /// </summary>
    public static void RecordResponse(
        Activity? activity,
        GenAiResponse response)
    {
        if (activity == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(response.ResponseId))
        {
            activity.SetTag(GenAiResponseId, response.ResponseId);
        }

        if (!string.IsNullOrEmpty(response.Model))
        {
            activity.SetTag(GenAiResponseModel, response.Model);
        }

        if (response.FinishReasons != null && response.FinishReasons.Length > 0)
        {
            activity.SetTag(GenAiResponseFinishReasons, JsonSerializer.Serialize(response.FinishReasons, JsonOptions));
        }

        if (response.InputTokens.HasValue)
        {
            activity.SetTag(GenAiUsageInputTokens, response.InputTokens.Value);
        }

        if (response.OutputTokens.HasValue)
        {
            activity.SetTag(GenAiUsageOutputTokens, response.OutputTokens.Value);
        }

        if (response.UsageDetails != null && response.UsageDetails.Count > 0)
        {
            activity.SetTag(LangfuseObservationUsageDetails,
                JsonSerializer.Serialize(response.UsageDetails, JsonOptions));
        }

        if (response.TotalCost.HasValue || response.InputCost.HasValue || response.OutputCost.HasValue ||
            (response.CostDetails != null && response.CostDetails.Count > 0))
        {
            Dictionary<string, decimal> costDetails = response.CostDetails ?? new Dictionary<string, decimal>();
            if (response.InputCost.HasValue && !costDetails.ContainsKey("input"))
            {
                costDetails["input"] = response.InputCost.Value;
            }

            if (response.OutputCost.HasValue && !costDetails.ContainsKey("output"))
            {
                costDetails["output"] = response.OutputCost.Value;
            }

            if (response.TotalCost.HasValue && !costDetails.ContainsKey("total"))
            {
                costDetails["total"] = response.TotalCost.Value;
            }

            activity.SetTag(LangfuseObservationCostDetails, JsonSerializer.Serialize(costDetails, JsonOptions));
        }

        if (response.CompletionStartTime.HasValue)
        {
            activity.SetTag(LangfuseObservationCompletionStartTime, response.CompletionStartTime.Value.ToString("O"));
        }

        if (response.OutputMessages != null && response.OutputMessages.Count > 0)
        {
            RecordOutputMessages(activity, response.OutputMessages);
        }
        else if (!string.IsNullOrEmpty(response.Completion))
        {
            RecordCompletion(activity, response.Completion);
        }
    }

    /// <summary>
    ///     Records an error on a Gen AI activity with proper status and error details.
    /// </summary>
    public static void RecordError(Activity activity, Exception exception)
    {
        activity.SetStatus(ActivityStatusCode.Error, exception.Message);
        activity.SetTag("error.type", exception.GetType().FullName);
        activity.SetTag("error.message", exception.Message);

        if (!string.IsNullOrEmpty(exception.StackTrace))
        {
            activity.SetTag("error.stack", exception.StackTrace);
        }

        activity.AddEvent(new ActivityEvent("exception",
            tags: new ActivityTagsCollection
            {
                { "exception.type", exception.GetType().FullName },
                { "exception.message", exception.Message }
            }));
    }

    /// <summary>
    ///     Records input messages (prompt/chat history) on a Gen AI activity.
    ///     Messages are serialized to JSON following OpenTelemetry semantic conventions.
    ///     Also sets Langfuse-specific input attribute for dashboard visibility.
    /// </summary>
    /// <param name="activity">The activity to record messages on.</param>
    /// <param name="messages">The input messages to record.</param>
    public static void RecordInputMessages(Activity? activity, IEnumerable<GenAiMessage> messages)
    {
        if (activity == null)
        {
            return;
        }

        List<GenAiMessage> messageList = messages.ToList();
        if (messageList.Count == 0)
        {
            return;
        }

        var json = JsonSerializer.Serialize(messageList, JsonOptions);
        activity.SetTag(GenAiContentPrompt, json);
        activity.SetTag(LangfuseObservationInput, json);
    }

    /// <summary>
    ///     Records a single input message (prompt) on a Gen AI activity.
    /// </summary>
    /// <param name="activity">The activity to record the message on.</param>
    /// <param name="message">The input message to record.</param>
    public static void RecordInputMessage(Activity? activity, GenAiMessage message)
    {
        RecordInputMessages(activity, [message]);
    }

    /// <summary>
    ///     Records a simple user prompt as an input message on a Gen AI activity.
    /// </summary>
    /// <param name="activity">The activity to record the prompt on.</param>
    /// <param name="prompt">The user prompt text.</param>
    public static void RecordPrompt(Activity? activity, string prompt)
    {
        if (activity == null || string.IsNullOrEmpty(prompt))
        {
            return;
        }

        RecordInputMessage(activity, new GenAiMessage { Role = "user", Content = prompt });
    }

    /// <summary>
    ///     Records output messages (completions) on a Gen AI activity.
    ///     Messages are serialized to JSON following OpenTelemetry semantic conventions.
    ///     Also sets Langfuse-specific output attribute for dashboard visibility.
    /// </summary>
    /// <param name="activity">The activity to record messages on.</param>
    /// <param name="messages">The output messages to record.</param>
    public static void RecordOutputMessages(Activity? activity, IEnumerable<GenAiMessage> messages)
    {
        if (activity == null)
        {
            return;
        }

        List<GenAiMessage> messageList = messages.ToList();
        if (messageList.Count == 0)
        {
            return;
        }

        var json = JsonSerializer.Serialize(messageList, JsonOptions);
        activity.SetTag(GenAiContentCompletion, json);
        activity.SetTag(LangfuseObservationOutput, json);
    }

    /// <summary>
    ///     Records a single output message (completion) on a Gen AI activity.
    /// </summary>
    /// <param name="activity">The activity to record the message on.</param>
    /// <param name="message">The output message to record.</param>
    public static void RecordOutputMessage(Activity? activity, GenAiMessage message)
    {
        RecordOutputMessages(activity, [message]);
    }

    /// <summary>
    ///     Records a simple assistant completion as an output message on a Gen AI activity.
    /// </summary>
    /// <param name="activity">The activity to record the completion on.</param>
    /// <param name="completion">The assistant completion text.</param>
    public static void RecordCompletion(Activity? activity, string completion)
    {
        if (activity == null || string.IsNullOrEmpty(completion))
        {
            return;
        }

        RecordOutputMessage(activity, new GenAiMessage { Role = "assistant", Content = completion });
    }

    /// <summary>
    ///     Records both input and output messages on a Gen AI activity in a single call.
    /// </summary>
    /// <param name="activity">The activity to record messages on.</param>
    /// <param name="inputMessages">The input messages (prompt/chat history).</param>
    /// <param name="outputMessages">The output messages (completions).</param>
    public static void RecordMessages(
        Activity? activity,
        IEnumerable<GenAiMessage> inputMessages,
        IEnumerable<GenAiMessage> outputMessages)
    {
        RecordInputMessages(activity, inputMessages);
        RecordOutputMessages(activity, outputMessages);
    }

    /// <summary>
    ///     Creates and configures a root trace activity for distributed tracing.
    /// </summary>
    public static Activity? CreateTraceActivity(
        ActivitySource activitySource,
        string traceName,
        TraceConfig config)
    {
        var activity = activitySource.StartActivity(traceName);

        if (activity == null)
        {
            return null;
        }

        // Always set trace name - use config.Name if provided, otherwise use traceName parameter
        activity.SetTag(LangfuseTraceName, config.Name ?? traceName);

        if (!string.IsNullOrEmpty(config.TraceId))
        {
            activity.SetTag(LangfuseTraceId, config.TraceId);
        }

        if (!string.IsNullOrEmpty(config.UserId))
        {
            activity.SetTag(LangfuseUserId, config.UserId);
        }

        if (!string.IsNullOrEmpty(config.SessionId))
        {
            activity.SetTag(LangfuseSessionId, config.SessionId);
        }

        if (!string.IsNullOrEmpty(config.Environment))
        {
            activity.SetTag(LangfuseEnvironment, config.Environment);
        }

        if (!string.IsNullOrEmpty(config.Release))
        {
            activity.SetTag(LangfuseRelease, config.Release);
        }

        if (!string.IsNullOrEmpty(config.Version))
        {
            activity.SetTag(LangfuseVersion, config.Version);
        }

        if (!string.IsNullOrEmpty(config.ServiceName))
        {
            activity.SetTag("service.name", config.ServiceName);
        }

        if (!string.IsNullOrEmpty(config.ServiceVersion))
        {
            activity.SetTag("service.version", config.ServiceVersion);
        }

        if (config.Public.HasValue)
        {
            activity.SetTag(LangfuseTracePublic, config.Public.Value);
        }

        if (config.Tags != null && config.Tags.Count > 0)
        {
            activity.SetTag(LangfuseTraceTags, JsonSerializer.Serialize(config.Tags, JsonOptions));
        }

        if (config.Input != null)
        {
            var inputJson = config.Input is string s ? s : JsonSerializer.Serialize(config.Input, JsonOptions);
            activity.SetTag(LangfuseTraceInput, inputJson);
        }

        if (config.Output != null)
        {
            var outputJson = config.Output is string s ? s : JsonSerializer.Serialize(config.Output, JsonOptions);
            activity.SetTag(LangfuseTraceOutput, outputJson);
        }

        if (config.Metadata != null && config.Metadata.Count > 0)
        {
            foreach (KeyValuePair<string, object> kvp in config.Metadata)
            {
                activity.SetTag($"{LangfuseTraceMetadataPrefix}{kvp.Key}", kvp.Value);
            }
        }

        return activity;
    }

    /// <summary>
    ///     Creates and configures a general span activity within a trace for tracking operations.
    /// </summary>
    public static Activity? CreateSpanActivity(
        ActivitySource activitySource,
        string spanName,
        SpanConfig config,
        Activity? parentActivity = null)
    {
        var activity = parentActivity == null
            ? activitySource.StartActivity(spanName)
            : activitySource.StartActivity(spanName, ActivityKind.Internal, parentActivity.Context);

        if (activity == null)
        {
            return null;
        }

        // Set span-level attributes
        if (!string.IsNullOrEmpty(config.SpanType))
        {
            activity.SetTag("span.type", config.SpanType);
        }

        if (!string.IsNullOrEmpty(config.Description))
        {
            activity.SetTag("span.description", config.Description);
        }

        if (config.Attributes != null && config.Attributes.Count > 0)
        {
            foreach (KeyValuePair<string, object> kvp in config.Attributes)
            {
                activity.SetTag(kvp.Key, kvp.Value);
            }
        }

        return activity;
    }

    /// <summary>
    ///     Sets trace-level attributes on an existing trace activity for cross-span propagation.
    /// </summary>
    public static void SetTraceAttributes(Activity traceActivity, TraceAttributesConfig config)
    {
        if (!string.IsNullOrEmpty(config.UserId))
        {
            traceActivity.SetTag("user.id", config.UserId);
        }

        if (!string.IsNullOrEmpty(config.SessionId))
        {
            traceActivity.SetTag("session.id", config.SessionId);
        }

        if (!string.IsNullOrEmpty(config.Environment))
        {
            traceActivity.SetTag("deployment.environment", config.Environment);
        }

        if (config.Tags != null && config.Tags.Count > 0)
        {
            foreach (var tag in config.Tags)
            {
                traceActivity.SetTag($"langfuse.trace.tag.{tag}", tag);
            }
        }

        if (config.Metadata != null && config.Metadata.Count > 0)
        {
            foreach (KeyValuePair<string, object> kvp in config.Metadata)
            {
                traceActivity.SetTag($"langfuse.trace.metadata.{kvp.Key}", kvp.Value);
            }
        }
    }

    #region OpenTelemetry Gen AI Semantic Convention Constants

    // Operation & Provider
    private const string GenAiOperationName = "gen_ai.operation.name";
    private const string GenAiProviderName = "gen_ai.provider.name";

    // Request attributes
    private const string GenAiRequestModel = "gen_ai.request.model";
    private const string GenAiRequestTemperature = "gen_ai.request.temperature";
    private const string GenAiRequestTopP = "gen_ai.request.top_p";
    private const string GenAiRequestTopK = "gen_ai.request.top_k";
    private const string GenAiRequestMaxTokens = "gen_ai.request.max_tokens";
    private const string GenAiRequestFrequencyPenalty = "gen_ai.request.frequency_penalty";
    private const string GenAiRequestPresencePenalty = "gen_ai.request.presence_penalty";
    private const string GenAiRequestChoiceCount = "gen_ai.request.choice_count";
    private const string GenAiRequestSeed = "gen_ai.request.seed";
    private const string GenAiRequestStopSequences = "gen_ai.request.stop_sequences";
    private const string GenAiRequestEncodingFormats = "gen_ai.request.encoding_formats";

    // Response attributes
    private const string GenAiResponseModel = "gen_ai.response.model";
    private const string GenAiResponseId = "gen_ai.response.id";
    private const string GenAiResponseFinishReasons = "gen_ai.response.finish_reasons";

    // Usage attributes
    private const string GenAiUsageInputTokens = "gen_ai.usage.input_tokens";
    private const string GenAiUsageOutputTokens = "gen_ai.usage.output_tokens";

    // Content attributes
    private const string GenAiContentPrompt = "gen_ai.content.prompt";
    private const string GenAiContentCompletion = "gen_ai.content.completion";
    private const string GenAiSystemInstructions = "gen_ai.system_instructions";
    private const string GenAiOutputType = "gen_ai.output.type";

    // Conversation
    private const string GenAiConversationId = "gen_ai.conversation.id";

    // Tool attributes
    private const string GenAiToolName = "gen_ai.tool.name";
    private const string GenAiToolType = "gen_ai.tool.type";
    private const string GenAiToolDescription = "gen_ai.tool.description";
    private const string GenAiToolDefinitions = "gen_ai.tool.definitions";
    private const string GenAiToolCallId = "gen_ai.tool.call.id";
    private const string GenAiToolCallArguments = "gen_ai.tool.call.arguments";
    private const string GenAiToolCallResult = "gen_ai.tool.call.result";

    // Embeddings attributes
    private const string GenAiEmbeddingsDimensionCount = "gen_ai.embeddings.dimension.count";

    // Agent attributes
    private const string GenAiAgentId = "gen_ai.agent.id";
    private const string GenAiAgentName = "gen_ai.agent.name";
    private const string GenAiAgentDescription = "gen_ai.agent.description";
    private const string GenAiDataSourceId = "gen_ai.data_source.id";

    // Evaluation attributes
    private const string GenAiEvaluationName = "gen_ai.evaluation.name";
    private const string GenAiEvaluationScoreValue = "gen_ai.evaluation.score.value";
    private const string GenAiEvaluationScoreLabel = "gen_ai.evaluation.score.label";
    private const string GenAiEvaluationExplanation = "gen_ai.evaluation.explanation";

    // Server attributes
    private const string ServerAddress = "server.address";
    private const string ServerPort = "server.port";

    #endregion

    #region Langfuse-Specific Constants

    // Trace-level attributes
    private const string LangfuseTraceName = "langfuse.trace.name";
    private const string LangfuseTraceId = "langfuse.trace.id";
    private const string LangfuseTraceTags = "langfuse.trace.tags";
    private const string LangfuseTraceInput = "langfuse.trace.input";
    private const string LangfuseTraceOutput = "langfuse.trace.output";
    private const string LangfuseTracePublic = "langfuse.trace.public";
    private const string LangfuseTraceMetadataPrefix = "langfuse.trace.metadata.";
    private const string LangfuseUserId = "langfuse.user.id";
    private const string LangfuseSessionId = "langfuse.session.id";
    private const string LangfuseRelease = "langfuse.release";
    private const string LangfuseVersion = "langfuse.version";
    private const string LangfuseEnvironment = "langfuse.environment";

    // Observation-level attributes
    private const string LangfuseObservationType = "langfuse.observation.type";
    private const string LangfuseObservationLevel = "langfuse.observation.level";
    private const string LangfuseObservationStatusMessage = "langfuse.observation.status_message";
    private const string LangfuseObservationInput = "langfuse.observation.input";
    private const string LangfuseObservationOutput = "langfuse.observation.output";
    private const string LangfuseObservationMetadataPrefix = "langfuse.observation.metadata.";
    private const string LangfuseObservationModelName = "langfuse.observation.model.name";
    private const string LangfuseObservationModelParameters = "langfuse.observation.model.parameters";
    private const string LangfuseObservationUsageDetails = "langfuse.observation.usage_details";
    private const string LangfuseObservationCostDetails = "langfuse.observation.cost_details";
    private const string LangfuseObservationPromptName = "langfuse.observation.prompt.name";
    private const string LangfuseObservationPromptVersion = "langfuse.observation.prompt.version";
    private const string LangfuseObservationCompletionStartTime = "langfuse.observation.completion_start_time";

    #endregion
}