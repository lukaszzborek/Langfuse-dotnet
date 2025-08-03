# Langfuse .NET SDK - Endpoint Implementation Plan

## Executive Summary

This document outlines a comprehensive plan to extend the Langfuse .NET SDK with missing API endpoints. The current SDK only implements basic ingestion functionality, missing **43 critical endpoints** across 8 major functional areas. This plan provides a phased approach to implement full API coverage while maintaining backward compatibility.
OpenApi specification: https://cloud.langfuse.com/generated/api/openapi.yml

## Current State Analysis

### Existing Implementation
- ✅ **Ingestion API**: POST `/api/public/ingestion` (batch event submission)
- ✅ **Core Architecture**: LangfuseTrace, ILangfuseClient, dependency injection
- ✅ **Background Processing**: Optional batch mode with LangfuseBackgroundService
- ✅ **Authentication**: Authorization header handling

### Missing Functionality
- ❌ **Read Operations**: No GET endpoints for retrieving data
- ❌ **CRUD Management**: No CREATE, UPDATE, DELETE for resources
- ❌ **Advanced Features**: No scores, prompts, datasets, or analytics
- ❌ **Filtering & Pagination**: No query capabilities

## Implementation Phases

### Phase 1: Core Read Operations (Weeks 1-2) - DONE
**Priority: HIGH** - Essential for most SDK users

#### Endpoints to Implement
- `GET /api/public/observations` - List observations with filtering
- `GET /api/public/observations/{observationId}` - Get single observation
- `GET /api/public/traces` - List traces with pagination
- `GET /api/public/traces/{traceId}` - Get single trace with nested data
- `GET /api/public/sessions` - List user sessions
- `GET /api/public/sessions/{sessionId}` - Get single session

#### Key Features
- **Pagination Support**: Cursor-based and offset pagination
- **Advanced Filtering**: By date range, user, name, tags, metadata
- **Sorting Options**: By timestamp, name, duration, cost
- **Nested Data Loading**: Include related observations in trace responses

#### Code Changes
1. **Service Interfaces**:
   ```csharp
   public interface IObservationService
   {
       Task<ObservationListResponse> ListAsync(ObservationListRequest request, CancellationToken cancellationToken = default);
       Task<Observation> GetAsync(string observationId, CancellationToken cancellationToken = default);
   }
   ```

2. **Request/Response Models**:
   - `ObservationListRequest` with filtering and pagination
   - `ObservationListResponse` with metadata and results
   - Enhanced `Observation`, `Trace`, `Session` models

3. **Client Extension**:
   ```csharp
   public interface ILangfuseClient
   {
       // Existing methods unchanged
       Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default);
       Task IngestAsync(LangfuseTrace langfuseTrace, CancellationToken cancellationToken = default);
       
       // New service properties
       IObservationService Observations { get; }
       ITraceService Traces { get; }
       ISessionService Sessions { get; }
   }
   ```

### Phase 2: Scores & Evaluation (Weeks 3-4) - DONE
**Priority: HIGH** - Critical for AI monitoring and evaluation

#### Endpoints to Implement
- `GET /api/public/scores` - List scores with filtering
- `POST /api/public/scores` - Create new score
- `GET /api/public/scores/{scoreId}` - Get single score
- `PATCH /api/public/scores/{scoreId}` - Update score
- `DELETE /api/public/scores/{scoreId}` - Delete score
- `GET /api/public/score-configs` - List score configurations
- `POST /api/public/score-configs` - Create score configuration

#### Key Features
- **Score Types**: Numeric, categorical, boolean scoring
- **Custom Configs**: Define scoring criteria and ranges
- **Batch Scoring**: Efficient bulk score operations
- **Score Analytics**: Aggregation and statistical functions

#### Use Cases
- **Model Evaluation**: Track LLM performance metrics
- **Human Feedback**: Capture user ratings and reviews
- **Automated Scoring**: Integration with evaluation pipelines
- **A/B Testing**: Compare model performance across variants

### Phase 3: Management Operations (Weeks 5-6)
**Priority: MEDIUM** - Important for production usage

