using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Observation;

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
    public async Task GetObservationListAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateComplexTrace();
        await traceHelper.WaitForTraceAsync(result.TraceId);
        await traceHelper.WaitForObservationAsync(result.SpanId);

        var observations = await client.GetObservationListAsync(new ObservationListRequest { Page = 1, Limit = 50 });

        observations.ShouldNotBeNull();
        observations.Data.ShouldNotBeNull();
        (observations.Data.Length >= 1).ShouldBeTrue();
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByType_Span()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            Type = "SPAN",
            Page = 1,
            Limit = 50
        });

        observations.ShouldNotBeNull();
        observations.Data.ShouldNotBeNull();
        observations.Data.ShouldAllBe(o => o.Type == "SPAN");
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByType_Generation()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            Type = "GENERATION",
            Page = 1,
            Limit = 50
        });

        observations.ShouldNotBeNull();
        observations.Data.ShouldNotBeNull();
        observations.Data.ShouldAllBe(o => o.Type == "GENERATION");
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByType_Event()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, eventId) = traceHelper.CreateTraceWithEvent();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(eventId);

        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            Type = "EVENT",
            Page = 1,
            Limit = 50
        });

        observations.ShouldNotBeNull();
        observations.Data.ShouldNotBeNull();
        observations.Data.ShouldAllBe(o => o.Type == "EVENT");
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByTraceId()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateComplexTrace();
        await traceHelper.WaitForTraceAsync(result.TraceId);
        await traceHelper.WaitForObservationAsync(result.SpanId);

        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            TraceId = result.TraceId,
            Page = 1,
            Limit = 50
        });

        observations.ShouldNotBeNull();
        observations.Data.ShouldNotBeNull();
        (observations.Data.Length >= 3).ShouldBeTrue(); // span, generation, event
        observations.Data.ShouldAllBe(o => o.TraceId == result.TraceId);
    }

    [Fact]
    public async Task GetObservationAsync_ReturnsObservation()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan(spanName: "test-span-retrieval");
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var observation = await client.GetObservationAsync(spanId);

        observation.ShouldNotBeNull();
        observation.Id.ShouldBe(spanId);
        observation.Name.ShouldBe("test-span-retrieval");
        observation.Type.ShouldBe("SPAN");
    }

    [Fact]
    public async Task GetObservationAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetObservationAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetObservationAsync_ReturnsGenerationWithModel()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration(
            generationName: "llm-generation",
            model: "gpt-4-turbo"
        );
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        var observation = await client.GetObservationAsync(generationId);

        observation.ShouldNotBeNull();
        observation.Id.ShouldBe(generationId);
        observation.Name.ShouldBe("llm-generation");
        observation.Type.ShouldBe("GENERATION");
        observation.Model.ShouldBe("gpt-4-turbo");
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByName()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var uniqueName = $"obs-name-{Guid.NewGuid():N}";

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan(spanName: uniqueName);
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            Name = uniqueName,
            Page = 1,
            Limit = 50
        });

        observations.ShouldNotBeNull();
        observations.Data.ShouldNotBeNull();
        observations.Data.ShouldContain(o => o.Name == uniqueName);
    }

    [Fact]
    public async Task GetObservationAsync_Span_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);
        var spanName = $"span-comprehensive-{Guid.NewGuid():N}";

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan(spanName: spanName);
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var observation = await client.GetObservationAsync(spanId);

        observation.Id.ShouldNotBeNullOrEmpty();
        observation.Id.ShouldBe(spanId);
        observation.Type.ShouldBe("SPAN");
        observation.Name.ShouldBe(spanName);
        observation.TraceId.ShouldNotBeNullOrEmpty();
        observation.TraceId.ShouldBe(traceId);
        observation.StartTime.ShouldNotBeNull();
        observation.StartTime.Value.ShouldBeGreaterThan(beforeTest);
        observation.StartTime.Value.ShouldBeLessThan(DateTime.UtcNow.AddMinutes(2));
        observation.EndTime.ShouldNotBeNull();
        observation.EndTime.Value.ShouldBeGreaterThanOrEqualTo(observation.StartTime.Value);
        observation.Input.ShouldNotBeNull();
        observation.Output.ShouldNotBeNull();
        observation.CreatedAt.ShouldBeGreaterThan(beforeTest);
        observation.UpdatedAt.ShouldBeGreaterThan(beforeTest);
    }

    [Fact]
    public async Task GetObservationAsync_Generation_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);
        var generationName = $"gen-comprehensive-{Guid.NewGuid():N}";
        var modelName = "gpt-4-turbo";

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration(
            generationName: generationName,
            model: modelName
        );
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        var observation = await client.GetObservationAsync(generationId);

        observation.Id.ShouldNotBeNullOrEmpty();
        observation.Id.ShouldBe(generationId);
        observation.Type.ShouldBe("GENERATION");
        observation.Name.ShouldBe(generationName);
        observation.TraceId.ShouldNotBeNullOrEmpty();
        observation.TraceId.ShouldBe(traceId);
        observation.Model.ShouldBe(modelName);
        observation.StartTime.ShouldNotBeNull();
        observation.StartTime.Value.ShouldBeGreaterThan(beforeTest);
        observation.StartTime.Value.ShouldBeLessThan(DateTime.UtcNow.AddMinutes(2));
        observation.Input.ShouldNotBeNull();
        observation.Output.ShouldNotBeNull();
        observation.CreatedAt.ShouldBeGreaterThan(beforeTest);
        observation.UpdatedAt.ShouldBeGreaterThan(beforeTest);
    }

    [Fact]
    public async Task GetObservationAsync_Event_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);
        var eventName = $"event-comprehensive-{Guid.NewGuid():N}";

        var (traceId, eventId) = traceHelper.CreateTraceWithEvent(eventName: eventName);
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(eventId);

        var observation = await client.GetObservationAsync(eventId);

        observation.Id.ShouldNotBeNullOrEmpty();
        observation.Id.ShouldBe(eventId);
        observation.Type.ShouldBe("EVENT");
        observation.Name.ShouldBe(eventName);
        observation.TraceId.ShouldNotBeNullOrEmpty();
        observation.TraceId.ShouldBe(traceId);
        observation.StartTime.ShouldNotBeNull();
        observation.StartTime.Value.ShouldBeGreaterThan(beforeTest);
        observation.StartTime.Value.ShouldBeLessThan(DateTime.UtcNow.AddMinutes(2));
        observation.Input.ShouldNotBeNull();
        observation.Output.ShouldNotBeNull();
        observation.CreatedAt.ShouldBeGreaterThan(beforeTest);
        observation.UpdatedAt.ShouldBeGreaterThan(beforeTest);
    }
}