using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.AnnotationQueue;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class AnnotationQueueTests
{
    private readonly LangfuseTestFixture _fixture;

    public AnnotationQueueTests(LangfuseTestFixture fixture)
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

    private async Task<string> CreateScoreConfigAsync(ILangfuseClient client)
    {
        var scoreConfig = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = $"config-{Guid.NewGuid():N}"[..20],
            DataType = ScoreDataType.Numeric,
            MinValue = 0,
            MaxValue = 1
        });
        return scoreConfig.Id;
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_CreatesQueue()
    {
        var client = CreateClient();
        var queueName = $"test-queue-{Guid.NewGuid():N}"[..30];
        var scoreConfigId = await CreateScoreConfigAsync(client);

        var request = new CreateAnnotationQueueRequest
        {
            Name = queueName,
            Description = "Integration test queue",
            ScoreConfigIds = [scoreConfigId]
        };

        var queue = await client.CreateAnnotationQueueAsync(request);

        queue.ShouldNotBeNull();
        queue.Id.ShouldNotBeNullOrEmpty();
        queue.Name.ShouldBe(queueName);
        queue.Description.ShouldBe("Integration test queue");
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_WithScoreConfigs_CreatesQueue()
    {
        var client = CreateClient();
        var queueName = $"test-queue-cfg-{Guid.NewGuid():N}"[..30];

        // First create a score config to reference
        var scoreConfig = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = $"config-{Guid.NewGuid():N}"[..20],
            DataType = ScoreDataType.Numeric,
            MinValue = 0,
            MaxValue = 1
        });

        var request = new CreateAnnotationQueueRequest
        {
            Name = queueName,
            Description = "Queue with score configs",
            ScoreConfigIds = new[] { scoreConfig.Id }
        };

        var queue = await client.CreateAnnotationQueueAsync(request);

        queue.ShouldNotBeNull();
        queue.Id.ShouldNotBeNullOrEmpty();
        queue.Name.ShouldBe(queueName);
        queue.ScoreConfigIds.ShouldContain(scoreConfig.Id);
    }

    [Fact]
    public async Task ListQueuesAsync_ReturnsList()
    {
        var client = CreateClient();
        var scoreConfigId = await CreateScoreConfigAsync(client);

        // Create a queue first to ensure there's at least one
        var queueName = $"list-test-{Guid.NewGuid():N}"[..30];
        await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = queueName,
            Description = "List test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var result = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task GetQueueAsync_ReturnsQueue()
    {
        var client = CreateClient();
        var queueName = $"get-test-{Guid.NewGuid():N}"[..30];
        var scoreConfigId = await CreateScoreConfigAsync(client);

        var createdQueue = await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = queueName,
            Description = "Get test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var queue = await client.GetQueueAsync(createdQueue.Id);

        queue.ShouldNotBeNull();
        queue.Id.ShouldBe(createdQueue.Id);
        queue.Name.ShouldBe(queueName);
    }

    [Fact]
    public async Task GetQueueAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetQueueAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreateItemAsync_WithExistingQueue_AddsTraceToQueue()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var scoreConfigId = await CreateScoreConfigAsync(client);

        // Create a queue first
        var queue = await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = $"item-test-{Guid.NewGuid():N}"[..30],
            Description = "Item test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        };

        var item = await client.CreateItemAsync(queue.Id, request);

        item.ShouldNotBeNull();
        item.ObjectId.ShouldBe(traceId);
        item.ObjectType.ShouldBe(AnnotationObjectType.Trace);
    }

    [Fact]
    public async Task ListItemsAsync_WithExistingQueue_ReturnsItems()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var scoreConfigId = await CreateScoreConfigAsync(client);

        // Create a queue first
        var queue = await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = $"list-items-{Guid.NewGuid():N}"[..30],
            Description = "List items test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        await client.CreateItemAsync(queue.Id, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        var items = await client.ListItemsAsync(queue.Id, new AnnotationQueueItemListRequest { Page = 1, Limit = 50 });

        items.ShouldNotBeNull();
        items.Data.ShouldNotBeNull();
        items.Data.Length.ShouldBeGreaterThanOrEqualTo(1);
        items.Data.ShouldContain(i => i.ObjectId == traceId && i.ObjectType == AnnotationObjectType.Trace);
    }

    [Fact]
    public async Task GetItemAsync_WithExistingQueue_ReturnsItem()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var scoreConfigId = await CreateScoreConfigAsync(client);

        // Create a queue first
        var queue = await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = $"get-item-{Guid.NewGuid():N}"[..30],
            Description = "Get item test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queue.Id, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        var item = await client.GetItemAsync(queue.Id, createdItem.Id);

        item.ShouldNotBeNull();
        item.Id.ShouldBe(createdItem.Id);
        item.ObjectId.ShouldBe(traceId);
    }

    [Fact]
    public async Task UpdateItemAsync_WithExistingQueue_UpdatesStatus()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var scoreConfigId = await CreateScoreConfigAsync(client);

        // Create a queue first
        var queue = await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = $"update-item-{Guid.NewGuid():N}"[..30],
            Description = "Update item test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queue.Id, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace,
            Status = AnnotationQueueStatus.Pending
        });

        var updatedItem = await client.UpdateItemAsync(queue.Id, createdItem.Id, new UpdateAnnotationQueueItemRequest
        {
            Status = AnnotationQueueStatus.Completed
        });

        updatedItem.ShouldNotBeNull();
        updatedItem.Status.ShouldBe(AnnotationQueueStatus.Completed);
    }

    [Fact]
    public async Task DeleteItemAsync_WithExistingQueue_RemovesItem()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var scoreConfigId = await CreateScoreConfigAsync(client);

        // Create a queue first
        var queue = await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = $"delete-item-{Guid.NewGuid():N}"[..30],
            Description = "Delete item test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queue.Id, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        var deleteResponse = await client.DeleteItemAsync(queue.Id, createdItem.Id);

        deleteResponse.ShouldNotBeNull();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetItemAsync(queue.Id, createdItem.Id));
        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetItemAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var scoreConfigId = await CreateScoreConfigAsync(client);

        // Create a queue first
        var queue = await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = $"not-found-{Guid.NewGuid():N}"[..30],
            Description = "Not found test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var nonExistentItemId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetItemAsync(queue.Id, nonExistentItemId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreateItemAsync_WithObservation_AddsObservationToQueue()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var scoreConfigId = await CreateScoreConfigAsync(client);

        // Create a queue first
        var queue = await client.CreateAnnotationQueueAsync(new CreateAnnotationQueueRequest
        {
            Name = $"obs-item-{Guid.NewGuid():N}"[..30],
            Description = "Observation item test queue",
            ScoreConfigIds = [scoreConfigId]
        });

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var request = new CreateAnnotationQueueItemRequest
        {
            ObjectId = spanId,
            ObjectType = AnnotationObjectType.Observation
        };

        var item = await client.CreateItemAsync(queue.Id, request);

        item.ShouldNotBeNull();
        item.ObjectId.ShouldBe(spanId);
        item.ObjectType.ShouldBe(AnnotationObjectType.Observation);
    }
}