# LangfuseDotnet — Tracing

> OpenTelemetry-based tracing with Gen AI semantic conventions.
> Create traces, spans, generations, embeddings, tool calls, agents, and events.

## IOtelLangfuseTrace — Scoped Trace Service (DI)

Inject `IOtelLangfuseTrace` (scoped, one per request). This is the primary tracing interface.

### Full Example

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
        _trace.StartTrace("my-workflow", userId: "user-123", sessionId: "session-456");

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

### Properties

```
TraceActivity → Activity?   // underlying OTel Activity
HasActiveTrace → bool       // whether trace is active
```

### Trace Lifecycle Methods

```
StartTrace(name, traceId?, userId?, sessionId?, environment?, tags?, input?, isPublic?) → IOtelLangfuseTrace
SetTraceName(string)
SetInput(object)
SetOutput(object)
SetTraceId(string)
SetUserId(string)
SetSessionId(string)
SetEnvironment(string)
SetRelease(string)
SetVersion(string)
SetPublic(bool)
SetMetadata(string key, object value)
SetTags(IEnumerable<string>)
Skip()  // don't export this trace
```

### Observation Creation Methods

```
CreateSpan(name, type?, description?, input?, configure?) → OtelSpan
CreateGeneration(name, model, provider, input?, configure?) → OtelGeneration
CreateEmbedding(name, model, provider, input?, configure?) → OtelEmbedding
CreateToolCall(name, toolName, toolDescription?, provider, input?, configure?) → OtelToolCall
CreateAgent(name, agentId, description?, input?, configure?) → OtelAgent
CreateEvent(name, input?, output?) → OtelEvent
```

---

## OtelLangfuseTrace — Direct Instantiation (no DI)

Create traces directly without dependency injection:

```csharp
using var trace = new OtelLangfuseTrace("customer-support",
    userId: "user-123",
    sessionId: "session-456",
    tags: ["support", "billing"],
    input: new { source = "web-chat" });

using (var span = trace.CreateSpan("document-retrieval",
           type: "retrieval",
           description: "Retrieve relevant documents"))
{
    using var embedding = trace.CreateEmbedding("query-embedding",
        model: "text-embedding-3-small",
        provider: "openai",
        input: query);

    embedding.SetResponse(new GenAiResponse { InputTokens = 15 });
    span.SetOutput(new { documents = new[] { "doc1.pdf", "doc2.pdf" } });
}
```

### Constructors

```
OtelLangfuseTrace() — default, lazy trace initialization
OtelLangfuseTrace(ActivitySource) — with custom ActivitySource
OtelLangfuseTrace(name, userId?, sessionId?, environment?, release?, tags?, input?, isPublic?) — immediate trace creation
OtelLangfuseTrace(ActivitySource, name, userId?, sessionId?, environment?, release?, tags?, input?, isPublic?) — with ActivitySource and immediate creation
```

### Static Factory

```csharp
// Create independent trace for parallel operations (not linked to current Activity context)
var detached = OtelLangfuseTrace.CreateDetachedTrace("parallel-task",
    userId: "user-123", sessionId: "session-456");
```

---

## Observation Types

### OtelObservation (base class — all observation types inherit these)

```
SetInput(object)
SetOutput(object)
SetMetadata(string key, object value)
SetLevel(LangfuseObservationLevel)  // DEBUG, INFO, WARNING, ERROR
SetTag(string key, object? value)
SetStatusMessage(string)
Skip()          // don't export this observation
EndObservation() // explicitly end
Dispose()       // ends the observation
```

Properties:
```
Activity → Activity?   // underlying OTel Activity
HasActivity → bool     // check if activity exists
IsSkipped → bool       // check if skipped
```

### OtelGeneration — LLM calls

```csharp
using var gen = trace.CreateGeneration("chat",
    model: "gpt-4",
    provider: "openai",
    input: new { prompt },
    configure: g =>
    {
        g.SetTemperature(0.7);
        g.SetMaxTokens(500);
        g.SetInputMessages(new List<GenAiMessage>
        {
            new() { Role = "user", Content = "Hello!" }
        });
    });

gen.SetResponse(new GenAiResponse
{
    Model = "gpt-4",
    InputTokens = 50,
    OutputTokens = 100,
    FinishReasons = ["stop"],
    Completion = result
});
```

All OtelGeneration methods:

```
SetInputMessages(IEnumerable<GenAiMessage>)
SetPrompt(string)
SetResponse(GenAiResponse)
SetCompletion(string)
SetPromptReference(name, version?)
RecordCompletionStartTime(DateTimeOffset?)  // Time to First Token
SetTemperature(double)
SetMaxTokens(int)
SetTopP(double)
SetTopK(int)
SetFrequencyPenalty(double)
SetPresencePenalty(double)
SetRequestModel(string)
SetProvider(string)
SetChoiceCount(int)
SetSeed(int)
SetStopSequences(string[])
SetOutputType(string)           // "text", "json"
SetConversationId(string)
SetSystemInstructions(string)
SetToolDefinitions(List<GenAiToolDefinition>)
SetToolDefinitions(string)      // from JSON
SetServerAddress(string)
SetServerPort(int)
SetUsageDetails(Dictionary<string, long>)
SetCostDetails(Dictionary<string, decimal>)
SetCacheReadInputTokens(int)
SetCacheCreationInputTokens(int)
```

### OtelToolCall — function/tool invocations

```csharp
using var tool = trace.CreateToolCall("lookup-account",
    toolName: "get_account_info",
    toolDescription: "Retrieves customer account information",
    provider: "internal",
    input: new { customer_id = "cust-789" });

var accountInfo = await GetAccountAsync("cust-789");
tool.SetResult(accountInfo);
```

Methods:
```
SetArguments(object)
SetResult(object)
```

### OtelEmbedding — vector embeddings

```csharp
using var emb = trace.CreateEmbedding("query-embedding",
    model: "text-embedding-3-small",
    provider: "openai",
    input: query);

emb.SetResponse(new GenAiResponse { InputTokens = 15 });
```

Methods:
```
SetText(string)
SetResponse(GenAiResponse)
SetDimensions(int)
SetUsageDetails(Dictionary<string, long>)
SetCostDetails(Dictionary<string, decimal>)
```

### OtelAgent — agent workflows

```csharp
using var agent = trace.CreateAgent("research-assistant",
    agentId: "agent-001",
    description: "An agent that researches topics",
    input: query);

// Agent performs multiple steps with nested observations...
agent.SetOutput(synthesizedResult);
```

Methods:
```
SetDataSource(string)
```

### OtelEvent — discrete point-in-time events

```csharp
using var evt = trace.CreateEvent("user-feedback",
    input: new { rating = 5 },
    output: new { action = "logged" });
```

---

## Skip Pattern

Skip individual observations or entire traces when not needed (e.g., cache hits):

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

---

## Key Types

### GenAiResponse

```csharp
new GenAiResponse
{
    Model = "gpt-4",
    InputTokens = 50,
    OutputTokens = 100,
    FinishReasons = ["stop"],
    Completion = "result text"
}
```

### GenAiMessage

```csharp
new GenAiMessage { Role = "user", Content = "Hello!" }
```

### GenAiToolDefinition

```csharp
new GenAiToolDefinition { Name = "get_weather", Description = "Get weather info" }
```

### LangfuseObservationLevel

```
DEBUG, INFO, WARNING, ERROR
```
