# Gen AI OpenTelemetry Helper - Quick Reference

A helper library for instrumenting Gen AI operations with OpenTelemetry following
the [Gen AI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/registry/attributes/gen-ai/).

## Quick Start

```csharp
using Langfuse.Example.OpenTelemetry;

var activitySource = new ActivitySource("MyApp");

// Chat completion
var config = new GenAiChatCompletionConfig
{
    Provider = "openai",
    Model = "gpt-4",
    Temperature = 0.7,
    MaxTokens = 1000
};

using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
    activitySource, "chat", config);

// ... make your LLM API call ...

GenAiActivityHelper.RecordResponse(activity, new GenAiResponse
{
    ResponseId = "chatcmpl-123",
    Model = "gpt-4-0613",
    InputTokens = 25,
    OutputTokens = 150,
    FinishReasons = new[] { "stop" }
});
```

## API Reference

### Chat Completion

```csharp
GenAiActivityHelper.CreateChatCompletionActivity(
    ActivitySource activitySource,
    string operationName,
    GenAiChatCompletionConfig config)
```

**Config Properties:**

- `Provider` (required): AI provider name (openai, anthropic, etc.)
- `Model` (required): Model identifier
- `Temperature`: Sampling temperature (0.0-2.0)
- `MaxTokens`: Maximum tokens to generate
- `TopP`: Nucleus sampling parameter
- `TopK`: Top-k sampling parameter
- `FrequencyPenalty`: Frequency penalty (-2.0 to 2.0)
- `PresencePenalty`: Presence penalty (-2.0 to 2.0)
- `ChoiceCount`: Number of completions to generate
- `Seed`: Seed for deterministic generation
- `ConversationId`: Unique conversation identifier

### Text Completion

```csharp
GenAiActivityHelper.CreateTextCompletionActivity(
    ActivitySource activitySource,
    string operationName,
    GenAiTextCompletionConfig config)
```

**Config Properties:**

- `Provider` (required): AI provider name
- `Model` (required): Model identifier
- `Temperature`: Sampling temperature
- `MaxTokens`: Maximum tokens to generate

### Embeddings

```csharp
GenAiActivityHelper.CreateEmbeddingsActivity(
    ActivitySource activitySource,
    string operationName,
    string provider,
    string model)
```

After creation, you can add embedding dimensions:

```csharp
activity?.SetTag("gen_ai.embeddings.dimension.count", 1536);
```

### Tool/Function Calls

```csharp
GenAiActivityHelper.CreateToolCallActivity(
    ActivitySource activitySource,
    string operationName,
    string toolName,
    string toolType = "function",
    string? toolCallId = null)
```

**Parameters:**

- `toolName`: Name of the function/tool
- `toolType`: Type of tool (function, extension, datastore)
- `toolCallId`: Unique identifier for this specific call

Add arguments and results:

```csharp
activity?.SetTag("gen_ai.tool.call.arguments", jsonArguments);
activity?.SetTag("gen_ai.tool.call.result", jsonResult);
```

### Recording Responses

```csharp
GenAiActivityHelper.RecordResponse(Activity activity, GenAiResponse response)
```

**Response Properties:**

- `ResponseId`: Unique response identifier
- `Model`: Actual model that responded
- `InputTokens`: Input token count
- `OutputTokens`: Output token count
- `FinishReasons`: Array of finish reasons (stop, length, tool_calls, etc.)

### Error Handling

```csharp
GenAiActivityHelper.RecordError(Activity activity, Exception exception)
```

This automatically:

- Sets activity status to Error
- Records exception type and message
- Adds exception stack trace
- Creates an exception event

## Common Patterns

### Basic Chat

```csharp
var config = new GenAiChatCompletionConfig
{
    Provider = "openai",
    Model = "gpt-4",
    Temperature = 0.7
};

using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
    activitySource, "chat", config);

try
{
    var result = await openAiClient.ChatCompletionsAsync(...);

    GenAiActivityHelper.RecordResponse(activity, new GenAiResponse
    {
        ResponseId = result.Id,
        Model = result.Model,
        InputTokens = result.Usage.PromptTokens,
        OutputTokens = result.Usage.CompletionTokens,
        FinishReasons = result.Choices.Select(c => c.FinishReason).ToArray()
    });
}
catch (Exception ex)
{
    GenAiActivityHelper.RecordError(activity, ex);
    throw;
}
```

### Multi-Turn Conversation

