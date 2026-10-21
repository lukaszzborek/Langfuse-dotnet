# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Commands

### Building and Testing

- `dotnet build` - Build the entire solution
- `dotnet test` - Run all tests in the solution
- `dotnet build src/Langfuse/Langfuse.csproj` - Build the main library project
- `dotnet test tests/Langfuse.Tests/Langfuse.Tests.csproj` - Run unit tests specifically

### Running Examples

- `dotnet run --project Examples/Langfuse.Example.Console` - Run console example
- `dotnet run --project Examples/Langfuse.Example.OpenTelemetry` - Run OpenTelemetry tracing example (available at
  localhost)

### Package Management

- `dotnet pack src/Langfuse/Langfuse.csproj` - Create NuGet package
- `dotnet restore` - Restore NuGet dependencies

## Project Architecture

The library targets the Langfuse v4 API. Traces are sent only through OpenTelemetry; the legacy v3 ingestion and read
endpoints (traces, sessions, v1 observations/metrics, v2 scores, dataset runs) are not supported.

### Core Components

**LangfuseClient** (`src/Langfuse/Client/`)

- Main HTTP client for Langfuse API communication
- Implements partial classes pattern for different API sections (AnnotationQueue, Comment, Dataset, etc.)

**OtelLangfuseTrace** (`src/Langfuse/OpenTelemetry/Trace/`)

- Tracing built on `System.Diagnostics.Activity` with Gen AI semantic conventions
- `IOtelLangfuseTrace` is the scoped DI service; `OtelLangfuseTrace` can also be created directly
- Observation types: `OtelSpan`, `OtelGeneration`, `OtelEmbedding`, `OtelToolCall`, `OtelAgent`, `OtelEvent`
- Exported to Langfuse by the OTLP exporter (`LangfuseOtlpExtensions.AddLangfuseExporter`)

**Configuration**

- `LangfuseConfig` (`src/Langfuse/Config/LangfuseConfig.cs`) - API client: URL, keys, request timeout
- `LangfuseOtlpExporterOptions` (`src/Langfuse/OpenTelemetry/`) - OTLP exporter
- Configurable via appsettings.json "Langfuse" section
- Default endpoint: https://cloud.langfuse.com

### Service Architecture

The client uses a **partial class pattern** where each API domain has its own file:

- `LangfuseClient.AnnotationQueue.cs` - Annotation queue management
- `LangfuseClient.Dataset.cs` - Dataset operations
- `LangfuseClient.ObservationV2.cs` - Observation querying (v2)
- `LangfuseClient.Score.cs` / `LangfuseClient.ScoreV3.cs` - Score creation, deletion and querying
- Plus additional domains (Comment, Experiment, Health, Media, MetricsV2, Model, Organization, Project, Prompt, Scim,
  Skills, ...)

### Dependency Injection Setup

**Extensions.cs** - API client registration:

```csharp
services.AddLangfuse(configuration);
```

- Configures the `ILangfuseClient` HTTP client with the authentication handler
- Registers `TimeProvider.System`

**LangfuseOtlpExtensions.cs** - tracing registration:

```csharp
services.AddOpenTelemetry().WithTracing(t => t.AddLangfuseExporter(configuration.GetSection("Langfuse")));
services.AddLangfuseTracing(); // scoped IOtelLangfuseTrace
```

## Key Patterns

### Authentication

- Uses `AuthorizationDelegatingHandler` for HTTP clients
- Supports both PublicKey/SecretKey authentication
- Configured through LangfuseConfig

### Error Handling

- Custom `LangfuseApiException` for API errors
- Graceful degradation when Langfuse client is unavailable

## Testing Framework

- Uses **xUnit** for unit testing
- **NSubstitute** for mocking
- **Microsoft.Extensions.TimeProvider.Testing** for time-based testing
- Unit tests in `tests/Langfuse.Tests/`, integration tests (live Langfuse server) in `tests/Langfuse.Tests.Integration/`

## OpenAPI Integration

- OpenAPI specification in `langfuse-openapi.yml`
- `split_openapi.py` splits it per tag into `openapi-split/`

## Development Workflow

1. Core library development in `src/Langfuse/`
2. Example applications in `Examples/` demonstrate real-world usage patterns
3. Integration with OpenAI shown in the OpenTelemetry example
4. JSON serialization uses camelCase with kebab-case-upper enums
5. All models follow nullable reference types pattern