#### Endpoints to Implement
- `GET /api/public/prompts` - List prompt templates
- `POST /api/public/prompts` - Create prompt template
- `GET /api/public/prompts/{promptName}` - Get prompt versions
- `GET /api/public/datasets` - List datasets
- `POST /api/public/datasets` - Create dataset
- `GET /api/public/dataset-items/{datasetName}` - Get dataset items
- `POST /api/public/dataset-items` - Add dataset items
- `GET /api/public/models` - List tracked models
- `POST /api/public/models` - Register new model

#### Key Features
- **Prompt Versioning**: Track prompt template evolution
- **Dataset Management**: Test data organization and retrieval
- **Model Tracking**: Monitor LLM usage, costs, and performance
- **Cost Analytics**: Track token usage and associated costs

### Phase 4: Advanced Features (Weeks 7-8)
**Priority: LOW** - Nice-to-have features

#### Endpoints to Implement
- `GET /api/public/comments` - List comments
- `POST /api/public/comments` - Create comment
- `GET /api/public/metrics/daily` - Daily aggregated metrics
- `GET /api/public/health` - Service health status

#### Key Features
- **Collaboration**: Team comments and annotations
- **Analytics Dashboard**: Pre-computed metrics and insights
- **Monitoring**: Health checks and system status

## Technical Architecture

### Service Pattern Implementation

```csharp
public class LangfuseClient : ILangfuseClient
{
    private readonly HttpClient _httpClient;
    private readonly IObservationService _observations;
    private readonly ITraceService _traces;
    private readonly ISessionService _sessions;
    private readonly IScoreService _scores;
    
    public IObservationService Observations => _observations;
    public ITraceService Traces => _traces;
    public ISessionService Sessions => _sessions;
    public IScoreService Scores => _scores;
    
    // Existing ingestion methods unchanged
    public async Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default)
    {
        // Existing implementation
    }
}
```

### Model Generation Strategy

1. **OpenAPI Analysis**: Parse specification from `https://cloud.langfuse.com/generated/api/openapi.yml`
2. **Code Generation**: Use NSwag or Kiota for model generation
3. **Manual Refinement**: Adjust generated models for .NET conventions
4. **Validation**: Ensure models match expected API behavior

### Error Handling Pattern

```csharp
public class LangfuseApiException : Exception
{
    public int StatusCode { get; }
    public string? ErrorCode { get; }
    public IDictionary<string, object>? Details { get; }
    
    public LangfuseApiException(int statusCode, string message, string? errorCode = null, IDictionary<string, object>? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Details = details;
    }
}
```

## Backwards Compatibility Strategy

### Non-Breaking Changes
- ✅ **Existing API Unchanged**: All current methods remain functional
- ✅ **Additive Extensions**: New functionality via service properties
- ✅ **Optional Dependencies**: New services registered automatically
- ✅ **Configuration Compatible**: Existing LangfuseConfig works unchanged

### Migration Path
1. **Phase 0**: Current users continue using ingestion-only functionality
2. **Phase 1**: Gradual adoption of read operations via new service properties
3. **Phase 2+**: Advanced features available when needed

### Versioning Strategy
- **Current**: 0.2.1 (ingestion-only)
- **Target**: 1.0.0 (full API coverage)
- **Future**: Semantic versioning for incremental improvements

## Implementation Details

### Dependency Injection Updates

```csharp
public static IServiceCollection AddLangfuse(this IServiceCollection services, IConfiguration configuration)
{
    // Existing registrations unchanged
    services.Configure<LangfuseConfig>(configuration.GetSection("Langfuse"));
    services.TryAddSingleton(TimeProvider.System);
    services.AddScoped<LangfuseTrace>();
    services.AddScoped<AuthorizationDelegatingHandler>();
    
    // New service registrations
    services.AddScoped<IObservationService, ObservationService>();
    services.AddScoped<ITraceService, TraceService>();
    services.AddScoped<ISessionService, SessionService>();
    services.AddScoped<IScoreService, ScoreService>();
    
    // Enhanced HTTP client
    services.AddHttpClient<ILangfuseClient, LangfuseClient>(x => { x.BaseAddress = new Uri(config.Url); })
        .AddHttpMessageHandler<AuthorizationDelegatingHandler>();
    
    return services;
}
```

