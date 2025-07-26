# WORK IN PROGRESS

# LangfuseDotnet

<a href="https://www.nuget.org/packages/zborek.LangfuseDotnet/">
 <img src="https://img.shields.io/nuget/v/zborek.LangfuseDotnet" alt="NuGet">
</a>

A .NET client library for [Langfuse](https://langfuse.com) - an open-source observability and analytics platform for LLM
applications. This library enables you to track, monitor, and analyze your AI application performance and behavior.

## Features

- Create and manage traces, spans, events, and generations
- Automatic background ingestion of observability data
- Integration with dependency injection
- Support for both .NET 8.0 and 9.0
- Customizable configuration options

## Installation

Install the package via NuGet:

```bash
dotnet add package zborek.LangfuseDotnet
```

## Getting Started

### Configuration

Add Langfuse configuration to your appsettings.json:

```json
{
  "Langfuse": {
    "PublicKey": "YOUR_PUBLIC_KEY",
    "SecretKey": "YOUR_SECRET_KEY",
    "SendLogs": true
  }
}
```

### Registration with DI

```csharp
// In Program.cs or Startup.cs
builder.Services.AddLangfuse(builder.Configuration);
```

### Usage in Your Application

Inject LangfuseTrace into your service:

```csharp
public class MyService
{
    private readonly LangfuseTrace _langfuseTrace;

    public MyService(LangfuseTrace langfuseTrace)
    {
        _langfuseTrace = langfuseTrace;
    }
    
    public async Task ProcessAsync(string input)
    {
        _langfuseTrace.Trace.Body.Input = input;
        
        // Create spans to track operations
        using var dataSpan = _langfuseTrace.CreateSpan("GetData");
        var data = await GetDataAsync(input);
        dataSpan.SetOutput(data);
        
        // Track LLM generations
        using var generationSpan = _langfuseTrace.CreateGeneration("LLMProcessing", 
            input: $"Process: {input} with {data}", 
            output: null);
        
        var result = await CallLLMAsync(input, data);
        generationSpan.SetOutput(result);
        
        // Record events
        using var _ = _langfuseTrace.CreateEvent("ProcessingComplete", 
            input: input, 
            output: result);
        
        // Send data to Langfuse
        await _langfuseTrace.IngestAsync();
        
        return result;
    }
}
```

## Core Concepts

- Trace: A complete workflow or user session
- Span: A logical operation or step within a trace
- Generation: A specific LLM call or generation
- Event: A discrete occurrence within your application

## Complete Example

See the Examples/Langfuse.Example.WebApi directory for a complete working example showing integration with OpenAI.

## API Endpoints in Example Project

- POST /chat: Processes a chat request using direct service implementation
- POST /chatDi: Processes a chat request using dependency injection

## Troubleshooting

- Missing ingestion data: Ensure SendLogs is set to true in your configuration
- Authentication errors: Verify your Langfuse API keys
- Data not appearing: Check if IngestAsync() is called after operations

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## License

This project is licensed under the MIT License.

## Additional Resources

- [Langfuse Documentation](https://langfuse.com/docs)
- [GitHub Repository](https://github.com/lukaszzborek/Langfuse-dotnet)