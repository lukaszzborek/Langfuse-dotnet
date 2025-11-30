using System.Diagnostics;

namespace Langfuse.Example.OpenTelemetry;

/// <summary>
///     Helper class for creating OpenTelemetry activities with Gen AI semantic conventions.
///     Based on: https://opentelemetry.io/docs/specs/semconv/registry/attributes/gen-ai/
/// </summary>
public static class GenAiActivityHelper
{
    // Gen AI semantic convention attribute names
    private const string GenAiOperationName = "gen_ai.operation.name";
    private const string GenAiProviderName = "gen_ai.provider.name";
    private const string GenAiRequestModel = "gen_ai.request.model";
    private const string GenAiResponseModel = "gen_ai.response.model";
    private const string GenAiResponseId = "gen_ai.response.id";
    private const string GenAiRequestTemperature = "gen_ai.request.temperature";
    private const string GenAiRequestTopP = "gen_ai.request.top_p";
    private const string GenAiRequestTopK = "gen_ai.request.top_k";
    private const string GenAiRequestMaxTokens = "gen_ai.request.max_tokens";
    private const string GenAiRequestFrequencyPenalty = "gen_ai.request.frequency_penalty";
    private const string GenAiRequestPresencePenalty = "gen_ai.request.presence_penalty";
    private const string GenAiRequestChoiceCount = "gen_ai.request.choice_count";
    private const string GenAiRequestSeed = "gen_ai.request.seed";
    private const string GenAiUsageInputTokens = "gen_ai.usage.input_tokens";
    private const string GenAiUsageOutputTokens = "gen_ai.usage.output_tokens";
    private const string GenAiResponseFinishReasons = "gen_ai.response.finish_reasons";
    private const string GenAiConversationId = "gen_ai.conversation.id";
    private const string GenAiToolName = "gen_ai.tool.name";
    private const string GenAiToolType = "gen_ai.tool.type";
    private const string GenAiToolCallId = "gen_ai.tool.call.id";

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

        if (!string.IsNullOrEmpty(config.ConversationId))
        {
            activity.SetTag(GenAiConversationId, config.ConversationId);
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
        var activity = activitySource.StartActivity(operationName, ActivityKind.Client);

        if (activity == null)
        {
            return null;
        }

        activity.SetTag(GenAiOperationName, "embeddings");
        activity.SetTag(GenAiProviderName, provider);
        activity.SetTag(GenAiRequestModel, model);

        return activity;
    }

    /// <summary>
    ///     Creates and configures an activity for a Gen AI tool/function call operation.
    /// </summary>
    public static Activity? CreateToolCallActivity(
        ActivitySource activitySource,
        string operationName,
        string toolName,
        string toolType = "function",
        string? toolCallId = null)
    {
        var activity = activitySource.StartActivity(operationName);

        if (activity == null)
        {
            return null;
        }

        activity.SetTag(GenAiOperationName, "execute_tool");
        activity.SetTag(GenAiToolName, toolName);
        activity.SetTag(GenAiToolType, toolType);

        if (!string.IsNullOrEmpty(toolCallId))
        {
            activity.SetTag(GenAiToolCallId, toolCallId);
        }

        return activity;
    }

    /// <summary>
    ///     Records response information on an existing Gen AI activity.
    /// </summary>
    public static void RecordResponse(
        Activity activity,
        GenAiResponse response)
    {
        if (!string.IsNullOrEmpty(response.ResponseId))
        {
            activity.SetTag(GenAiResponseId, response.ResponseId);
        }

        if (!string.IsNullOrEmpty(response.Model))
        {
            activity.SetTag(GenAiResponseModel, response.Model);
        }

        if (response.InputTokens.HasValue)
        {
            activity.SetTag(GenAiUsageInputTokens, response.InputTokens.Value);
        }

        if (response.OutputTokens.HasValue)
        {
            activity.SetTag(GenAiUsageOutputTokens, response.OutputTokens.Value);
        }

        if (response.FinishReasons != null && response.FinishReasons.Length > 0)
        {
            activity.SetTag(GenAiResponseFinishReasons, string.Join(",", response.FinishReasons));
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
    ///     Creates and configures a root trace activity for distributed tracing.
    /// </summary>
    public static Activity? CreateTraceActivity(
        ActivitySource activitySource,
        string traceName,
        TraceConfig config)
    {
        var activity = activitySource.StartActivity(traceName, ActivityKind.Internal);

        if (activity == null)
        {
            return null;
        }

        // Set trace-level attributes that will propagate to all spans
        if (!string.IsNullOrEmpty(config.UserId))
        {
            activity.SetTag("user.id", config.UserId);
        }

        if (!string.IsNullOrEmpty(config.SessionId))
        {
            activity.SetTag("session.id", config.SessionId);
        }

        if (!string.IsNullOrEmpty(config.TraceId))
        {
            activity.SetTag("langfuse.trace.id", config.TraceId);
        }

        if (!string.IsNullOrEmpty(config.Environment))
        {
            activity.SetTag("deployment.environment", config.Environment);
        }

        if (config.Metadata != null && config.Metadata.Count > 0)
        {
            foreach (var kvp in config.Metadata)
            {
                activity.SetTag($"langfuse.trace.metadata.{kvp.Key}", kvp.Value);
            }
        }

        if (!string.IsNullOrEmpty(config.ServiceName))
        {
            activity.SetTag("service.name", config.ServiceName);
        }

        if (!string.IsNullOrEmpty(config.ServiceVersion))
        {
            activity.SetTag("service.version", config.ServiceVersion);
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
            ? activitySource.StartActivity(spanName, ActivityKind.Internal)
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
            foreach (var kvp in config.Attributes)
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
            foreach (var kvp in config.Metadata)
            {
                traceActivity.SetTag($"langfuse.trace.metadata.{kvp.Key}", kvp.Value);
            }
        }
    }
}

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
    public string? ConversationId { get; init; }
}

/// <summary>
///     Configuration for Gen AI text completion operations.
/// </summary>
public class GenAiTextCompletionConfig
{
    public required string Provider { get; init; }
    public required string Model { get; init; }
    public double? Temperature { get; init; }
    public int? MaxTokens { get; init; }
}

/// <summary>
///     Response data for Gen AI operations.
/// </summary>
public class GenAiResponse
{
    public string? ResponseId { get; init; }
    public string? Model { get; init; }
    public int? InputTokens { get; init; }
    public int? OutputTokens { get; init; }
    public string[]? FinishReasons { get; init; }
}

/// <summary>
///     Configuration for creating root trace activities with cross-span attributes.
/// </summary>
public class TraceConfig
{
    public string? UserId { get; init; }
    public string? SessionId { get; init; }
    public string? TraceId { get; init; }
    public string? Environment { get; init; }
    public string? ServiceName { get; init; }
    public string? ServiceVersion { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
///     Configuration for creating span activities within a trace.
/// </summary>
public class SpanConfig
{
    public string? SpanType { get; init; }
    public string? Description { get; init; }
    public Dictionary<string, object>? Attributes { get; init; }
}

/// <summary>
///     Configuration for setting trace-level attributes that propagate across spans.
/// </summary>
public class TraceAttributesConfig
{
    public string? UserId { get; init; }
    public string? SessionId { get; init; }
    public string? Environment { get; init; }
    public List<string>? Tags { get; init; }
    public Dictionary<string, object>? Metadata { get; init; }
}