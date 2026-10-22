using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.ObservationV2;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class ObservationTests
{
    private readonly LangfuseTestFixture _fixture;

    public ObservationTests(LangfuseTestFixture fixture)
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
    public async Task GetObservationsV2Async_FiltersByTraceId()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateComplexTrace();
        await traceHelper.WaitForObservationAsync(result.SpanId);
        await traceHelper.WaitForObservationAsync(result.GenerationId);
        await traceHelper.WaitForObservationAsync(result.EventId);

        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = result.TraceId,
            Limit = 50
        });

        observations.Data.ShouldAllBe(o => TraceTestHelper.GetString(o, "traceId") == result.TraceId);
        observations.Data.ShouldContain(o =>
            TraceTestHelper.GetString(o, "id") == result.SpanId && TraceTestHelper.GetString(o, "type") == "SPAN");
        observations.Data.ShouldContain(o =>
            TraceTestHelper.GetString(o, "id") == result.GenerationId &&
            TraceTestHelper.GetString(o, "type") == "GENERATION");
        observations.Data.ShouldContain(o =>
            TraceTestHelper.GetString(o, "id") == result.EventId && TraceTestHelper.GetString(o, "type") == "EVENT");
    }

    [Fact]
    public async Task GetObservationsV2Async_PaginatesWithCursor()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateComplexTrace();
        await traceHelper.WaitForObservationAsync(result.SpanId);
        await traceHelper.WaitForObservationAsync(result.GenerationId);
        await traceHelper.WaitForObservationAsync(result.EventId);

        var page1 = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = result.TraceId,
            Limit = 1
        });

        page1.Data.Count.ShouldBe(1);
        page1.Meta.Cursor.ShouldNotBeNullOrEmpty();

        var page2 = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = result.TraceId,
            Limit = 1,
            Cursor = page1.Meta.Cursor
        });

        page2.Data.Count.ShouldBe(1);
        TraceTestHelper.GetString(page2.Data[0], "id").ShouldNotBe(TraceTestHelper.GetString(page1.Data[0], "id"));
    }

    [Theory]
    [InlineData("SPAN")]
    [InlineData("GENERATION")]
    [InlineData("EVENT")]
    public async Task GetObservationsV2Async_FiltersByType(string type)
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateComplexTrace();
        await traceHelper.WaitForObservationAsync(result.SpanId);
        await traceHelper.WaitForObservationAsync(result.GenerationId);
        await traceHelper.WaitForObservationAsync(result.EventId);

        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = result.TraceId,
            Type = type,
            Limit = 50
        });

        observations.Data.ShouldNotBeEmpty();
        observations.Data.ShouldAllBe(o => TraceTestHelper.GetString(o, "type") == type);
    }

    [Fact]
    public async Task GetObservationsV2Async_FiltersByName()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var uniqueName = $"obs-name-{Guid.NewGuid():N}";

        var (_, spanId) = traceHelper.CreateTraceWithSpan(spanName: uniqueName);
        await traceHelper.WaitForObservationAsync(spanId);

        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            Name = uniqueName,
            Limit = 50
        });

        observations.Data.ShouldNotBeEmpty();
        observations.Data.ShouldAllBe(o => TraceTestHelper.GetString(o, "name") == uniqueName);
    }

    [Fact]
    public async Task GetObservationsV2Async_Span_ReturnsCoreAndBasicFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var spanName = $"span-comprehensive-{Guid.NewGuid():N}";

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan(spanName: spanName);

        var observation = await traceHelper.GetObservationV2Async(spanId, "core,basic,io");

        TraceTestHelper.GetString(observation, "id").ShouldBe(spanId);
        TraceTestHelper.GetString(observation, "traceId").ShouldBe(traceId);
        TraceTestHelper.GetString(observation, "type").ShouldBe("SPAN");
        TraceTestHelper.GetString(observation, "name").ShouldBe(spanName);
        TraceTestHelper.GetString(observation, "startTime").ShouldNotBeNullOrEmpty();
        TraceTestHelper.GetString(observation, "endTime").ShouldNotBeNullOrEmpty();
        TraceTestHelper.GetString(observation, "input").ShouldNotBeNullOrEmpty();
        TraceTestHelper.GetString(observation, "output").ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetObservationsV2Async_Generation_ReturnsModel()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var generationName = $"gen-comprehensive-{Guid.NewGuid():N}";

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration(
            generationName: generationName,
            model: "gpt-4-turbo"
        );

        var observation = await traceHelper.GetObservationV2Async(generationId, "core,basic,model");

        TraceTestHelper.GetString(observation, "id").ShouldBe(generationId);
        TraceTestHelper.GetString(observation, "traceId").ShouldBe(traceId);
        TraceTestHelper.GetString(observation, "type").ShouldBe("GENERATION");
        TraceTestHelper.GetString(observation, "name").ShouldBe(generationName);
        TraceTestHelper.GetString(observation, "model").ShouldBe("gpt-4-turbo");
    }

    [Fact]
    public async Task SkippedSpan_IsNotExportedToLangfuse()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, skippedSpanId) = traceHelper.CreateTraceWithSkippedSpan();
        await traceHelper.WaitForTraceAsync(traceId);

        var exists = await traceHelper.ObservationExistsAsync(skippedSpanId);

        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task SkippedGeneration_IsNotExportedToLangfuse()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, skippedGenerationId) = traceHelper.CreateTraceWithSkippedGeneration();
        await traceHelper.WaitForTraceAsync(traceId);

        var exists = await traceHelper.ObservationExistsAsync(skippedGenerationId);

        exists.ShouldBeFalse();
    }

    [Fact]
    public async Task MixedObservations_OnlyActiveObservationsAreExported()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, activeSpanId, skippedSpanId) = traceHelper.CreateTraceWithMixedObservations();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(activeSpanId);

        var activeExists = await traceHelper.ObservationExistsAsync(activeSpanId);
        var skippedExists = await traceHelper.ObservationExistsAsync(skippedSpanId);

        activeExists.ShouldBeTrue();
        skippedExists.ShouldBeFalse();
    }

    [Fact]
    public async Task SkippedSpan_TraceStillExistsWithoutSkippedObservation()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, skippedSpanId) = traceHelper.CreateTraceWithSkippedSpan();
        await traceHelper.WaitForTraceAsync(traceId);

        var observations = await client.GetObservationsV2Async(new ObservationsV2Request
        {
            TraceId = traceId,
            Limit = 50
        });

        observations.Data.ShouldNotBeEmpty();
        observations.Data.ShouldNotContain(o => TraceTestHelper.GetString(o, "id") == skippedSpanId);
    }
}
