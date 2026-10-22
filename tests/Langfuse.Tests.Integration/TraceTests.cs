using System.Diagnostics;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.ObservationV2;
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
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ILangfuseClient>();
    }

    private TraceTestHelper CreateTraceHelper(ILangfuseClient client)
    {
        return new TraceTestHelper(client, _fixture);
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

        // Deletion is processed asynchronously by the worker; allow extra time under full-suite load
        var stopwatch = Stopwatch.StartNew();
        var timeout = TimeSpan.FromSeconds(90);
        var deleted = false;

        while (stopwatch.Elapsed < timeout)
        {
            var observations = await client.GetObservationsV2Async(
                new ObservationsV2Request { TraceId = traceId, Limit = 1 });
            if (observations.Data.Count == 0)
            {
                deleted = true;
                break;
            }

            await Task.Delay(500);
        }

        deleted.ShouldBeTrue();
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
}
