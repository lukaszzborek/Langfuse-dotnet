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
- `dotnet run --project Examples/Langfuse.Example.WebApi` - Run web API example (available at localhost)

### Package Management

- `dotnet pack src/Langfuse/Langfuse.csproj` - Create NuGet package
- `dotnet restore` - Restore NuGet dependencies

## Project Architecture

### Core Components

**LangfuseClient** (`src/Langfuse/Client/`)

- Main HTTP client for Langfuse API communication
- Implements partial classes pattern for different API sections (AnnotationQueue, Comment, Dataset, etc.)

**OtelLangfuseTrace** (`src/Langfuse/OpenTelemetry/Trace/`)

- OpenTelemetry-based tracing (`IOtelLangfuseTrace` for DI, `OtelLangfuseTrace` for direct use)
- Uses System.Diagnostics.Activity as the core primitive; exported via OTLP
- Provides trace/span/generation/tool-call/embedding/agent creation methods

**Configuration** (`src/Langfuse/Config/LangfuseConfig.cs`)

- Configurable via appsettings.json "Langfuse" section
- Supports retry policies, timeouts, and page sizes
- Default endpoint: https://cloud.langfuse.com

### Service Architecture

The client uses a **partial class pattern** where each API domain has its own file:

- `LangfuseClient.AnnotationQueue.cs` - Annotation queue management
- `LangfuseClient.Dataset.cs` - Dataset and dataset run operations
- `LangfuseClient.Observation.cs` - Observation querying and management
- `LangfuseClient.Score.cs` - Scoring and score configuration
- Plus additional domains (Comment, Health, Media, Model, Organization, Project, Prompt, Scim, Session)

### Dependency Injection Setup

**Extensions.cs** - Registration pattern:

```csharp
services.AddLangfuse(configuration);
```

- Configures HTTP clients with authentication handlers
- Configures time provider for testing support

## Key Patterns

### Authentication

- Uses `AuthorizationDelegatingHandler` for HTTP clients
- Supports both PublicKey/SecretKey authentication
- Configured through LangfuseConfig

### Error Handling

- Custom `LangfuseApiException` for API errors
- Automatic retry support with configurable policies
- Graceful degradation when Langfuse client is unavailable

## Testing Framework

- Uses **xUnit** for unit testing
- **NSubstitute** for mocking
- **Microsoft.Extensions.TimeProvider.Testing** for time-based testing
- Tests located in `tests/Langfuse.Tests/`

## OpenAPI Integration

- OpenAPI specifications in `src/Langfuse/openapi.yml` and `openapi-all.yml`
- Implementation plan document: `spec/missing-endpoints-implementation-plan.md`
- API coverage tracking for ensuring complete endpoint implementation

## Development Workflow

1. Core library development in `src/Langfuse/`
2. Example applications in `Examples/` demonstrate real-world usage patterns
3. Integration with OpenAI SDK shown in WebApi example
4. JSON serialization uses camelCase with kebab-case-upper enums
5. All models follow nullable reference types pattern (.NET 9.0)