# Task 13: Integration Testing

**Task ID:** openapi-update-13
**Priority:** Medium
**Dependencies:** All previous tasks (01-12)
**Approach:** Integration Testing

## Objective

Create integration tests and/or example applications demonstrating the new features work end-to-end with actual API calls (or against a test instance).

## What Needs to Be Done

### Option A: Integration Test Suite

Create integration tests that can run against a real Langfuse instance:

**1. tests/Langfuse.IntegrationTests/** (new project)

Create a new test project for integration tests:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="xunit" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" />
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="../../src/Langfuse/Langfuse.csproj" />
  </ItemGroup>
</Project>
```

**2. Integration Test Structure:**

```csharp
public class BlobStorageIntegrationTests : IAsyncLifetime
{
    private readonly LangfuseClient _client;
    private readonly string _testProjectId;

    public BlobStorageIntegrationTests()
    {
        // Initialize client with org-scoped API key from env vars
        var config = new LangfuseConfig
        {
            PublicKey = Environment.GetEnvironmentVariable("LANGFUSE_ORG_PUBLIC_KEY"),
            SecretKey = Environment.GetEnvironmentVariable("LANGFUSE_ORG_SECRET_KEY"),
            BaseUrl = Environment.GetEnvironmentVariable("LANGFUSE_BASE_URL") ?? "https://cloud.langfuse.com"
        };

        _client = new LangfuseClient(config);
        _testProjectId = Environment.GetEnvironmentVariable("LANGFUSE_TEST_PROJECT_ID")!;
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task BlobStorage_FullLifecycle()
    {
        // Skip if no API keys configured
        Skip.IfNot(IsConfigured(), "Integration tests require API keys");

        // 1. Create blob storage integration
        var createRequest = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = _testProjectId,
            Type = BlobStorageIntegrationType.S3,
            BucketName = "test-bucket",
            Region = "us-east-1",
            AccessKeyId = "test-key",
            SecretAccessKey = "test-secret",
            ExportFrequency = BlobStorageExportFrequency.Daily,
            Enabled = false, // Don't actually export
            ForcePathStyle = false,
            FileType = BlobStorageIntegrationFileType.Json,
            ExportMode = BlobStorageExportMode.FromToday
        };

        var created = await _client.UpsertBlobStorageIntegrationAsync(createRequest);
        Assert.NotNull(created);
        Assert.NotNull(created.Id);

        // 2. List integrations
        var integrations = await _client.GetBlobStorageIntegrationsAsync();
        Assert.Contains(integrations.Data, i => i.Id == created.Id);

        // 3. Update integration
        var updateRequest = createRequest with { Enabled = true };
        var updated = await _client.UpsertBlobStorageIntegrationAsync(updateRequest);
        Assert.True(updated.Enabled);

        // 4. Delete integration
        var deleted = await _client.DeleteBlobStorageIntegrationAsync(created.Id);
        Assert.NotNull(deleted.Message);

        // 5. Verify deletion
        var afterDelete = await _client.GetBlobStorageIntegrationsAsync();
        Assert.DoesNotContain(afterDelete.Data, i => i.Id == created.Id);
    }

    private bool IsConfigured() =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LANGFUSE_ORG_PUBLIC_KEY"));

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync() => Task.CompletedTask;
}
```

### Option B: Example Application

Create console example demonstrating new features:

**Examples/Langfuse.Example.NewFeatures/Program.cs**

```csharp
using Langfuse;
using Langfuse.Models.BlobStorageIntegration;
using Langfuse.Models.LlmConnection;

// Configuration from environment
var config = new LangfuseConfig
{
    PublicKey = Environment.GetEnvironmentVariable("LANGFUSE_PUBLIC_KEY")!,
    SecretKey = Environment.GetEnvironmentVariable("LANGFUSE_SECRET_KEY")!
};

var client = new LangfuseClient(config);

Console.WriteLine("=== Blob Storage Integration Example ===\n");

try
{
    // List existing integrations
    var integrations = await client.GetBlobStorageIntegrationsAsync();
    Console.WriteLine($"Found {integrations.Data.Length} existing integrations");

    // Create new integration
    var createRequest = new CreateBlobStorageIntegrationRequest
    {
        ProjectId = "your-project-id",
        Type = BlobStorageIntegrationType.S3,
        BucketName = "my-langfuse-exports",
        Region = "us-east-1",
        ExportFrequency = BlobStorageExportFrequency.Daily,
        Enabled = false,
        ForcePathStyle = false,
        FileType = BlobStorageIntegrationFileType.Json,
        ExportMode = BlobStorageExportMode.FromToday
    };

    var created = await client.UpsertBlobStorageIntegrationAsync(createRequest);
    Console.WriteLine($"Created integration: {created.Id}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\n=== LLM Connection Example ===\n");

try
{
    // Create LLM connection
    var llmRequest = new UpsertLlmConnectionRequest
    {
        Provider = "my-openai-gateway",
        Adapter = LlmAdapter.OpenAi,
        SecretKey = "sk-your-api-key",
        CustomModels = new[] { "gpt-4-custom" },
        WithDefaultModels = true
    };

    var llmConnection = await client.UpsertLlmConnectionAsync(llmRequest);
    Console.WriteLine($"Created LLM connection: {llmConnection.Id}");
    Console.WriteLine($"Display secret: {llmConnection.DisplaySecretKey}");

    // List connections
    var connections = await client.GetLlmConnectionsAsync();
    Console.WriteLine($"\nFound {connections.Data.Length} LLM connections");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\n=== Trace Filtering Example ===\n");

// Example of using new filter API
var filter = new TraceFilter
{
    Conditions = new[]
    {
        new StringFilterCondition
        {
            Column = "userId",
            Operator = "=",
            Value = "user-123"
        },
        new DateTimeFilterCondition
        {
            Column = "timestamp",
            Operator = ">",
            Value = DateTime.Now.AddDays(-7)
        }
    }
};

var tracesRequest = new GetTracesRequest
{
    Filter = filter,
    Limit = 10
};

var traces = await client.GetTracesAsync(tracesRequest);
Console.WriteLine($"Found {traces.Data.Length} traces matching filter");
```

### Integration Test Scenarios

**1. Blob Storage:**
- [ ] Create integration with all fields
- [ ] Create integration with minimal fields
- [ ] Update existing integration (upsert)
- [ ] List integrations
- [ ] Delete integration
- [ ] Test with S3Compatible type
- [ ] Test with Azure type

**2. LLM Connections:**
- [ ] Create connection
- [ ] Update connection (same provider)
- [ ] List connections with pagination
- [ ] Test different adapters
- [ ] Verify secret not returned

**3. Annotation Queues:**
- [ ] Create queue
- [ ] Create assignment
- [ ] List queue items
- [ ] Delete assignment

**4. Organization Management:**
- [ ] Delete org membership
- [ ] Delete project membership

**5. Score Configs:**
- [ ] Update score config
- [ ] Get scores with sessionId

**6. Trace Filtering:**
- [ ] Filter by string
- [ ] Filter by datetime
- [ ] Filter by multiple conditions
- [ ] Test complex filters

### CI/CD Considerations

**Skip integration tests by default:**
```csharp
[Fact]
[Trait("Category", "Integration")]
public async Task IntegrationTest()
{
    // Skip if not in CI or API keys not configured
    Skip.IfNot(IsIntegrationTestEnvironment(), "Integration tests disabled");
    // ... test code
}
```

**Run in CI only when:**
- Environment variable `RUN_INTEGRATION_TESTS=true`
- API keys are configured as secrets
- Against a test/staging Langfuse instance

## Acceptance Criteria

- [ ] Integration test project created (or example app)
- [ ] Tests/examples for all major new features
- [ ] Blob storage full lifecycle tested
- [ ] LLM connections full lifecycle tested
- [ ] Annotation queue operations tested
- [ ] Organization management tested
- [ ] Score config updates tested
- [ ] Trace filtering tested
- [ ] Tests can be skipped when API keys not configured
- [ ] README added explaining how to run integration tests
- [ ] Environment variable configuration documented
- [ ] Tests pass against real Langfuse instance (or documented why not possible)
- [ ] Example output demonstrates features work correctly
