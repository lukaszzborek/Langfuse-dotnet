# LangfuseDotnet — Setup & Configuration

> .NET SDK (v0.8.0) for Langfuse LLM observability. OpenTelemetry-based tracing
> with Gen AI semantic conventions plus full Langfuse REST API client.
> NuGet: zborek.LangfuseDotnet | Targets: net8.0, net9.0, net10.0 | License: MIT
> GitHub: https://github.com/lukaszzborek/Langfuse-dotnet

## Installation

```
dotnet add package zborek.LangfuseDotnet
```

## Setup — OpenTelemetry Tracing

Register the exporter and trace service:

```csharp
using zborek.Langfuse.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

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
            options.Url = "https://cloud.langfuse.com";
        });
    });
```

appsettings.json:

```json
{
  "Langfuse": {
    "PublicKey": "YOUR_PUBLIC_KEY",
    "SecretKey": "YOUR_SECRET_KEY",
    "Url": "https://cloud.langfuse.com"
  }
}
```

## Setup — API Client (optional, for REST API access)

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

## DI Extension Methods Summary

```
AddLangfuseExporter(TracerProviderBuilder, Action<LangfuseOtlpExporterOptions>) — Configure OTLP exporter programmatically
AddLangfuseExporter(TracerProviderBuilder, IConfiguration) — Configure OTLP exporter from IConfiguration
AddLangfuseDefaultAiSources(TracerProviderBuilder, params string[]) — Add default AI activity sources (SemanticKernel, Extensions.AI, Agents)
UseLangfuseActivityListener() — Register ActivityListener for context enrichment (idempotent, thread-safe)
AddLangfuseTracing(IServiceCollection) — Register scoped IOtelLangfuseTrace
AddLangfuseTracingNoOp(IServiceCollection) — Register no-op trace implementation for testing
AddLangfuse(IServiceCollection, IConfiguration) — Register ILangfuseClient with IConfiguration
AddLangfuse(IServiceCollection, Action<LangfuseConfig>) — Register ILangfuseClient with lambda
```

---

## Configuration Reference

### LangfuseOtlpExporterOptions (OpenTelemetry Exporter)

| Property                    | Type                   | Default                    | Description                             |
|-----------------------------|------------------------|----------------------------|-----------------------------------------|
| EnableOpenTelemetryExporter | bool                   | true                       | Enable/disable the exporter             |
| Url                         | string                 | https://cloud.langfuse.com | Langfuse API endpoint                   |
| PublicKey                   | string                 | —                          | Langfuse public API key                 |
| SecretKey                   | string                 | —                          | Langfuse secret API key                 |
| TimeoutMilliseconds         | int                    | 10000                      | Export timeout in ms                    |
| OnlyGenAiActivities         | bool                   | true                       | Filter to only export Gen AI activities |
| ActivityFilter              | Func<Activity, bool>?  | null                       | Custom filter function for activities   |

### LangfuseConfig (API Client)

| Property         | Type     | Default                    | Description                |
|------------------|----------|----------------------------|----------------------------|
| Url              | string   | https://cloud.langfuse.com | Langfuse API endpoint      |
| PublicKey        | string   | —                          | Langfuse public API key    |
| SecretKey        | string   | —                          | Langfuse secret API key    |
| BatchMode        | bool     | true                       | Enable batch mode          |
| BatchWaitTime    | TimeSpan | 5s                         | Batch processing interval  |
| DefaultTimeout   | TimeSpan | 30s                        | HTTP request timeout       |
| DefaultPageSize  | int      | 50                         | Default pagination size    |
| EnableRetry      | bool     | true                       | Auto-retry on failure      |
| MaxRetryAttempts | int      | 3                          | Maximum retry attempts     |
| RetryDelay       | TimeSpan | 1s                         | Base delay between retries |

---

## Testing

Register no-op implementation — all methods are safe no-ops, no telemetry exported:

```csharp
services.AddLangfuseTracingNoOp();
```

The `NullOtelLangfuseTrace` singleton is used internally. All observation creation methods return no-op instances.

---

## Error Handling

All `ILangfuseClient` methods throw `LangfuseApiException` on non-success HTTP responses.

```csharp
try
{
    var trace = await _langfuseClient.GetTraceAsync("trace-id");
}
catch (LangfuseApiException ex)
{
    // Handle API error
}
```

---

## Architecture Notes

- **Namespace**: `zborek.Langfuse` (client), `zborek.Langfuse.OpenTelemetry` (tracing)
- **Partial class pattern**: LangfuseClient is split across partial classes per API domain
- **Scoped services**: IOtelLangfuseTrace is scoped (one per request/operation)
- **Activity-based**: All tracing uses System.Diagnostics.Activity as the core primitive
- **ActivitySource name**: "Langfuse"
- **Batch ingestion**: 3.5MB size limit with automatic splitting
- **Background processing**: Channel<IIngestionEvent> for thread-safe event queuing
- **JSON serialization**: camelCase property names
