using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Dataset;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class DatasetRunItemTests
{
    private readonly LangfuseTestFixture _fixture;

    public DatasetRunItemTests(LangfuseTestFixture fixture)
    {
        _fixture = fixture;
    }

    private ILangfuseClient CreateClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLangfuse(config =>
        {
            config.Url = _fixture.LangfuseBaseUrl;
            config.PublicKey = _fixture.PublicKey;
            config.SecretKey = _fixture.SecretKey;
            config.BatchMode = false;
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ILangfuseClient>();
    }

    private TraceTestHelper CreateTraceHelper(ILangfuseClient client)
    {
        return new TraceTestHelper(client, _fixture);
    }

    private async Task<(string DatasetName, string ItemId)> CreateTestDatasetWithItemAsync(ILangfuseClient client)
    {
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Test dataset for run item tests"
        });

        var item = await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "What is the capital of France?" },
            ExpectedOutput = new { answer = "Paris" }
        });

        return (datasetName, item.Id);
    }

    [Fact]
    public async Task CreateDataSetRunAsync_LinksToTrace()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (datasetName, itemId) = await CreateTestDatasetWithItemAsync(client);
        var runName = $"test-run-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace("evaluation-trace");
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new CreateDatasetRunItemRequest
        {
            DatasetItemId = itemId,
            RunName = runName,
            TraceId = traceId,
            RunDescription = "Test run for integration tests",
            Metadata = new { evaluator = "integration-test" }
        };

        // Act
        var runItem = await client.CreateDataSetRunAsync(request);

        // Assert
        Assert.NotNull(runItem);
        Assert.Equal(itemId, runItem.DatasetItemId);
        Assert.Equal(traceId, runItem.TraceId);
        Assert.Equal(runName, runItem.DatasetRunName);
    }

    [Fact]
    public async Task CreateDataSetRunAsync_WithObservation_LinksToObservation()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (datasetName, itemId) = await CreateTestDatasetWithItemAsync(client);
        var runName = $"test-run-obs-{Guid.NewGuid():N}";

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        var request = new CreateDatasetRunItemRequest
        {
            DatasetItemId = itemId,
            RunName = runName,
            TraceId = traceId,
            ObservationId = generationId
        };

        // Act
        var runItem = await client.CreateDataSetRunAsync(request);

        // Assert
        Assert.NotNull(runItem);
        Assert.Equal(generationId, runItem.ObservationId);
    }

    [Fact]
    public async Task GetDatasetRunListAsync_ReturnsRunItems()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // Get or create dataset
        var (datasetName, itemId) = await CreateTestDatasetWithItemAsync(client);
        var runName = $"list-run-{Guid.NewGuid():N}";

        // Get dataset to get its ID
        var dataset = await client.GetDatasetAsync(datasetName);

        // Create a trace and run item
        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        await client.CreateDataSetRunAsync(new CreateDatasetRunItemRequest
        {
            DatasetItemId = itemId,
            RunName = runName,
            TraceId = traceId
        });

        // Act
        var result = await client.GetDatasetRunListAsync(new DatasetRunItemListRequest
        {
            DatasetId = dataset.Id,
            RunName = runName,
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Length >= 1);
    }

    [Fact]
    public async Task CreateDataSetRunAsync_MultipleItemsSameRun()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Test dataset with multiple items"
        });

        // Create multiple dataset items
        var item1 = await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "Question 1" },
            ExpectedOutput = new { answer = "Answer 1" }
        });

        var item2 = await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "Question 2" },
            ExpectedOutput = new { answer = "Answer 2" }
        });

        var runName = $"multi-item-run-{Guid.NewGuid():N}";

        // Create traces for each item
        var traceId1 = traceHelper.CreateTrace("trace-1");
        var traceId2 = traceHelper.CreateTrace("trace-2");
        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);

        // Act - Create run items for the same run
        var runItem1 = await client.CreateDataSetRunAsync(new CreateDatasetRunItemRequest
        {
            DatasetItemId = item1.Id,
            RunName = runName,
            TraceId = traceId1
        });

        var runItem2 = await client.CreateDataSetRunAsync(new CreateDatasetRunItemRequest
        {
            DatasetItemId = item2.Id,
            RunName = runName,
            TraceId = traceId2
        });

        // Assert
        Assert.NotNull(runItem1);
        Assert.NotNull(runItem2);
        Assert.Equal(runName, runItem1.DatasetRunName);
        Assert.Equal(runName, runItem2.DatasetRunName);
    }

    [Fact]
    public async Task CreateDataSetRunAsync_CreatesNewRun()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (datasetName, itemId) = await CreateTestDatasetWithItemAsync(client);
        var runName = $"new-run-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        // Act
        var runItem = await client.CreateDataSetRunAsync(new CreateDatasetRunItemRequest
        {
            DatasetItemId = itemId,
            RunName = runName,
            TraceId = traceId,
            RunDescription = "New run description"
        });

        // Assert
        Assert.NotNull(runItem);
        Assert.NotNull(runItem.Id);

        // Verify the run exists
        var runs = await client.GetDatasetRunsAsync(datasetName, new DatasetRunListRequest { Page = 1, Limit = 50 });
        Assert.Contains(runs.Data, r => r.Name == runName);
    }

    [Fact]
    public async Task GetDatasetRunListAsync_Pagination()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (datasetName, _) = await CreateTestDatasetWithItemAsync(client);
        var dataset = await client.GetDatasetAsync(datasetName);
        var runName = $"pagination-run-{Guid.NewGuid():N}";

        // Create multiple items and run items
        for (var i = 0; i < 3; i++)
        {
            var item = await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
            {
                DatasetName = datasetName,
                Input = new { index = i },
                ExpectedOutput = new { result = i }
            });

            var traceId = traceHelper.CreateTrace();
            await traceHelper.WaitForTraceAsync(traceId);

            await client.CreateDataSetRunAsync(new CreateDatasetRunItemRequest
            {
                DatasetItemId = item.Id,
                RunName = runName,
                TraceId = traceId
            });
        }

        // Act
        var page1 = await client.GetDatasetRunListAsync(new DatasetRunItemListRequest
        {
            DatasetId = dataset.Id,
            RunName = runName,
            Page = 1,
            Limit = 2
        });

        var page2 = await client.GetDatasetRunListAsync(new DatasetRunItemListRequest
        {
            DatasetId = dataset.Id,
            RunName = runName,
            Page = 2,
            Limit = 2
        });

        // Assert
        Assert.NotNull(page1);
        Assert.NotNull(page1.Data);
        Assert.True(page1.Data.Length <= 2);
    }
}