### Configuration Extensions

```csharp
public class LangfuseConfig
{
    // Existing properties unchanged
    public string Url { get; set; } = "https://cloud.langfuse.com";
    public string PublicKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool BatchMode { get; set; } = true;
    public TimeSpan BatchWaitTime { get; set; } = TimeSpan.FromSeconds(5);
    public bool SendLogs { get; set; } = true;
    
    // New optional properties
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);
    public int DefaultPageSize { get; set; } = 50;
    public bool EnableRetry { get; set; } = true;
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}
```

## Testing Strategy

### Unit Testing
- **Service Tests**: Mock HTTP client, test business logic
- **Model Tests**: Serialization/deserialization validation
- **Configuration Tests**: Dependency injection and setup
- **Error Handling Tests**: Exception scenarios and edge cases

### Integration Testing
- **Live API Tests**: Real Langfuse API interaction (with test data)
- **End-to-End Workflows**: Complete user scenarios
- **Performance Tests**: Large dataset handling and pagination
- **Compatibility Tests**: Backward compatibility validation

### Test Coverage Goals
- **Unit Tests**: 90%+ coverage for new code
- **Integration Tests**: All major API endpoints covered
- **Performance Tests**: Pagination and filtering under load
- **Documentation Tests**: All examples in docs are runnable

## Documentation Updates

### API Documentation
- **Service Interfaces**: XML documentation for all public APIs
- **Usage Examples**: Real-world scenarios for each service
- **Error Handling**: Common exceptions and resolution strategies
- **Configuration Guide**: All options explained with examples

### Updated README Sections
- **Feature Matrix**: Comparison with other Langfuse SDKs
- **Quick Start**: Enhanced examples showing read operations
- **Advanced Usage**: Filtering, pagination, and batch operations
- **Migration Guide**: Upgrading from ingestion-only usage

### Example Applications
- **Enhanced Web API**: Demonstrate read operations and analytics
- **Console Analytics Tool**: Command-line data exploration utility
- **Evaluation Pipeline**: Automated scoring and assessment example

## Risk Assessment and Mitigation

### Identified Risks

1. **API Rate Limiting**
   - **Risk**: Langfuse API rate limits could affect SDK performance
   - **Mitigation**: Implement exponential backoff, request throttling, caching

2. **Breaking API Changes**
   - **Risk**: Langfuse API evolution could break SDK
   - **Mitigation**: Robust error handling, API versioning support, optional properties

3. **Large Dataset Performance**
   - **Risk**: Large responses could cause memory issues
   - **Mitigation**: Streaming responses, pagination limits, lazy loading

4. **Authentication Security**
   - **Risk**: API key exposure in logs or errors
   - **Mitigation**: Secure logging practices, credential masking, environment variables

### Performance Considerations
- **Connection Pooling**: Reuse HTTP connections efficiently
- **Response Caching**: Cache stable data (prompts, configs) appropriately
- **Memory Management**: Stream large responses, dispose resources properly
- **Async Patterns**: Non-blocking operations throughout

## Success Criteria

### Functional Requirements
- ✅ All Phase 1-3 endpoints implemented and tested
- ✅ Backward compatibility maintained (zero breaking changes)
- ✅ Comprehensive error handling and logging
- ✅ Complete documentation and examples

### Non-Functional Requirements
- ✅ Performance equivalent to current ingestion functionality
- ✅ Memory efficient handling of large datasets
- ✅ Intuitive API design following .NET conventions
- ✅ Comprehensive test coverage (90%+ for new code)

### Quality Requirements
- ✅ Code review process followed for all changes
- ✅ Integration tests pass against live Langfuse API
- ✅ Documentation examples are tested and working
- ✅ NuGet package successfully deploys and functions

## Delivery Timeline

