using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Session;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class SessionTests
{
    private readonly LangfuseTestFixture _fixture;

    public SessionTests(LangfuseTestFixture fixture)
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
    public async Task GetSessionListAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var sessionId = $"session-{Guid.NewGuid():N}";

        var traceId = traceHelper.CreateTrace(sessionId: sessionId);
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForSessionAsync(sessionId);

        var result = await client.GetSessionListAsync(new SessionListRequest { Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldContain(s => s.Id == sessionId);
    }

    [Fact]
    public async Task GetSessionAsync_ReturnsSessionWithTraces()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var sessionId = $"session-{Guid.NewGuid():N}";

        var traceId1 = traceHelper.CreateTrace(sessionId: sessionId, name: "trace-1");
        var traceId2 = traceHelper.CreateTrace(sessionId: sessionId, name: "trace-2");

        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);
        await traceHelper.WaitForSessionAsync(sessionId);

        var session = await client.GetSessionAsync(sessionId);

        session.ShouldNotBeNull();
        session.Id.ShouldBe(sessionId);
    }

    [Fact]
    public async Task GetSessionAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var nonExistentId = $"non-existent-{Guid.NewGuid():N}";

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetSessionAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetSessionListAsync_FiltersByFromTimestamp()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var sessionId = $"session-{Guid.NewGuid():N}";
        var beforeCreation = DateTime.UtcNow.AddMinutes(-1);

        var traceId = traceHelper.CreateTrace(sessionId: sessionId);
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForSessionAsync(sessionId);

        var result = await client.GetSessionListAsync(new SessionListRequest
        {
            FromTimestamp = beforeCreation,
            Page = 1,
            Limit = 50
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldContain(s => s.Id == sessionId);
    }

    [Fact]
    public async Task GetSessionListAsync_MultipleTracesInSession()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var sessionId = $"multi-trace-session-{Guid.NewGuid():N}";
        var userId = $"user-{Guid.NewGuid():N}";

        var traceId1 = traceHelper.CreateTrace(sessionId: sessionId, userId: userId, name: "first-trace");
        var traceId2 = traceHelper.CreateTrace(sessionId: sessionId, userId: userId, name: "second-trace");
        var traceId3 = traceHelper.CreateTrace(sessionId: sessionId, userId: userId, name: "third-trace");

        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);
        await traceHelper.WaitForTraceAsync(traceId3);
        await traceHelper.WaitForSessionAsync(sessionId);

        var session = await client.GetSessionAsync(sessionId);

        session.ShouldNotBeNull();
        session.Id.ShouldBe(sessionId);
    }

    [Fact]
    public async Task GetSessionAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);
        var sessionId = $"comprehensive-session-{Guid.NewGuid():N}";
        var userId = $"user-{Guid.NewGuid():N}";

        var traceId1 = traceHelper.CreateTrace(name: "trace-1", sessionId: sessionId, userId: userId);
        var traceId2 = traceHelper.CreateTrace(name: "trace-2", sessionId: sessionId, userId: userId);

        await traceHelper.WaitForTraceAsync(traceId1);
        await traceHelper.WaitForTraceAsync(traceId2);
        await traceHelper.WaitForSessionAsync(sessionId);

        var session = await client.GetSessionAsync(sessionId);

        session.Id.ShouldBe(sessionId);
        session.CreatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        session.Environment.ShouldBe("default");
        session.ProjectId.ShouldBe("test-project");
        session.Traces.ShouldNotBeNull();
        session.Traces.Length.ShouldBe(2);
    }
}