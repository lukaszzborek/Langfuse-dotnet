using System.Diagnostics;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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

        var runItem = await client.CreateDataSetRunAsync(request);

        runItem.ShouldNotBeNull();
        runItem.DatasetItemId.ShouldBe(itemId);
        runItem.TraceId.ShouldBe(traceId);
        runItem.DatasetRunName.ShouldBe(runName);
    }

    [Fact]
    public async Task CreateDataSetRunAsync_WithObservation_LinksToObservation()
    {
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

        var runItem = await client.CreateDataSetRunAsync(request);

        runItem.ShouldNotBeNull();
        runItem.ObservationId.ShouldBe(generationId);
    }

    [Fact]
    public async Task GetDatasetRunListAsync_ReturnsRunItems()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (datasetName, itemId) = await CreateTestDatasetWithItemAsync(client);
        var runName = $"list-run-{Guid.NewGuid():N}";

        var dataset = await client.GetDatasetAsync(datasetName);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        await client.CreateDataSetRunAsync(new CreateDatasetRunItemRequest
        {
            DatasetItemId = itemId,
            RunName = runName,
            TraceId = traceId
        });

        var stopwatch = Stopwatch.StartNew();
        var timeout = TimeSpan.FromSeconds(30);
        PaginatedDatasetRunItems? result = null;

        while (stopwatch.Elapsed < timeout)
        {
            result = await client.GetDatasetRunListAsync(new DatasetRunItemListRequest
            {
                DatasetId = dataset.Id,
                RunName = runName,
                Page = 1,
                Limit = 50
            });

            if (result.Data.Length >= 1)
            {
                break;
            }

            await Task.Delay(500);
        }

        result.ShouldNotBeNull();
        (result.Data.Length >= 1).ShouldBeTrue();
    }

    [Fact]
    public async Task CreateDataSetRunAsync_MultipleItemsSameRun()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Test dataset with multiple items"
        });

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

        var traceId1 = traceHelper.CreateTrace("trace-1");
        var traceId2 = traceHelper.CreateTrace("trace-2");
        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);

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

        runItem1.ShouldNotBeNull();
        runItem2.ShouldNotBeNull();
        runItem1.DatasetRunName.ShouldBe(runName);
        runItem2.DatasetRunName.ShouldBe(runName);
    }

    [Fact]
    public async Task CreateDataSetRunAsync_CreatesNewRun()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (datasetName, itemId) = await CreateTestDatasetWithItemAsync(client);
        var runName = $"new-run-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var runItem = await client.CreateDataSetRunAsync(new CreateDatasetRunItemRequest
        {
            DatasetItemId = itemId,
            RunName = runName,
            TraceId = traceId,
            RunDescription = "New run description"
        });

        runItem.ShouldNotBeNull();
        runItem.Id.ShouldNotBeNull();

        var runs = await client.GetDatasetRunsAsync(datasetName, new DatasetRunListRequest { Page = 1, Limit = 50 });
        runs.Data.ShouldContain(r => r.Name == runName);
    }

    [Fact]
    public async Task GetDatasetRunListAsync_Pagination()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (datasetName, _) = await CreateTestDatasetWithItemAsync(client);
        var dataset = await client.GetDatasetAsync(datasetName);
        var runName = $"pagination-run-{Guid.NewGuid():N}";

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

        page1.ShouldNotBeNull();
        page1.Data.ShouldNotBeNull();
        (page1.Data.Length <= 2).ShouldBeTrue();
    }
}