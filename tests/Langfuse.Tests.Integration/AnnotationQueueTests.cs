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

    [Fact]
    public async Task ListQueuesAsync_ReturnsList()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var result = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 50 });

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        // Note: Queues are typically created via UI, so the list might be empty
    }

    [Fact]
    public async Task GetQueueAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetQueueAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreateItemAsync_WithExistingQueue_AddsTraceToQueue()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // First, check if any queues exist
        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            // Skip test if no queues exist - queues must be created via UI
            return;
        }

        var queueId = queues.Data[0].Id;

        // Create a trace to add to the queue
        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        };

        // Act
        var item = await client.CreateItemAsync(queueId, request);

        // Assert
        item.ShouldNotBeNull();
        item.ObjectId.ShouldBe(traceId);
        item.ObjectType.ShouldBe(AnnotationObjectType.Trace);
    }

    [Fact]
    public async Task ListItemsAsync_WithExistingQueue_ReturnsItems()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // First, check if any queues exist
        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            // Skip test if no queues exist
            return;
        }

        var queueId = queues.Data[0].Id;

        // Create and add a trace to the queue
        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        await client.CreateItemAsync(queueId, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        // Act
        var items = await client.ListItemsAsync(queueId, new AnnotationQueueItemListRequest { Page = 1, Limit = 50 });

        // Assert
        items.ShouldNotBeNull();
        items.Data.ShouldNotBeNull();
        (items.Data.Length >= 1).ShouldBeTrue();
    }

    [Fact]
    public async Task GetItemAsync_WithExistingQueue_ReturnsItem()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // First, check if any queues exist
        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            // Skip test if no queues exist
            return;
        }

        var queueId = queues.Data[0].Id;

        // Create and add a trace to the queue
        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queueId, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        // Act
        var item = await client.GetItemAsync(queueId, createdItem.Id);

        // Assert
        item.ShouldNotBeNull();
        item.Id.ShouldBe(createdItem.Id);
        item.ObjectId.ShouldBe(traceId);
    }

    [Fact]
    public async Task UpdateItemAsync_WithExistingQueue_UpdatesStatus()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // First, check if any queues exist
        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            // Skip test if no queues exist
            return;
        }

        var queueId = queues.Data[0].Id;

        // Create and add a trace to the queue
        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queueId, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace,
            Status = AnnotationQueueStatus.Pending
        });

        // Act
        var updatedItem = await client.UpdateItemAsync(queueId, createdItem.Id, new UpdateAnnotationQueueItemRequest
        {
            Status = AnnotationQueueStatus.Completed
        });

        // Assert
        updatedItem.ShouldNotBeNull();
        updatedItem.Status.ShouldBe(AnnotationQueueStatus.Completed);
    }

    [Fact]
    public async Task DeleteItemAsync_WithExistingQueue_RemovesItem()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // First, check if any queues exist
        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            // Skip test if no queues exist
            return;
        }

        var queueId = queues.Data[0].Id;

        // Create and add a trace to the queue
        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdItem = await client.CreateItemAsync(queueId, new CreateAnnotationQueueItemRequest
        {
            ObjectId = traceId,
            ObjectType = AnnotationObjectType.Trace
        });

        // Act
        var deleteResponse = await client.DeleteItemAsync(queueId, createdItem.Id);

        // Assert
        deleteResponse.ShouldNotBeNull();

        // Verify deletion
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetItemAsync(queueId, createdItem.Id));
        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetItemAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();

        // First, check if any queues exist
        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            // Skip test if no queues exist
            return;
        }

        var queueId = queues.Data[0].Id;
        var nonExistentItemId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetItemAsync(queueId, nonExistentItemId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreateItemAsync_WithObservation_AddsObservationToQueue()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // First, check if any queues exist
        var queues = await client.ListQueuesAsync(new AnnotationQueueListRequest { Page = 1, Limit = 1 });

        if (queues.Data == null || queues.Data.Length == 0)
        {
            // Skip test if no queues exist
            return;
        }

        var queueId = queues.Data[0].Id;

        // Create a trace with an observation
        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var request = new CreateAnnotationQueueItemRequest
        {
            ObjectId = spanId,
            ObjectType = AnnotationObjectType.Observation
        };

        // Act
        var item = await client.CreateItemAsync(queueId, request);

        // Assert
        item.ShouldNotBeNull();
        item.ObjectId.ShouldBe(spanId);
        item.ObjectType.ShouldBe(AnnotationObjectType.Observation);
    }
}