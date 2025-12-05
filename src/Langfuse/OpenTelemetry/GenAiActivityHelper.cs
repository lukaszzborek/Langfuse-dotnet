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

        activity.SetTag(LangfuseAttributes.ObservationType, LangfuseAttributes.ObservationTypeGeneration);

        activity.SetTag(GenAiAttributes.OperationName, "chat");
        activity.SetTag(GenAiAttributes.ProviderName, config.Provider);
        activity.SetTag(GenAiAttributes.RequestModel, config.Model);

        if (config.Temperature.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestTemperature, config.Temperature.Value);
        }

        if (config.TopP.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestTopP, config.TopP.Value);
        }

        if (config.TopK.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestTopK, config.TopK.Value);
        }

        if (config.MaxTokens.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestMaxTokens, config.MaxTokens.Value);
        }

        if (config.FrequencyPenalty.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestFrequencyPenalty, config.FrequencyPenalty.Value);
        }

        if (config.PresencePenalty.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestPresencePenalty, config.PresencePenalty.Value);
        }

        if (config.ChoiceCount.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestChoiceCount, config.ChoiceCount.Value);
        }

        if (config.Seed.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestSeed, config.Seed.Value);
        }

        if (config.StopSequences != null && config.StopSequences.Length > 0)
        {
            activity.SetTag(GenAiAttributes.RequestStopSequences, JsonSerializer.Serialize(config.StopSequences, JsonOptions));
        }

        if (!string.IsNullOrEmpty(config.OutputType))
        {
            activity.SetTag(GenAiAttributes.OutputType, config.OutputType);
        }

        if (!string.IsNullOrEmpty(config.ConversationId))
        {
            activity.SetTag(GenAiAttributes.ConversationId, config.ConversationId);
        }

        if (!string.IsNullOrEmpty(config.SystemInstructions))
        {
            activity.SetTag(GenAiAttributes.SystemInstructions, config.SystemInstructions);
        }

        // Tool definitions
        if (config.Tools != null && config.Tools.Count > 0)
        {
            activity.SetTag(GenAiAttributes.ToolDefinitions, JsonSerializer.Serialize(config.Tools, JsonOptions));
        }

        // Server info
        if (!string.IsNullOrEmpty(config.ServerAddress))
        {
            activity.SetTag(GenAiAttributes.ServerAddress, config.ServerAddress);
        }

        if (config.ServerPort.HasValue)
        {
            activity.SetTag(GenAiAttributes.ServerPort, config.ServerPort.Value);
        }

        if (!string.IsNullOrEmpty(config.PromptName))
        {
            activity.SetTag(LangfuseAttributes.ObservationPromptName, config.PromptName);
        }

        if (config.PromptVersion.HasValue)
        {
            activity.SetTag(LangfuseAttributes.ObservationPromptVersion, config.PromptVersion.Value);
        }

        if (config.Level.HasValue)
        {
            activity.SetTag(LangfuseAttributes.ObservationLevel, config.Level.Value.ToString().ToUpperInvariant());
        }

        if (config.Metadata != null && config.Metadata.Count > 0)
        {
            foreach (KeyValuePair<string, object> kvp in config.Metadata)
            {
                activity.SetTag($"{LangfuseAttributes.ObservationMetadataPrefix}{kvp.Key}", kvp.Value);
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

        activity.SetTag(LangfuseAttributes.ObservationType, LangfuseAttributes.ObservationTypeGeneration);

        activity.SetTag(GenAiAttributes.OperationName, "text_completion");
        activity.SetTag(GenAiAttributes.ProviderName, config.Provider);
        activity.SetTag(GenAiAttributes.RequestModel, config.Model);

        if (config.Temperature.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestTemperature, config.Temperature.Value);
        }

        if (config.MaxTokens.HasValue)
        {
            activity.SetTag(GenAiAttributes.RequestMaxTokens, config.MaxTokens.Value);
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

        activity.SetTag(LangfuseAttributes.ObservationType, LangfuseAttributes.ObservationTypeEmbedding);

        activity.SetTag(GenAiAttributes.OperationName, "embeddings");
        activity.SetTag(GenAiAttributes.ProviderName, config.Provider);
        activity.SetTag(GenAiAttributes.RequestModel, config.Model);

        if (config.Dimensions.HasValue)
        {
            activity.SetTag(GenAiAttributes.EmbeddingsDimensionCount, config.Dimensions.Value);
        }

        if (config.EncodingFormats != null && config.EncodingFormats.Length > 0)
        {
            activity.SetTag(GenAiAttributes.RequestEncodingFormats, JsonSerializer.Serialize(config.EncodingFormats, JsonOptions));
        }

        if (!string.IsNullOrEmpty(config.ServerAddress))
        {
            activity.SetTag(GenAiAttributes.ServerAddress, config.ServerAddress);
        }

        if (config.ServerPort.HasValue)
        {
            activity.SetTag(GenAiAttributes.ServerPort, config.ServerPort.Value);
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

        activity.SetTag(LangfuseAttributes.ObservationType, LangfuseAttributes.ObservationTypeTool);

        activity.SetTag(GenAiAttributes.OperationName, "execute_tool");
        activity.SetTag(GenAiAttributes.ToolName, toolName);
        activity.SetTag(GenAiAttributes.ToolType, toolType);

        if (!string.IsNullOrEmpty(toolDescription))
        {
            activity.SetTag(GenAiAttributes.ToolDescription, toolDescription);
        }

        if (!string.IsNullOrEmpty(toolCallId))
        {
            activity.SetTag(GenAiAttributes.ToolCallId, toolCallId);
        }

        return activity;
    }

    /// <summary>
    ///     Records tool call arguments on an activity.
    /// </summary>
    public static void RecordToolCallArguments(Activity? activity, string arguments)
    {
        activity?.SetTag(GenAiAttributes.ToolCallArguments, arguments);
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
        activity.SetTag(GenAiAttributes.ToolCallArguments, json);
    }

    /// <summary>
    ///     Records tool call result on an activity.
    /// </summary>
    public static void RecordToolCallResult(Activity? activity, string result)
    {
        activity?.SetTag(GenAiAttributes.ToolCallResult, result);
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
        activity.SetTag(GenAiAttributes.ToolCallResult, json);
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

        activity.SetTag(LangfuseAttributes.ObservationType, LangfuseAttributes.ObservationTypeAgent);

        activity.SetTag(GenAiAttributes.OperationName, "create_agent");
        activity.SetTag(GenAiAttributes.AgentId, config.Id);
        activity.SetTag(GenAiAttributes.AgentName, config.Name);

        if (!string.IsNullOrEmpty(config.Description))
        {
            activity.SetTag(GenAiAttributes.AgentDescription, config.Description);
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

        activity.SetTag(LangfuseAttributes.ObservationType, LangfuseAttributes.ObservationTypeAgent);

        activity.SetTag(GenAiAttributes.OperationName, "invoke_agent");
        activity.SetTag(GenAiAttributes.AgentId, agentId);

        if (!string.IsNullOrEmpty(agentName))
        {
            activity.SetTag(GenAiAttributes.AgentName, agentName);
        }

        return activity;
    }

    /// <summary>
    ///     Sets the data source ID on an activity (for RAG/knowledge base operations).
    /// </summary>
    public static void SetDataSource(Activity? activity, string dataSourceId)
    {
        activity?.SetTag(GenAiAttributes.DataSourceId, dataSourceId);
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

        activity.SetTag(GenAiAttributes.EvaluationName, evaluation.Name);

        if (evaluation.ScoreValue.HasValue)
        {
            activity.SetTag(GenAiAttributes.EvaluationScoreValue, evaluation.ScoreValue.Value);
        }

        if (!string.IsNullOrEmpty(evaluation.ScoreLabel))
        {
            activity.SetTag(GenAiAttributes.EvaluationScoreLabel, evaluation.ScoreLabel);
        }

        if (!string.IsNullOrEmpty(evaluation.Explanation))
        {
            activity.SetTag(GenAiAttributes.EvaluationExplanation, evaluation.Explanation);
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
        activity.SetTag(LangfuseAttributes.ObservationInput, json);
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
        activity.SetTag(LangfuseAttributes.ObservationOutput, json);
    }

    /// <summary>
    ///     Sets the observation level/severity on an activity (Langfuse).
    /// </summary>
    public static void SetObservationLevel(Activity? activity, LangfuseObservationLevel level)
    {
        activity?.SetTag(LangfuseAttributes.ObservationLevel, level.ToString().ToUpperInvariant());
    }

    /// <summary>
    ///     Sets observation metadata on an activity (Langfuse).
    /// </summary>
    public static void SetObservationMetadata(Activity? activity, string key, object value)
    {
        activity?.SetTag($"{LangfuseAttributes.ObservationMetadataPrefix}{key}", value);
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

        activity.SetTag(LangfuseAttributes.ObservationPromptName, promptName);

        if (version.HasValue)
        {
            activity.SetTag(LangfuseAttributes.ObservationPromptVersion, version.Value);
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
        activity.SetTag(LangfuseAttributes.ObservationCompletionStartTime, time.ToString("O"));
    }

    /// <summary>
    ///     Sets trace-level input on an activity (Langfuse).
    ///     Also sets observation-level input so the root span has the data.
    /// </summary>
    public static void SetTraceInput(Activity? activity, object input)
    {
        if (activity == null)
        {
            return;
        }

        var json = input is string s ? s : JsonSerializer.Serialize(input, JsonOptions);
        activity.SetTag(LangfuseAttributes.TraceInput, json);
        activity.SetTag(LangfuseAttributes.ObservationInput, json);
    }

    /// <summary>
    ///     Sets trace-level output on an activity (Langfuse).
    ///     Also sets observation-level output so the root span has the data.
    /// </summary>
    public static void SetTraceOutput(Activity? activity, object output)
    {
        if (activity == null)
        {
            return;
        }

        var json = output is string s ? s : JsonSerializer.Serialize(output, JsonOptions);
        activity.SetTag(LangfuseAttributes.TraceOutput, json);
        activity.SetTag(LangfuseAttributes.ObservationOutput, json);
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
            activity.SetTag(LangfuseAttributes.TraceTags, JsonSerializer.Serialize(tagList, JsonOptions));
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
            activity.SetTag(GenAiAttributes.ResponseId, response.ResponseId);
        }

        if (!string.IsNullOrEmpty(response.Model))
        {
            activity.SetTag(GenAiAttributes.ResponseModel, response.Model);
        }

        if (response.FinishReasons != null && response.FinishReasons.Length > 0)
        {
            activity.SetTag(GenAiAttributes.ResponseFinishReasons, JsonSerializer.Serialize(response.FinishReasons, JsonOptions));
        }

        if (response.InputTokens.HasValue)
        {
            activity.SetTag(GenAiAttributes.UsageInputTokens, response.InputTokens.Value);
        }

        if (response.OutputTokens.HasValue)
        {
            activity.SetTag(GenAiAttributes.UsageOutputTokens, response.OutputTokens.Value);
        }

        if (response.UsageDetails != null && response.UsageDetails.Count > 0)
        {
            activity.SetTag(LangfuseAttributes.ObservationUsageDetails,
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

            activity.SetTag(LangfuseAttributes.ObservationCostDetails, JsonSerializer.Serialize(costDetails, JsonOptions));
        }

        if (response.CompletionStartTime.HasValue)
        {
            activity.SetTag(LangfuseAttributes.ObservationCompletionStartTime, response.CompletionStartTime.Value.ToString("O"));
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
        activity.SetTag(GenAiAttributes.ErrorType, exception.GetType().FullName);
        activity.SetTag(GenAiAttributes.ErrorMessage, exception.Message);

        if (!string.IsNullOrEmpty(exception.StackTrace))
        {
            activity.SetTag(GenAiAttributes.ErrorStack, exception.StackTrace);
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
        activity.SetTag(GenAiAttributes.Prompt, json);
        activity.SetTag(LangfuseAttributes.ObservationInput, json);
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
        activity.SetTag(GenAiAttributes.Completion, json);
        activity.SetTag(LangfuseAttributes.ObservationOutput, json);
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

        activity.SetTag(LangfuseAttributes.TraceName, config.Name ?? traceName);

        if (!string.IsNullOrEmpty(config.TraceId))
        {
            activity.SetTag(LangfuseAttributes.TraceId, config.TraceId);
        }

        if (!string.IsNullOrEmpty(config.UserId))
        {
            activity.SetTag(LangfuseAttributes.UserId, config.UserId);
        }

        if (!string.IsNullOrEmpty(config.SessionId))
        {
            activity.SetTag(LangfuseAttributes.SessionId, config.SessionId);
        }

        if (!string.IsNullOrEmpty(config.Environment))
        {
            activity.SetTag(LangfuseAttributes.Environment, config.Environment);
        }

        if (!string.IsNullOrEmpty(config.Release))
        {
            activity.SetTag(LangfuseAttributes.Release, config.Release);
        }

        if (!string.IsNullOrEmpty(config.Version))
        {
            activity.SetTag(LangfuseAttributes.Version, config.Version);
        }

        if (!string.IsNullOrEmpty(config.ServiceName))
        {
            activity.SetTag(GenAiAttributes.ServiceName, config.ServiceName);
        }

        if (!string.IsNullOrEmpty(config.ServiceVersion))
        {
            activity.SetTag(GenAiAttributes.ServiceVersion, config.ServiceVersion);
        }

        if (config.Public.HasValue)
        {
            activity.SetTag(LangfuseAttributes.TracePublic, config.Public.Value);
        }

        if (config.Tags != null && config.Tags.Count > 0)
        {
            activity.SetTag(LangfuseAttributes.TraceTags, JsonSerializer.Serialize(config.Tags, JsonOptions));
        }

        if (config.Input != null)
        {
            var inputJson = config.Input is string s ? s : JsonSerializer.Serialize(config.Input, JsonOptions);
            activity.SetTag(LangfuseAttributes.TraceInput, inputJson);
        }

        if (config.Output != null)
        {
            var outputJson = config.Output is string s ? s : JsonSerializer.Serialize(config.Output, JsonOptions);
            activity.SetTag(LangfuseAttributes.TraceOutput, outputJson);
        }

        if (config.Metadata != null && config.Metadata.Count > 0)
        {
            foreach (KeyValuePair<string, object> kvp in config.Metadata)
            {
                activity.SetTag($"{LangfuseAttributes.TraceMetadataPrefix}{kvp.Key}", kvp.Value);
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
            traceActivity.SetTag(LangfuseAttributes.UserId, config.UserId);
        }

        if (!string.IsNullOrEmpty(config.SessionId))
        {
            traceActivity.SetTag(LangfuseAttributes.SessionId, config.SessionId);
        }

        if (!string.IsNullOrEmpty(config.Environment))
        {
            traceActivity.SetTag(LangfuseAttributes.Environment, config.Environment);
        }

        if (config.Tags != null && config.Tags.Count > 0)
        {
            traceActivity.SetTag(LangfuseAttributes.TraceTags, JsonSerializer.Serialize(config.Tags, JsonOptions));
        }

        if (config.Metadata != null && config.Metadata.Count > 0)
        {
            foreach (KeyValuePair<string, object> kvp in config.Metadata)
            {
                traceActivity.SetTag($"{LangfuseAttributes.TraceMetadataPrefix}{kvp.Key}", kvp.Value);
            }
        }
    }
}