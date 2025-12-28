using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.AnnotationQueue;
using zborek.Langfuse.Models.Core;

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

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task ListQueuesAsync_ReturnsList()
    {
        var client = CreateClient();

        var result = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
    }

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task GetQueueAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetQueueAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task CreateItemAsync_WithExistingQueue_AddsTraceToQueue()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            throw new Exception("No queues found");
        }

        var queueId = queues.Data[0].Id;

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        };

        var item = await client.CreateItemAsync(queueId, request);

        item.ShouldNotBeNull();
        item.ObjectId.ShouldBe(traceId);
        item.ObjectType.ShouldBe(AnnotationObjectType.Trace);
    }

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task ListItemsAsync_WithExistingQueue_ReturnsItems()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            throw new Exception("No queues found");
        }

        var queueId = queues.Data[0].Id;

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        await client.CreateItemAsync(queueId, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        var items = await client.ListItemsAsync(queueId, new AnnotationQueueItemListRequest { Page = 1, Limit = 50 });

        items.ShouldNotBeNull();
        items.Data.ShouldNotBeNull();
        items.Data.Length.ShouldBeGreaterThanOrEqualTo(1);
        items.Data.ShouldContain(i => i.ObjectId == traceId && i.ObjectType == AnnotationObjectType.Trace);
    }

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task GetItemAsync_WithExistingQueue_ReturnsItem()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            throw new Exception("No queues found");
        }

        var queueId = queues.Data[0].Id;

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queueId, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        var item = await client.GetItemAsync(queueId, createdItem.Id);

        item.ShouldNotBeNull();
        item.Id.ShouldBe(createdItem.Id);
        item.ObjectId.ShouldBe(traceId);
    }

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task UpdateItemAsync_WithExistingQueue_UpdatesStatus()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data.Length == 0)
        {
            throw new Exception("No queues found");
        }

        var queueId = queues.Data[0].Id;

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queueId, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace,
            Status = AnnotationQueueStatus.Pending
        });

        var updatedItem = await client.UpdateItemAsync(queueId, createdItem.Id, new UpdateAnnotationQueueItemRequest
        {
            Status = AnnotationQueueStatus.Completed
        });

        updatedItem.ShouldNotBeNull();
        updatedItem.Status.ShouldBe(AnnotationQueueStatus.Completed);
    }

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task DeleteItemAsync_WithExistingQueue_RemovesItem()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data.Length == 0)
        {
            throw new Exception("No queues found");
        }

        var queueId = queues.Data[0].Id;

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queueId, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        var deleteResponse = await client.DeleteItemAsync(queueId, createdItem.Id);

        deleteResponse.ShouldNotBeNull();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetItemAsync(queueId, createdItem.Id));
        exception.StatusCode.ShouldBe(404);
    }

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task GetItemAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();

        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data.Length == 0)
        {
            throw new Exception("No queues found");
        }

        var queueId = queues.Data[0].Id;
        var nonExistentItemId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetItemAsync(queueId, nonExistentItemId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact(Skip = "Queues are created via UI, so the list might be empty")]
    public async Task CreateItemAsync_WithObservation_AddsObservationToQueue()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data.Length == 0)
        {
            throw new Exception("No queues found");
        }

        var queueId = queues.Data[0].Id;

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var request = new CreateAnnotationQueueItemRequest
        {
            ObjectId = spanId,
            ObjectType = AnnotationObjectType.Observation
        };

        var item = await client.CreateItemAsync(queueId, request);

        item.ShouldNotBeNull();
        item.ObjectId.ShouldBe(spanId);
        item.ObjectType.ShouldBe(AnnotationObjectType.Observation);
    }
}