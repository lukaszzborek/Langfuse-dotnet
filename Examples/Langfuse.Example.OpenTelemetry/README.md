# Langfuse OpenTelemetry Example

This example demonstrates how to use the Langfuse OTLP exporter with OpenTelemetry in a .NET application.

## Prerequisites

- .NET 9.0 SDK or later
- Langfuse account with API credentials (Public Key and Secret Key)

## Setup

1. **Configure your Langfuse credentials**

   Edit `appsettings.json` and replace the placeholder values with your actual Langfuse credentials:

   ```json
   {
     "Langfuse": {
       "Endpoint": "https://cloud.langfuse.com",
       "PublicKey": "your-public-key-here",
       "SecretKey": "your-secret-key-here",
       "TimeoutMilliseconds": 10000
     }
   }
   ```

   You can obtain your API keys from the Langfuse dashboard:
    - Go to https://cloud.langfuse.com
    - Navigate to Settings > API Keys
    - Create or copy your Public Key and Secret Key

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

## Running the Examples

### Basic OpenTelemetry Examples

Run the standard examples showing basic OpenTelemetry span creation:

```bash
dotnet run
```

The application will:

1. Configure OpenTelemetry with the Langfuse exporter
2. Create several example traces and spans demonstrating different features
3. Export all telemetry data to your Langfuse project
4. Wait for you to press a key before exiting

### Gen AI Semantic Conventions Examples

Run examples specifically demonstrating Gen AI operations
following [OpenTelemetry Gen AI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/registry/attributes/gen-ai/):

```bash
dotnet run genai
```

These examples show how to properly instrument AI/LLM operations including chat completions, embeddings, function
calling, and multi-turn conversations.

## What the Example Demonstrates

### Basic Examples

#### Example 1: Simple Span

Creates a basic span with custom attributes.

#### Example 2: Nested Spans

Demonstrates parent-child span relationships.

#### Example 3: Langfuse-Specific Attributes

Shows how to add custom attributes that are useful for AI/LLM applications:

- Model information
- User and session tracking
- Token usage and costs

#### Example 4: Span Events

Demonstrates adding timeline events to spans for tracking workflow progress.

#### Example 5: Error Handling

Shows how to record errors and exceptions in spans.

### Gen AI Examples

The Gen AI examples (`GenAiExamples.cs`) demonstrate proper instrumentation for:

1. **Chat Completion** - OpenAI GPT-4 chat with proper semantic attributes
2. **Multiple Choices** - Requesting multiple completions with finish reasons
3. **Text Completion** - Legacy completion API style
4. **Embeddings Generation** - Vector embeddings with dimension tracking
5. **Function/Tool Calling** - Chat with nested tool execution spans
6. **Multi-Turn Conversations** - Tracking conversation context across turns
7. **Claude Integration** - Anthropic Claude with provider-specific attributes
8. **Error Handling** - Proper error recording with semantic information

## Configuration Options

### Using Configuration File (appsettings.json)

```csharp
services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddLangfuseExporter(configuration.GetSection("Langfuse"));
    });
```

### Using Programmatic Configuration

```csharp
services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddLangfuseExporter(options =>
        {
            options.Endpoint = "https://cloud.langfuse.com";
            options.PublicKey = "your-public-key";
            options.SecretKey = "your-secret-key";
            options.TimeoutMilliseconds = 10000;
            options.Headers.Add("X-Custom-Header", "value");
        });
    });
```

## Viewing Results

After running the example:

1. Go to your Langfuse dashboard at https://cloud.langfuse.com
2. Navigate to the "Traces" section
3. You should see the traces created by this example application
4. Click on any trace to see the detailed span hierarchy and attributes

## Key Concepts

### ActivitySource

The `ActivitySource` is the OpenTelemetry concept for creating spans. It's similar to the TraceSource in .NET
diagnostics.

### Spans

Spans represent units of work in your application. They can be nested to form traces.

### Attributes (Tags)

Use `SetTag()` to add key-value pairs to spans. These become searchable attributes in Langfuse.

### Events

Use `AddEvent()` to add timestamped events to spans for tracking progress or important moments.

### Status

Use `SetStatus()` to indicate success or failure of operations.

## Gen AI Helper Methods

The `GenAiActivityHelper` class provides convenient methods for creating OpenTelemetry activities that follow Gen AI
semantic conventions.

### Creating Chat Completion Activities

```csharp
var config = new GenAiChatCompletionConfig
{
    Provider = "openai",
    Model = "gpt-4",
    Temperature = 0.7,
    MaxTokens = 1000,
    TopP = 1.0,
    ConversationId = "conv-123"
};

using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
    activitySource,
    "chat-completion",
    config);

// ... perform your LLM API call ...

// Record the response
var response = new GenAiResponse
{
    ResponseId = "chatcmpl-123456",
    Model = "gpt-4-0613",
    InputTokens = 25,
    OutputTokens = 150,
    FinishReasons = new[] { "stop" }
};

GenAiActivityHelper.RecordResponse(activity, response);
```

### Creating Tool/Function Call Activities

```csharp
using var toolActivity = GenAiActivityHelper.CreateToolCallActivity(
    activitySource,
    "execute-weather-function",
    "get_weather",
    "function",
    "call_abc123");

toolActivity?.SetTag("gen_ai.tool.call.arguments", "{\"location\":\"San Francisco\"}");
// ... execute the tool ...
toolActivity?.SetTag("gen_ai.tool.call.result", "{\"temperature\":18}");
```

### Error Handling

```csharp
try
{
    await CallLlmApiAsync();
}
catch (Exception ex)
{
    GenAiActivityHelper.RecordError(activity, ex);
    throw;
}
```

### Supported Providers

- `openai` - OpenAI (GPT models)
- `anthropic` - Anthropic (Claude models)
- `azure.ai.openai` - Azure OpenAI
- `aws.bedrock` - AWS Bedrock
- `google.vertex.ai` - Google Vertex AI
- `cohere` - Cohere
- `mistral` - Mistral AI

### Gen AI Semantic Attributes

The helper automatically sets these OpenTelemetry attributes:

**Operation Metadata:**

- `gen_ai.operation.name` - Operation type (chat, text_completion, embeddings, execute_tool)
- `gen_ai.provider.name` - AI provider name
- `gen_ai.request.model` - Requested model
- `gen_ai.response.model` - Actual responding model

**Request Parameters:**

- `gen_ai.request.temperature`
- `gen_ai.request.top_p`
- `gen_ai.request.top_k`
- `gen_ai.request.max_tokens`
- `gen_ai.request.frequency_penalty`
- `gen_ai.request.presence_penalty`
- `gen_ai.request.choice_count`
- `gen_ai.request.seed`

**Response Data:**

- `gen_ai.response.id`
- `gen_ai.response.finish_reasons`
- `gen_ai.usage.input_tokens`
- `gen_ai.usage.output_tokens`

**Conversation:**

- `gen_ai.conversation.id` - Track conversation sessions

**Tools:**

- `gen_ai.tool.name`
- `gen_ai.tool.type`
- `gen_ai.tool.call.id`
- `gen_ai.tool.call.arguments`
- `gen_ai.tool.call.result`

## Learn More

- [Langfuse Documentation](https://langfuse.com/docs)
- [OpenTelemetry .NET Documentation](https://opentelemetry.io/docs/languages/net/)
- [Langfuse OpenTelemetry Integration](https://langfuse.com/docs/integrations/opentelemetry)
- [OpenTelemetry Gen AI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/registry/attributes/gen-ai/)
