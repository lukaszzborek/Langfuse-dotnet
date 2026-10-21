using System.Diagnostics;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
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
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ILangfuseClient>();
    }

    private static CancellationToken Token => TestContext.Current.CancellationToken;

    private TraceTestHelper CreateTraceHelper(ILangfuseClient client)
    {
        return new TraceTestHelper(client, _fixture);
    }

    // Score creation is asynchronous and the response carries no score id,
    // so every test sets the id upfront to look the score up afterwards.

    [Fact]
    public async Task CreateScoreAsync_CreatesNumericScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        var result = await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = "quality",
            Value = 0.85,
            DataType = ScoreDataType.Numeric,
            Comment = "Good quality response"
        }, Token);

        result.Message.ShouldBe("Accepted");

        var score = (await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token))
            .ShouldBeOfType<NumericScoreV3>();
        score.Name.ShouldBe("quality");
        score.Value.ShouldBe(0.85, 0.01);
        score.Subject.ShouldBeOfType<ScoreSubjectTraceV3>().Id.ShouldBe(traceId);
    }

    [Fact]
    public async Task CreateScoreAsync_CreatesCategoricalScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = "sentiment",
            Value = "positive",
            DataType = ScoreDataType.Categorical
        }, Token);

        var score = (await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token))
            .ShouldBeOfType<CategoricalScoreV3>();
        score.Value.ShouldBe("positive");
    }

    [Fact]
    public async Task CreateScoreAsync_CreatesBooleanScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = "is_accurate",
            Value = 1,
            DataType = ScoreDataType.Boolean
        }, Token);

        var score = (await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token))
            .ShouldBeOfType<BooleanScoreV3>();
        score.Name.ShouldBe("is_accurate");
        score.Value.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateScoreAsync_ScoresObservation()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);
        await traceHelper.WaitForObservationAsync(generationId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            ObservationId = generationId,
            Name = "generation_quality",
            Value = 0.9,
            DataType = ScoreDataType.Numeric
        }, Token);

        var score = await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token);
        var subject = score.Subject.ShouldBeOfType<ScoreSubjectObservationV3>();
        subject.Id.ShouldBe(generationId);
        subject.TraceId.ShouldBe(traceId);
    }

    [Fact]
    public async Task CreateScoreAsync_WithMetadata_IncludesMetadata()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = "score-with-metadata",
            Value = 0.9,
            DataType = ScoreDataType.Numeric,
            Metadata = new { evaluator = "human", confidence = 0.95 }
        }, Token);

        var score = await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token);
        score.Metadata.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateScoreAsync_SameIdAndTimestamp_UpdatesScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        var timestamp = DateTimeOffset.UtcNow;

        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = "upsert-score",
            Value = 0.1,
            DataType = ScoreDataType.Numeric,
            Timestamp = timestamp
        }, Token);
        await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token);

        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = "upsert-score",
            Value = 0.9,
            DataType = ScoreDataType.Numeric,
            Timestamp = timestamp
        }, Token);

        var stopwatch = Stopwatch.StartNew();
        NumericScoreV3? score = null;
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(30))
        {
            score = (await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token))
                .ShouldBeOfType<NumericScoreV3>();
            if (Math.Abs(score.Value - 0.9) < 0.01)
            {
                break;
            }

            await Task.Delay(500, Token);
        }

        score.ShouldNotBeNull();
        score.Value.ShouldBe(0.9, 0.01);

        var scores = await client.GetScoresV3Async(new ScoreV3ListRequest { Id = scoreId }, Token);
        scores.Data.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task GetScoresV3Async_FiltersByName()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreName = $"unique-{Guid.NewGuid():N}"[..20];
        var scoreId = Guid.NewGuid().ToString();
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = scoreName,
            Value = 0.6,
            DataType = ScoreDataType.Numeric
        }, Token);
        await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token);

        var result = await client.GetScoresV3Async(new ScoreV3ListRequest { Name = scoreName }, Token);

        result.Data.ShouldNotBeEmpty();
        result.Data.ShouldAllBe(s => s.Name == scoreName);
    }

    [Fact]
    public async Task DeleteScoreAsync_DeletesScore()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = "score-to-delete",
            Value = 0.5,
            DataType = ScoreDataType.Numeric
        }, Token);
        await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token);

        await client.DeleteScoreAsync(scoreId, Token);

        var stopwatch = Stopwatch.StartNew();
        var deleted = false;
        while (stopwatch.Elapsed < TimeSpan.FromSeconds(30))
        {
            var result = await client.GetScoresV3Async(new ScoreV3ListRequest { Id = scoreId }, Token);
            if (result.Data.Length == 0)
            {
                deleted = true;
                break;
            }

            await Task.Delay(500, Token);
        }

        deleted.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateScoreAsync_NumericScore_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        var scoreName = $"comprehensive-score-{Guid.NewGuid():N}"[..20];
        const string comment = "Comprehensive test score comment";

        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = scoreName,
            Value = 0.85,
            DataType = ScoreDataType.Numeric,
            Comment = comment,
            Metadata = new { evaluator = "integration-test", confidence = 0.95 }
        }, Token);

        var score = (await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token))
            .ShouldBeOfType<NumericScoreV3>();

        score.Id.ShouldBe(scoreId);
        score.ProjectId.ShouldNotBeNullOrEmpty();
        score.Name.ShouldBe(scoreName);
        score.Value.ShouldBe(0.85, 0.01);
        score.DataType.ShouldBe(ScoreV3DataType.Numeric);
        score.Source.ShouldBe(ScoreSource.Api);
        score.Comment.ShouldBe(comment);
        score.Environment.ShouldNotBeNull();
        score.Timestamp.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        score.CreatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        score.UpdatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        score.Subject.ShouldBeOfType<ScoreSubjectTraceV3>().Id.ShouldBe(traceId);
    }

    [Fact]
    public async Task CreateScoreAsync_CategoricalScore_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var scoreId = Guid.NewGuid().ToString();
        var scoreName = $"cat-score-{Guid.NewGuid():N}"[..20];

        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = scoreName,
            Value = "positive",
            DataType = ScoreDataType.Categorical
        }, Token);

        var score = (await traceHelper.WaitForScoreAsync(scoreId, cancellationToken: Token))
            .ShouldBeOfType<CategoricalScoreV3>();

        score.Name.ShouldBe(scoreName);
        score.Value.ShouldBe("positive");
        score.DataType.ShouldBe(ScoreV3DataType.Categorical);
        score.Timestamp.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        score.CreatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        score.Subject.ShouldBeOfType<ScoreSubjectTraceV3>().Id.ShouldBe(traceId);
    }

    [Fact]
    public async Task CreateScoresAsync_CreatesAllScoresInBatch()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var numericId = Guid.NewGuid().ToString();
        var categoricalId = Guid.NewGuid().ToString();
        var booleanId = Guid.NewGuid().ToString();

        var result = await client.CreateScoresAsync([
            new ScoreCreateRequest
            {
                Id = numericId, TraceId = traceId, Name = "batch-quality", Value = 0.75,
                DataType = ScoreDataType.Numeric
            },
            new ScoreCreateRequest
            {
                Id = categoricalId, TraceId = traceId, Name = "batch-sentiment", Value = "positive",
                DataType = ScoreDataType.Categorical
            },
            new ScoreCreateRequest
            {
                Id = booleanId, TraceId = traceId, Name = "batch-accurate", Value = 1,
                DataType = ScoreDataType.Boolean
            }
        ], Token);

        result.Message.ShouldBe("Accepted");

        var scores = await traceHelper.WaitForTraceScoresV3Async(traceId, 3, cancellationToken: Token);
        scores.Length.ShouldBe(3);

        var numeric = scores.Single(s => s.Id == numericId).ShouldBeOfType<NumericScoreV3>();
        numeric.Name.ShouldBe("batch-quality");
        numeric.Value.ShouldBe(0.75, 0.01);

        var categorical = scores.Single(s => s.Id == categoricalId).ShouldBeOfType<CategoricalScoreV3>();
        categorical.Name.ShouldBe("batch-sentiment");
        categorical.Value.ShouldBe("positive");

        var boolean = scores.Single(s => s.Id == booleanId).ShouldBeOfType<BooleanScoreV3>();
        boolean.Name.ShouldBe("batch-accurate");
        boolean.Value.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateScoresAsync_PartiallyInvalidBatch_CreatesOnlyValidScores()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: Token);

        var validId = Guid.NewGuid().ToString();
        var invalidId = Guid.NewGuid().ToString();

        var result = await client.CreateScoresAsync([
            new ScoreCreateRequest
            {
                Id = validId, TraceId = traceId, Name = "batch-valid", Value = 0.5,
                DataType = ScoreDataType.Numeric
            },
            new ScoreCreateRequest
            {
                Id = invalidId, TraceId = traceId, Name = "batch-invalid", Value = 5,
                DataType = ScoreDataType.Boolean
            }
        ], Token);

        result.Accepted.ShouldBe(1);
        result.Rejected.ShouldBe(1);
        result.Errors.ShouldHaveSingleItem().Message.ShouldNotBeNullOrEmpty();

        var scores = await traceHelper.WaitForTraceScoresV3Async(traceId, 1, cancellationToken: Token);

        var valid = scores.ShouldHaveSingleItem().ShouldBeOfType<NumericScoreV3>();
        valid.Id.ShouldBe(validId);
        valid.Name.ShouldBe("batch-valid");
        valid.Value.ShouldBe(0.5, 0.01);
    }
}
