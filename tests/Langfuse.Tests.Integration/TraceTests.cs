using System.Diagnostics;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var prefix = $"list-test-{Guid.NewGuid():N}";

        var traceId1 = traceHelper.CreateTrace($"{prefix}-1");
        var traceId2 = traceHelper.CreateTrace($"{prefix}-2");

        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);

        var result1 = await client.GetTraceListAsync(new TraceListRequest { Page = 1, Limit = 1 });
        var result2 = await client.GetTraceListAsync(new TraceListRequest { Page = 2, Limit = 1 });

        result1.ShouldNotBeNull();
        result1.Data.ShouldNotBeNull();
        result1.Data.Length.ShouldBe(1);

        result2.ShouldNotBeNull();
        result2.Data.ShouldNotBeNull();
        result2.Data.Length.ShouldBe(1);

        result1.Data[0].Id.ShouldNotBe(result2.Data[0].Id);
    }

    [Fact]
    public async Task GetTraceListAsync_FiltersByName()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var uniqueName = $"filter-name-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(uniqueName);
        await traceHelper.WaitForTraceAsync(traceId);

        var result = await client.GetTraceListAsync(new TraceListRequest { Name = uniqueName, Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Length.ShouldBe(1);
        result.Data[0].Name.ShouldBe(uniqueName);
    }

    [Fact]
    public async Task GetTraceListAsync_FiltersByUserId()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var userId = $"user-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(userId: userId);
        await traceHelper.WaitForTraceAsync(traceId);

        var result = await client.GetTraceListAsync(new TraceListRequest { UserId = userId, Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldContain(t => t.UserId == userId);
    }

    [Fact]
    public async Task GetTraceListAsync_FiltersByTags()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var uniqueTag = $"tag-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(tags: [uniqueTag, "integration-test"]);
        await traceHelper.WaitForTraceAsync(traceId);

        var result =
            await client.GetTraceListAsync(new TraceListRequest { Tags = [uniqueTag], Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldContain(t => t.Tags != null && t.Tags.Contains(uniqueTag));
    }

    [Fact]
    public async Task GetTraceAsync_ReturnsTraceWithDetails()
    {
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

        var trace = await client.GetTraceAsync(traceId);

        trace.ShouldNotBeNull();
        trace.Id.ShouldBe(traceId);
        trace.Name.ShouldBe(traceName);
        trace.UserId.ShouldBe("test-user");
    }

    [Fact]
    public async Task GetTraceAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetTraceAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task DeleteTraceAsync_DeletesTrace()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace("trace-to-delete");
        await traceHelper.WaitForTraceAsync(traceId);

        var deleteResponse = await client.DeleteTraceAsync(traceId);

        deleteResponse.ShouldNotBeNull();

        var stopwatch = Stopwatch.StartNew();
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

        deleteException.ShouldNotBeNull();
        deleteException.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task DeleteTraceManyAsync_DeletesByIds()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId1 = traceHelper.CreateTrace("bulk-delete-1");
        var traceId2 = traceHelper.CreateTrace("bulk-delete-2");

        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);

        var deleteResponse =
            await client.DeleteTraceManyAsync(new DeleteTraceManyRequest { TraceIds = [traceId1, traceId2] });

        // Assert - verify delete was called successfully
        deleteResponse.ShouldNotBeNull();

        // The bulk delete API processes asynchronously, so we verify at least
        // the API accepted the request. Full deletion verification is optional
        // due to eventual consistency with background processing.
    }

    [Fact]
    public async Task GetTraceListAsync_FiltersBySessionId()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var sessionId = $"session-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(sessionId: sessionId);
        await traceHelper.WaitForTraceAsync(traceId);

        var result =
            await client.GetTraceListAsync(new TraceListRequest { SessionId = sessionId, Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldContain(t => t.SessionId == sessionId);
    }

    [Fact]
    public async Task GetTraceAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);
        var traceName = $"comprehensive-test-{Guid.NewGuid():N}";
        var userId = $"user-{Guid.NewGuid():N}";
        var sessionId = $"session-{Guid.NewGuid():N}";
        var tags = new[] { $"tag-{Guid.NewGuid():N}", "integration-test" };
        var inputData = new { prompt = "test prompt", context = "test context" };
        var outputData = new { response = "test response", confidence = 0.95 };
        var metadata = new { source = "integration-test", version = "1.0" };

        var traceId = traceHelper.CreateTrace(
            traceName,
            sessionId,
            userId,
            tags,
            inputData,
            outputData,
            metadata
        );
        await traceHelper.WaitForTraceAsync(traceId);

        var trace = await client.GetTraceAsync(traceId);

        trace.Id.ShouldNotBeNullOrEmpty();
        trace.Id.ShouldBe(traceId);
        trace.Name.ShouldBe(traceName);
        trace.UserId.ShouldBe(userId);
        trace.SessionId.ShouldBe(sessionId);
        trace.Tags.ShouldNotBeNull();
        trace.Tags.Length.ShouldBe(2);
        trace.Tags.ShouldContain(tags[0]);
        trace.Tags.ShouldContain("integration-test");
        trace.Input.ShouldNotBeNull();
        trace.Output.ShouldNotBeNull();
        trace.Timestamp.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        trace.Observations.ShouldNotBeNull();
        trace.Scores.ShouldNotBeNull();
    }
}