using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
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
        // Arrange
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

        // Act
        var score = await client.CreateScoreAsync(request);

        // Assert
        Assert.NotNull(score);
        Assert.NotNull(score.Id);
        Assert.Equal(traceId, score.TraceId);
        Assert.Equal("quality", score.Name);
        Assert.Equal(0.85, score.Value);
    }

    [Fact]
    public async Task CreateScoreAsync_CreatesCategoricalScore()
    {
        // Arrange
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

        // Act
        var score = await client.CreateScoreAsync(request);

        // Assert
        Assert.NotNull(score);
        Assert.NotNull(score.Id);
        Assert.Equal("positive", score.StringValue);
    }

    [Fact]
    public async Task CreateScoreAsync_CreatesBooleanScore()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "is_accurate",
            Value = true,
            DataType = ScoreDataType.Boolean
        };

        // Act
        var score = await client.CreateScoreAsync(request);

        // Assert
        Assert.NotNull(score);
        Assert.NotNull(score.Id);
        Assert.Equal("is_accurate", score.Name);
    }

    [Fact]
    public async Task CreateScoreAsync_ScoresObservation()
    {
        // Arrange
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

        // Act
        var score = await client.CreateScoreAsync(request);

        // Assert
        Assert.NotNull(score);
        Assert.Equal(generationId, score.ObservationId);
    }

    [Fact]
    public async Task GetScoreAsync_ReturnsScore()
    {
        // Arrange
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

        // Act
        var score = await client.GetScoreAsync(createdScore.Id);

        // Assert
        Assert.NotNull(score);
        Assert.Equal(createdScore.Id, score.Id);
        Assert.Equal("retrieval-score", score.Name);
        Assert.Equal(0.75, score.Value);
    }

    [Fact]
    public async Task GetScoreAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetScoreAsync(nonExistentId));

        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task GetScoreListAsync_ReturnsPaginatedList()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        // Create multiple scores
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "score-1",
            Value = 0.5,
            DataType = ScoreDataType.Numeric
        });
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            TraceId = traceId,
            Name = "score-2",
            Value = 0.7,
            DataType = ScoreDataType.Numeric
        });

        // Act
        var result = await client.GetScoreListAsync(new ScoreListRequest { Page = 1, Limit = 50 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Length >= 2);
    }

    [Fact]
    public async Task GetScoreListAsync_FiltersByScoreName()
    {
        // Arrange
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

        // Act
        var result = await client.GetScoreListAsync(new ScoreListRequest
        {
            Name = scoreName,
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.All(result.Data, s => Assert.Equal(scoreName, s.Name));
    }

    [Fact]
    public async Task DeleteScoreAsync_DeletesScore()
    {
        // Arrange
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

        // Act
        await client.DeleteScoreAsync(score.Id);

        // Assert - Verify deletion
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetScoreAsync(score.Id));
        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task CreateScoreAsync_WithMetadata_IncludesMetadata()
    {
        // Arrange
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

        // Act
        var score = await client.CreateScoreAsync(request);

        // Assert
        Assert.NotNull(score);
        Assert.NotNull(score.Id);
    }

    [Fact]
    public async Task GetScoreListAsync_FiltersByName()
    {
        // Arrange
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

        // Act
        var result = await client.GetScoreListAsync(new ScoreListRequest
        {
            Name = scoreName,
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.All(result.Data, s => Assert.Equal(scoreName, s.Name));
    }
}