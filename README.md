# LangfuseDotnet

<a href="https://www.nuget.org/packages/zborek.LangfuseDotnet/">
 <img src="https://img.shields.io/nuget/v/zborek.LangfuseDotnet" alt="NuGet">
</a>

A .NET client library for [Langfuse](https://langfuse.com) - an open-source observability and analytics platform for LLM applications. This library uses **OpenTelemetry** to track, monitor, and analyze your AI application performance and behavior with [Gen AI semantic conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/).

## Features

- OpenTelemetry-based tracing with Gen AI semantic conventions
- Create and manage traces, spans, generations, embeddings, tool calls, agents, and events
- Automatic context propagation and parent-child relationships
- Filtering to export only Gen AI activities (excludes infrastructure noise)
- Full Langfuse API client for datasets, prompts, scores, and more
- Integration with dependency injection
- Support for both .NET 8.0 and 9.0

## Installation

Install the package via NuGet:

```bash
dotnet add package zborek.LangfuseDotnet
```

## Getting Started

### Configuration

Add Langfuse configuration to your `appsettings.json`:

```json
{
  "Langfuse": {
    "PublicKey": "YOUR_PUBLIC_KEY",
    "SecretKey": "YOUR_SECRET_KEY",
    "Endpoint": "https://cloud.langfuse.com"
  }
}
```

### Registration with DI

```csharp
using zborek.Langfuse.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry with Langfuse exporter
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddLangfuseExporter(builder.Configuration.GetSection("Langfuse"));
    });

// Register IOtelLangfuseTrace for dependency injection
builder.Services.AddLangfuseTracing();
```

Or configure programmatically:

```csharp
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing.AddLangfuseExporter(options =>
        {
            options.PublicKey = "pk-...";
            options.SecretKey = "sk-...";
            options.Endpoint = "https://cloud.langfuse.com";
        });
    });
```

### Usage in Your Application

#### Option 1: Dependency Injection with `IOtelLangfuseTrace`

Inject `IOtelLangfuseTrace` into your service for scoped tracing across multiple services:

```csharp
public class MyService
{
    private readonly IOtelLangfuseTrace _trace;

    public MyService(IOtelLangfuseTrace trace)
    {
        _trace = trace;
    }

    public async Task<string> ProcessAsync(string prompt)
    {
        // Start a trace (call once per request)
        _trace.StartTrace("my-workflow", userId: "user-123", sessionId: "session-456");

        // Create a generation for LLM call
        using var generation = _trace.CreateGeneration("openai-chat",
            model: "gpt-4",
            provider: "openai",
            input: new { prompt },
            configure: g =>
            {
                g.SetTemperature(0.7);
                g.SetMaxTokens(500);
            });

        var result = await CallLLMAsync(prompt);

        generation.SetResponse(new GenAiResponse
        {
            Model = "gpt-4",
            InputTokens = 50,
            OutputTokens = 100,
            FinishReasons = ["stop"],
            Completion = result
        });

        _trace.SetOutput(new { result });
        return result;
    }
}
```

#### Option 2: Direct Instantiation with `OtelLangfuseTrace`

Create traces directly without DI:

```csharp
using var trace = new OtelLangfuseTrace("customer-support",
    userId: "user-123",
    sessionId: "session-456",
    tags: ["support", "billing"],
    input: new { source = "web-chat" });

// Create a span for a logical operation
using (var span = trace.CreateSpan("document-retrieval",
           type: "retrieval",
           description: "Retrieve relevant documents"))
{
    // Nested embedding
    using var embedding = trace.CreateEmbedding("query-embedding",
        model: "text-embedding-3-small",
        provider: "openai",
        input: query);

    await Task.Delay(50);
    embedding.SetResponse(new GenAiResponse { InputTokens = 15 });

    span.SetOutput(new { documents = new[] { "doc1.pdf", "doc2.pdf" } });
}

// Create a generation
using (var generation = trace.CreateGeneration("generate-response",
           model: "gpt-4",
           provider: "openai",
           configure: g =>
           {
               g.SetTemperature(0.7);
               g.SetInputMessages(new List<GenAiMessage>
               {
                   new() { Role = "user", Content = "Hello!" }
               });
           }))
{
    var response = await CallLLMAsync();
    generation.SetResponse(new GenAiResponse
    {
        Model = "gpt-4",
        InputTokens = 10,
        OutputTokens = 25,
        FinishReasons = ["stop"],
        Completion = response
    });
}
```

## Observation Types

| Type | Method | Description |
|------|--------|-------------|
| **Span** | `CreateSpan()` | Logical operation or workflow step |
| **Generation** | `CreateGeneration()` | LLM API call with model/token tracking |
| **Embedding** | `CreateEmbedding()` | Vector embedding operation |
| **ToolCall** | `CreateToolCall()` | Function/tool invocation |
| **Agent** | `CreateAgent()` | Agent loop iteration |
| **Event** | `CreateEvent()` | Discrete point-in-time event |

### Tool Call Example

```csharp
using var toolCall = trace.CreateToolCall("lookup-account",
    toolName: "get_account_info",
    toolDescription: "Retrieves customer account information",
    input: new { customer_id = "cust-789" });

var accountInfo = await GetAccountAsync("cust-789");
toolCall.SetResult(accountInfo);
```

### Agent Example

```csharp
using var agent = trace.CreateAgent("research-assistant",
    agentId: "agent-001",
    description: "An agent that researches topics",
    input: query);

// Agent performs multiple steps...
agent.SetOutput(synthesizedResult);
```

## Skipping Traces and Observations

Skip individual observations or entire traces when they're not needed (e.g., cache hits):

```csharp
using var span = trace.CreateSpan("llm-processing");

if (cachedResult != null)
{
    // Skip this span - won't be sent to Langfuse
    span.Skip();
    return cachedResult;
}

// Or skip the entire trace
trace.Skip();
```

## Langfuse API Client

For accessing Langfuse API endpoints (datasets, prompts, scores, etc.), register the `ILangfuseClient`:

```csharp
using zborek.Langfuse;

builder.Services.AddLangfuse(builder.Configuration);
```

Or configure programmatically:

```csharp
builder.Services.AddLangfuse(config =>
{
    config.Url = "https://cloud.langfuse.com";
    config.PublicKey = "pk-...";
    config.SecretKey = "sk-...";
});
```

### Using the API Client

Inject `ILangfuseClient` to access all Langfuse API endpoints:

```csharp
public class MyService
{
    private readonly ILangfuseClient _langfuseClient;

    public MyService(ILangfuseClient langfuseClient)
    {
        _langfuseClient = langfuseClient;
    }

    public async Task RunEvaluationAsync()
    {
        // Get a prompt
        var prompt = await _langfuseClient.GetPromptAsync("my-prompt", label: "production");

        // Get dataset items
        var dataset = await _langfuseClient.GetDatasetAsync("my-dataset");

        // Create a score
        await _langfuseClient.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = "trace-123",
            Name = "accuracy",
            Value = 0.95
        });
    }
}
```

### Available API Domains

| Domain | Description |
|--------|-------------|
| **Datasets** | Create and manage test datasets and runs |
| **Prompts** | Version control and retrieve prompt templates |
| **Scores** | Create and query evaluation scores |
| **Score Configs** | Define score schemas and metadata |
| **Traces** | Query and manage traces |
| **Observations** | Query spans, generations, and events |
| **Sessions** | Group traces into user sessions |
| **Comments** | Add comments to traces and observations |
| **Models** | Query supported LLM models and pricing |
| **Media** | Upload and manage media assets |
| **Annotation Queues** | Manage annotation workflows |
| **Health** | API health checks |

### API Client Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `Url` | `https://cloud.langfuse.com` | Langfuse API endpoint |
| `PublicKey` | - | Langfuse public API key |
| `SecretKey` | - | Langfuse secret API key |
| `DefaultTimeout` | `30s` | HTTP request timeout |
| `DefaultPageSize` | `50` | Default pagination size |
| `EnableRetry` | `true` | Auto-retry on failure |
| `MaxRetryAttempts` | `3` | Maximum retry attempts |
| `RetryDelay` | `1s` | Base delay between retries |

## OpenTelemetry Exporter Options

| Option | Default | Description |
|--------|---------|-------------|
| `Enabled` | `true` | Enable/disable the exporter |
| `Endpoint` | `https://cloud.langfuse.com` | Langfuse API endpoint |
| `PublicKey` | - | Langfuse public API key |
| `SecretKey` | - | Langfuse secret API key |
| `TimeoutMilliseconds` | `10000` | Export timeout |
| `OnlyGenAiActivities` | `true` | Filter to only export Gen AI activities |
| `ActivityFilter` | `null` | Custom filter function for activities |

## Testing

Use the no-op implementation for unit tests:

```csharp
services.AddLangfuseTracingNoOp();
```

## Complete Example

See the `Examples/Langfuse.Example.OpenTelemetry` directory for a complete working example showing:
- Integration with OpenAI
- RAG pipeline with nested spans
- Agent workflows with tool calls
- Skip patterns for caching

## API Endpoints in Example Project

- POST `/otel-trace-example`: Customer support conversation with multiple generations
- POST `/otel-nested-example`: RAG pipeline with document retrieval and embeddings
- POST `/otel-agent-example`: Research agent with tool calls
- POST `/otel-skip-span-example`: Demonstrates skipping individual observations
- POST `/otel-skip-trace-example`: Demonstrates skipping entire traces

## Troubleshooting

- **No data appearing**: Ensure `Enabled` is `true` and API keys are correct
- **Authentication errors**: Verify your Langfuse API keys
- **Missing activities**: Check `OnlyGenAiActivities` setting - infrastructure activities are filtered by default

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.

## Additional Resources

- [Langfuse Documentation](https://langfuse.com/docs)
- [OpenTelemetry Gen AI Semantic Conventions](https://opentelemetry.io/docs/specs/semconv/gen-ai/)
- [GitHub Repository](https://github.com/lukaszzborek/Langfuse-dotnet)