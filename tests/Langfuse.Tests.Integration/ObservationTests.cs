using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
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
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateComplexTrace();
        await traceHelper.WaitForTraceAsync(result.TraceId);
        await traceHelper.WaitForObservationAsync(result.SpanId);

        // Act
        var observations = await client.GetObservationListAsync(new ObservationListRequest { Page = 1, Limit = 50 });

        // Assert
        Assert.NotNull(observations);
        Assert.NotNull(observations.Data);
        Assert.True(observations.Data.Length >= 1);
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByType_Span()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        // Act
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            Type = "SPAN",
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(observations);
        Assert.NotNull(observations.Data);
        Assert.All(observations.Data, o => Assert.Equal("SPAN", o.Type));
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByType_Generation()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        // Act
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            Type = "GENERATION",
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(observations);
        Assert.NotNull(observations.Data);
        Assert.All(observations.Data, o => Assert.Equal("GENERATION", o.Type));
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByType_Event()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, eventId) = traceHelper.CreateTraceWithEvent();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(eventId);

        // Act
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            Type = "EVENT",
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(observations);
        Assert.NotNull(observations.Data);
        Assert.All(observations.Data, o => Assert.Equal("EVENT", o.Type));
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByTraceId()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var result = traceHelper.CreateComplexTrace();
        await traceHelper.WaitForTraceAsync(result.TraceId);
        await traceHelper.WaitForObservationAsync(result.SpanId);

        // Act
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            TraceId = result.TraceId,
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(observations);
        Assert.NotNull(observations.Data);
        Assert.True(observations.Data.Length >= 3); // span, generation, event
        Assert.All(observations.Data, o => Assert.Equal(result.TraceId, o.TraceId));
    }

    [Fact]
    public async Task GetObservationAsync_ReturnsObservation()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan(spanName: "test-span-retrieval");
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        // Act
        var observation = await client.GetObservationAsync(spanId);

        // Assert
        Assert.NotNull(observation);
        Assert.Equal(spanId, observation.Id);
        Assert.Equal("test-span-retrieval", observation.Name);
        Assert.Equal("SPAN", observation.Type);
    }

    [Fact]
    public async Task GetObservationAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetObservationAsync(nonExistentId));

        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task GetObservationAsync_ReturnsGenerationWithModel()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration(
            generationName: "llm-generation",
            model: "gpt-4-turbo"
        );
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        // Act
        var observation = await client.GetObservationAsync(generationId);

        // Assert
        Assert.NotNull(observation);
        Assert.Equal(generationId, observation.Id);
        Assert.Equal("llm-generation", observation.Name);
        Assert.Equal("GENERATION", observation.Type);
        Assert.Equal("gpt-4-turbo", observation.Model);
    }

    [Fact]
    public async Task GetObservationListAsync_FiltersByName()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var uniqueName = $"obs-name-{Guid.NewGuid():N}";

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan(spanName: uniqueName);
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        // Act
        var observations = await client.GetObservationListAsync(new ObservationListRequest
        {
            Name = uniqueName,
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(observations);
        Assert.NotNull(observations.Data);
        Assert.Contains(observations.Data, o => o.Name == uniqueName);
    }
}