### Week 1: Phase 1A - Observations Service
- Implement `IObservationService` interface and `ObservationService` class
- Create request/response models for observations
- Add comprehensive unit tests
- Update dependency injection registration

### Week 2: Phase 1B - Traces and Sessions Services  
- Implement `ITraceService` and `ISessionService`
- Add pagination and filtering capabilities
- Create integration tests against live API
- Update documentation and examples

### Week 3: Phase 2A - Scores CRUD Operations
- Implement full CRUD for scores API
- Add score configuration management
- Create comprehensive error handling
- Performance testing for batch operations

### Week 4: Phase 2B - Advanced Scoring Features
- Add score analytics and aggregation methods
- Implement batch scoring capabilities
- Create evaluation pipeline examples
- Update SDK documentation

### Week 5-6: Phase 3 - Management Operations
- Implement prompts, datasets, and models services
- Add cost tracking and analytics features
- Create advanced usage examples
- Comprehensive testing and documentation

### Week 7-8: Phase 4 - Advanced Features & Polish
- Implement comments and metrics endpoints
- Add health monitoring capabilities
- Final testing, documentation, and examples
- Package for 1.0.0 release

## Key Implementation Files

### New Files to Create

#### Service Interfaces
- `/src/Langfuse/Client/Services/IObservationService.cs`
- `/src/Langfuse/Client/Services/ITraceService.cs`
- `/src/Langfuse/Client/Services/ISessionService.cs`
- `/src/Langfuse/Client/Services/IScoreService.cs`

#### Service Implementations
- `/src/Langfuse/Client/Services/ObservationService.cs`
- `/src/Langfuse/Client/Services/TraceService.cs`
- `/src/Langfuse/Client/Services/SessionService.cs`
- `/src/Langfuse/Client/Services/ScoreService.cs`

#### Request Models
- `/src/Langfuse/Models/Requests/ObservationListRequest.cs`
- `/src/Langfuse/Models/Requests/TraceListRequest.cs`
- `/src/Langfuse/Models/Requests/SessionListRequest.cs`
- `/src/Langfuse/Models/Requests/ScoreCreateRequest.cs`

#### Response Models
- `/src/Langfuse/Models/Responses/ObservationListResponse.cs`
- `/src/Langfuse/Models/Responses/TraceListResponse.cs`
- `/src/Langfuse/Models/Responses/SessionListResponse.cs`
- `/src/Langfuse/Models/Responses/ScoreListResponse.cs`

#### Enhanced Models
- `/src/Langfuse/Models/Observation.cs`
- `/src/Langfuse/Models/Trace.cs`
- `/src/Langfuse/Models/Session.cs`
- `/src/Langfuse/Models/Score.cs`

### Files to Modify

#### Core Client Updates
- `/src/Langfuse/Client/ILangfuseClient.cs` - Add service properties
- `/src/Langfuse/Client/LangfuseClient.cs` - Implement service properties
- `/src/Langfuse/Extensions.cs` - Register new services
- `/src/Langfuse/Config/LangfuseConfig.cs` - Add optional configuration

#### Testing Files
- `/tests/Langfuse.Tests/Client/Services/ObservationServiceTests.cs`
- `/tests/Langfuse.Tests/Client/Services/TraceServiceTests.cs`
- `/tests/Langfuse.Tests/Client/Services/SessionServiceTests.cs`
- `/tests/Langfuse.Tests/Client/Services/ScoreServiceTests.cs`
- `/tests/Langfuse.Tests/Integration/LiveApiTests.cs`

## Conclusion

This implementation plan provides a structured approach to transforming the Langfuse .NET SDK from a basic ingestion-only library into a comprehensive, full-featured SDK covering the entire Langfuse API surface. The phased approach allows for incremental delivery, thorough testing, and maintained backward compatibility, ensuring existing users can continue using the library without disruption while gaining access to powerful new capabilities.

The plan prioritizes the most commonly needed functionality first (read operations and scoring) while building a solid foundation for advanced features. The resulting SDK will provide .NET developers with a complete, production-ready toolkit for building sophisticated LLM observability and evaluation systems.