```csharp
var conversationId = $"conv-{Guid.NewGuid():N}";

for (int turn = 1; turn <= 3; turn++)
{
    var config = new GenAiChatCompletionConfig
    {
        Provider = "openai",
        Model = "gpt-4",
        Temperature = 0.7,
        ConversationId = conversationId
    };

    using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
        activitySource, $"chat-turn-{turn}", config);

    activity?.SetTag("conversation.turn", turn);

    // ... make API call and record response ...
}
```

### Function Calling

```csharp
// Main chat request
using var chatActivity = GenAiActivityHelper.CreateChatCompletionActivity(
    activitySource, "chat", config);

var chatResult = await CallOpenAiAsync();

GenAiActivityHelper.RecordResponse(chatActivity, new GenAiResponse
{
    ResponseId = chatResult.Id,
    FinishReasons = new[] { "tool_calls" }
});

// Execute tool call as child span
using (var toolActivity = GenAiActivityHelper.CreateToolCallActivity(
    activitySource, "execute-function", "get_weather", "function", "call_123"))
{
    toolActivity?.SetTag("gen_ai.tool.call.arguments", "{\"location\":\"SF\"}");
    var result = await ExecuteToolAsync();
    toolActivity?.SetTag("gen_ai.tool.call.result", result);
}

// Send result back to model
var finalResult = await CallOpenAiAsync();
GenAiActivityHelper.RecordResponse(chatActivity, ...);
```

### Provider-Specific Examples

**OpenAI:**

```csharp
Provider = "openai"
Model = "gpt-4" | "gpt-3.5-turbo" | "text-embedding-ada-002"
```

**Anthropic (Claude):**

```csharp
Provider = "anthropic"
Model = "claude-3-5-sonnet-20241022" | "claude-3-opus-20240229"
FinishReasons = new[] { "end_turn" | "max_tokens" | "stop_sequence" }
```

**Azure OpenAI:**

```csharp
Provider = "azure.ai.openai"
Model = "your-deployment-name"
```

**AWS Bedrock:**

```csharp
Provider = "aws.bedrock"
Model = "anthropic.claude-v2" | "amazon.titan-text-express-v1"
```

## Semantic Attributes Set by Helper

The helper automatically sets these OpenTelemetry attributes according to the semantic conventions:

| Attribute                          | Type   | Description                                                      |
|------------------------------------|--------|------------------------------------------------------------------|
| `gen_ai.operation.name`            | string | Operation type (chat, text_completion, embeddings, execute_tool) |
| `gen_ai.provider.name`             | string | Provider name (openai, anthropic, etc.)                          |
| `gen_ai.request.model`             | string | Requested model                                                  |
| `gen_ai.response.model`            | string | Actual responding model                                          |
| `gen_ai.response.id`               | string | Unique response identifier                                       |
| `gen_ai.request.temperature`       | double | Temperature parameter                                            |
| `gen_ai.request.top_p`             | double | Top-p parameter                                                  |
| `gen_ai.request.top_k`             | double | Top-k parameter                                                  |
| `gen_ai.request.max_tokens`        | int    | Max tokens parameter                                             |
| `gen_ai.request.frequency_penalty` | double | Frequency penalty                                                |
| `gen_ai.request.presence_penalty`  | double | Presence penalty                                                 |
| `gen_ai.request.choice_count`      | int    | Number of choices                                                |
| `gen_ai.request.seed`              | int    | Seed for reproducibility                                         |
| `gen_ai.usage.input_tokens`        | int    | Input token count                                                |
| `gen_ai.usage.output_tokens`       | int    | Output token count                                               |
| `gen_ai.response.finish_reasons`   | string | Comma-separated finish reasons                                   |
| `gen_ai.conversation.id`           | string | Conversation identifier                                          |
| `gen_ai.tool.name`                 | string | Tool/function name                                               |
| `gen_ai.tool.type`                 | string | Tool type                                                        |
| `gen_ai.tool.call.id`              | string | Tool invocation ID                                               |

## Benefits

✅ **Standards Compliant** - Follows OpenTelemetry Gen AI semantic conventions
✅ **Provider Agnostic** - Works with any LLM provider
✅ **Type Safe** - Strongly typed configuration objects
✅ **Easy Error Handling** - Automatic error recording with context
✅ **Conversation Tracking** - Built-in support for multi-turn conversations
✅ **Tool Integration** - First-class support for function/tool calling
✅ **Comprehensive** - Supports all major LLM operation types

## Learn More

- [OpenTelemetry Gen AI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/registry/attributes/gen-ai/)
- [Langfuse Documentation](https://langfuse.com/docs)
- [Example Code](./GenAiExamples.cs)
