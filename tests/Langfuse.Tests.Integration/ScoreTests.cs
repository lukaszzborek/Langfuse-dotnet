using System.Text.Json;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class ScoreTests
{
    private readonly LangfuseTestFixture _fixture;

    public ScoreTests(LangfuseTestFixture fixture)
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
    public async Task CreateScoreAsync_CreatesNumericScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "quality",
            Value = 0.85,
            DataType = ScoreDataType.Numeric,
            Comment = "Good quality response"
        };

        var score = await client.CreateScoreAsync(request);

        score.ShouldNotBeNull();
        score.Id.ShouldNotBeNull();

        var fetchedScore = await traceHelper.WaitForScoreAsync(score.Id);
        fetchedScore.TraceId.ShouldBe(traceId);
        fetchedScore.Name.ShouldBe("quality");
        var actualValue = fetchedScore.Value is JsonElement je ? je.GetDouble() : Convert.ToDouble(fetchedScore.Value);
        actualValue.ShouldBe(0.85, 0.01);
    }

    [Fact]
    public async Task CreateScoreAsync_CreatesCategoricalScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "sentiment",
            Value = "positive",
            DataType = ScoreDataType.Categorical
        };

        var score = await client.CreateScoreAsync(request);

        score.ShouldNotBeNull();
        score.Id.ShouldNotBeNull();

        var fetchedScore = await traceHelper.WaitForScoreAsync(score.Id);
        fetchedScore.StringValue.ShouldBe("positive");
    }

    [Fact]
    public async Task CreateScoreAsync_CreatesBooleanScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "is_accurate",
            Value = 1,
            DataType = ScoreDataType.Boolean
        };

        var score = await client.CreateScoreAsync(request);

        score.ShouldNotBeNull();
        score.Id.ShouldNotBeNull();

        var fetchedScore = await traceHelper.WaitForScoreAsync(score.Id);
        fetchedScore.Name.ShouldBe("is_accurate");
    }

    [Fact]
    public async Task CreateScoreAsync_ScoresObservation()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        var request = new ScoreCreateRequest
        {
            TraceId = traceId,
            ObservationId = generationId,
            Name = "generation_quality",
            Value = 0.9,
            DataType = ScoreDataType.Numeric
        };

        var score = await client.CreateScoreAsync(request);

        score.ShouldNotBeNull();
        score.Id.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetScoreAsync_ReturnsScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createdScore = await client.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "retrieval-score",
            Value = 0.75,
            DataType = ScoreDataType.Numeric
        });

        var score = await traceHelper.WaitForScoreAsync(createdScore.Id);

        score.ShouldNotBeNull();
        score.Id.ShouldBe(createdScore.Id);
        score.Name.ShouldBe("retrieval-score");
        var actualValue = score.Value is JsonElement je ? je.GetDouble() : Convert.ToDouble(score.Value);
        actualValue.ShouldBe(0.75, 0.01);
    }

    [Fact]
    public async Task GetScoreAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetScoreAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetScoreListAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var score1 = await client.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "score-1",
            Value = 0.5,
            DataType = ScoreDataType.Numeric
        });
        var score2 = await client.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "score-2",
            Value = 0.7,
            DataType = ScoreDataType.Numeric
        });

        await traceHelper.WaitForScoreAsync(score1.Id);
        await traceHelper.WaitForScoreAsync(score2.Id);

        var result = await client.GetScoreListAsync(new ScoreListRequest { Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        (result.Data.Length >= 2).ShouldBeTrue();
    }

    [Fact]
    public async Task GetScoreListAsync_FiltersByScoreName()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var scoreName = $"filtered-score-{Guid.NewGuid():N}"[..20];
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = scoreName,
            Value = 0.8,
            DataType = ScoreDataType.Numeric
        });

        var result = await client.GetScoreListAsync(new ScoreListRequest
        {
            Name = scoreName,
            Page = 1,
            Limit = 50
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldAllBe(s => s.Name == scoreName);
    }

    [Fact]
    public async Task DeleteScoreAsync_DeletesScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var score = await client.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "score-to-delete",
            Value = 0.5,
            DataType = ScoreDataType.Numeric
        });

        await client.DeleteScoreAsync(score.Id);

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetScoreAsync(score.Id));
        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreateScoreAsync_WithMetadata_IncludesMetadata()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "score-with-metadata",
            Value = 0.9,
            DataType = ScoreDataType.Numeric,
            Metadata = new { evaluator = "human", confidence = 0.95 }
        };

        var score = await client.CreateScoreAsync(request);

        score.ShouldNotBeNull();
        score.Id.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetScoreListAsync_FiltersByName()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var scoreName = $"unique-{Guid.NewGuid():N}"[..20];
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = scoreName,
            Value = 0.6,
            DataType = ScoreDataType.Numeric
        });

        var result = await client.GetScoreListAsync(new ScoreListRequest
        {
            Name = scoreName,
            Page = 1,
            Limit = 50
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldAllBe(s => s.Name == scoreName);
    }
}