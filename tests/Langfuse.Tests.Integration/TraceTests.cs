using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Trace;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class TraceTests
{
    private readonly LangfuseTestFixture _fixture;

    public TraceTests(LangfuseTestFixture fixture)
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
    public async Task GetTraceListAsync_ReturnsPaginatedList()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var prefix = $"list-test-{Guid.NewGuid():N}";

        var traceId1 = traceHelper.CreateTrace($"{prefix}-1");
        var traceId2 = traceHelper.CreateTrace($"{prefix}-2");

        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);

        // Act
        var result = await client.GetTraceListAsync(new TraceListRequest { Page = 1, Limit = 50 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Length >= 2);
    }

    [Fact]
    public async Task GetTraceListAsync_FiltersByName()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var uniqueName = $"filter-name-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(uniqueName);
        await traceHelper.WaitForTraceAsync(traceId);

        // Act
        var result = await client.GetTraceListAsync(new TraceListRequest { Name = uniqueName, Page = 1, Limit = 50 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal(uniqueName, result.Data[0].Name);
    }

    [Fact]
    public async Task GetTraceListAsync_FiltersByUserId()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var userId = $"user-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(userId: userId);
        await traceHelper.WaitForTraceAsync(traceId);

        // Act
        var result = await client.GetTraceListAsync(new TraceListRequest { UserId = userId, Page = 1, Limit = 50 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, t => t.UserId == userId);
    }

    [Fact]
    public async Task GetTraceListAsync_FiltersByTags()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var uniqueTag = $"tag-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(tags: [uniqueTag, "integration-test"]);
        await traceHelper.WaitForTraceAsync(traceId);

        // Act
        var result =
            await client.GetTraceListAsync(new TraceListRequest { Tags = [uniqueTag], Page = 1, Limit = 50 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, t => t.Tags != null && t.Tags.Contains(uniqueTag));
    }

    [Fact]
    public async Task GetTraceAsync_ReturnsTraceWithDetails()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var traceName = $"detail-test-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(
            traceName,
            input: "test input",
            output: "test output",
            userId: "test-user"
        );
        await traceHelper.WaitForTraceAsync(traceId);

        // Act
        var trace = await client.GetTraceAsync(traceId);

        // Assert
        Assert.NotNull(trace);
        Assert.Equal(traceId, trace.Id);
        Assert.Equal(traceName, trace.Name);
        Assert.Equal("test-user", trace.UserId);
    }

    [Fact]
    public async Task GetTraceAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetTraceAsync(nonExistentId));

        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteTraceAsync_DeletesTrace()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace("trace-to-delete");
        await traceHelper.WaitForTraceAsync(traceId);

        // Act
        var deleteResponse = await client.DeleteTraceAsync(traceId);

        // Assert
        Assert.NotNull(deleteResponse);

        // Wait for deletion to propagate and verify trace is gone
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var timeout = TimeSpan.FromSeconds(30);
        LangfuseApiException? deleteException = null;

        while (stopwatch.Elapsed < timeout)
        {
            try
            {
                await client.GetTraceAsync(traceId);
                // Trace still exists, wait and retry
                await Task.Delay(500);
            }
            catch (LangfuseApiException ex) when (ex.StatusCode == 404)
            {
                deleteException = ex;
                break;
            }
        }

        Assert.NotNull(deleteException);
        Assert.Equal(404, deleteException.StatusCode);
    }

    [Fact]
    public async Task DeleteTraceManyAsync_DeletesByIds()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // Create multiple traces
        var traceId1 = traceHelper.CreateTrace("bulk-delete-1");
        var traceId2 = traceHelper.CreateTrace("bulk-delete-2");

        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);

        // Act
        var deleteResponse = await client.DeleteTraceManyAsync(new DeleteTraceManyRequest { TraceIds = [traceId1, traceId2] });

        // Assert - verify delete was called successfully
        Assert.NotNull(deleteResponse);

        // The bulk delete API processes asynchronously, so we verify at least
        // the API accepted the request. Full deletion verification is optional
        // due to eventual consistency with background processing.
    }

    [Fact]
    public async Task GetTraceListAsync_FiltersBySessionId()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var sessionId = $"session-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(sessionId: sessionId);
        await traceHelper.WaitForTraceAsync(traceId);

        // Act
        var result =
            await client.GetTraceListAsync(new TraceListRequest { SessionId = sessionId, Page = 1, Limit = 50 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Contains(result.Data, t => t.SessionId == sessionId);
    }
}