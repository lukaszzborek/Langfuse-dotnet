using System.Text.Json;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.BlobStorageIntegration;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Models.Score;

namespace Langfuse.Tests.Integration;

/// <summary>
///     Integration tests that validate the model fixes work correctly with the actual Langfuse API.
///     These tests verify the new and fixed properties serialize/deserialize correctly.
/// </summary>
[Collection(LangfuseTestCollection.Name)]
public class ModelValidationIntegrationTests
{
    private readonly LangfuseTestFixture _fixture;

    public ModelValidationIntegrationTests(LangfuseTestFixture fixture)
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

    #region BlobStorageIntegration Tests

    [Fact]
    public void BlobStorageIntegrationResponse_PrefixIsRequired()
    {
        // This test validates that when we receive a response, the Prefix property
        // is correctly deserialized as a required field.
        // Since we don't have access to the blob storage API without proper auth,
        // we just verify the model structure is correct.

        var json = @"{
            ""data"": [
                {
                    ""id"": ""int-1"",
                    ""projectId"": ""proj-1"",
                    ""type"": ""S3"",
                    ""bucketName"": ""bucket"",
                    ""region"": ""us-east-1"",
                    ""prefix"": ""exports/"",
                    ""exportFrequency"": ""daily"",
                    ""enabled"": true,
                    ""forcePathStyle"": false,
                    ""fileType"": ""JSON"",
                    ""exportMode"": ""FULL_HISTORY"",
                    ""compressed"": true,
                    ""exportSource"": ""OBSERVATIONS_V2"",
                    ""createdAt"": ""2024-01-01T00:00:00Z"",
                    ""updatedAt"": ""2024-01-01T00:00:00Z""
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationsResponse>(json);

        response.ShouldNotBeNull();
        response.Data.ShouldNotBeNull();
        response.Data[0].Prefix.ShouldBe("exports/");
    }

    #endregion

    #region Score Model Tests

    [Fact]
    public async Task CreateScoreAsync_WithEnvironment_SerializesCorrectly()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new ScoreCreateRequest
        {
            Id = Guid.NewGuid().ToString(),
            TraceId = traceId,
            Name = $"env-test-{Guid.NewGuid():N}"[..20],
            Value = 0.85,
            DataType = ScoreDataType.Numeric,
            Environment = "production"
        };

        await client.CreateScoreAsync(request);

        var fetchedScore = await traceHelper.WaitForScoreAsync(request.Id);

        fetchedScore.Name.ShouldBe(request.Name);
        fetchedScore.Environment.ShouldBe("production");
    }

    [Fact]
    public async Task GetScoresV3Async_FiltersByDataType()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var scoreId = Guid.NewGuid().ToString();
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = $"numeric-{Guid.NewGuid():N}"[..20],
            Value = 0.75,
            DataType = ScoreDataType.Numeric
        });

        await traceHelper.WaitForScoreAsync(scoreId);

        var result = await client.GetScoresV3Async(new ScoreV3ListRequest
        {
            TraceId = traceId,
            DataType = "NUMERIC"
        });

        result.Data.ShouldNotBeEmpty();
        result.Data.ShouldAllBe(s => s.DataType == ScoreV3DataType.Numeric);
    }

    [Fact]
    public async Task GetScoresV3Async_FiltersBySource()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        // Scores created through the public API get source API
        var scoreId = Guid.NewGuid().ToString();
        await client.CreateScoreAsync(new ScoreCreateRequest
        {
            Id = scoreId,
            TraceId = traceId,
            Name = $"source-test-{Guid.NewGuid():N}"[..20],
            Value = 0.9,
            DataType = ScoreDataType.Numeric
        });

        await traceHelper.WaitForScoreAsync(scoreId);

        var result = await client.GetScoresV3Async(new ScoreV3ListRequest
        {
            TraceId = traceId,
            Source = "API"
        });

        result.Data.ShouldNotBeEmpty();
        result.Data.ShouldAllBe(s => s.Source == ScoreSource.Api);
    }

    #endregion

    #region Dataset Model Tests

    [Fact]
    public async Task CreateDatasetAsync_WithInputSchema_SerializesCorrectly()
    {
        var client = CreateClient();
        var datasetName = $"schema-test-{Guid.NewGuid():N}";

        var inputSchema = new
        {
            type = "object",
            properties = new
            {
                query = new { type = "string" },
                context = new { type = "array", items = new { type = "string" } }
            },
            required = new[] { "query" }
        };

        var request = new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Dataset with input schema",
            InputSchema = inputSchema
        };

        var dataset = await client.CreateDatasetAsync(request);

        dataset.ShouldNotBeNull();
        dataset.Name.ShouldBe(datasetName);
    }

    [Fact]
    public async Task CreateDatasetAsync_WithExpectedOutputSchema_SerializesCorrectly()
    {
        var client = CreateClient();
        var datasetName = $"output-schema-{Guid.NewGuid():N}";

        var expectedOutputSchema = new
        {
            type = "object",
            properties = new
            {
                response = new { type = "string" },
                confidence = new { type = "number", minimum = 0, maximum = 1 }
            }
        };

        var request = new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Dataset with expected output schema",
            ExpectedOutputSchema = expectedOutputSchema
        };

        var dataset = await client.CreateDatasetAsync(request);

        dataset.ShouldNotBeNull();
        dataset.Name.ShouldBe(datasetName);
    }

    [Fact]
    public async Task CreateDatasetAsync_WithBothSchemas_SerializesCorrectly()
    {
        var client = CreateClient();
        var datasetName = $"both-schemas-{Guid.NewGuid():N}";

        var inputSchema = new { type = "object", properties = new { input = new { type = "string" } } };
        var outputSchema = new { type = "object", properties = new { output = new { type = "string" } } };

        var request = new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Dataset with both schemas",
            InputSchema = inputSchema,
            ExpectedOutputSchema = outputSchema
        };

        var dataset = await client.CreateDatasetAsync(request);

        dataset.ShouldNotBeNull();
        dataset.Name.ShouldBe(datasetName);
        dataset.Description.ShouldBe("Dataset with both schemas");
    }

    [Fact]
    public async Task DatasetModel_HasExpectedNewProperties()
    {
        var client = CreateClient();
        var datasetName = $"props-dataset-{Guid.NewGuid():N}";

        await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Dataset for property validation"
        });

        var dataset = await client.GetDatasetAsync(datasetName);

        dataset.ShouldNotBeNull();
        // Verify new properties exist (may be null but shouldn't throw)
        _ = dataset.InputSchema;
        _ = dataset.ExpectedOutputSchema;
    }

    #endregion

    #region Score Config Tests

    [Fact]
    public async Task GetScoreConfigListAsync_UsesPageParameter_NotOffset()
    {
        var client = CreateClient();

        // This tests that the ScoreConfigListRequest uses Page (not Offset)
        var result = await client.GetScoreConfigListAsync(new ScoreConfigListRequest
        {
            Page = 1,
            Limit = 10
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateScoreConfigAsync_WithScoreConfigDataType_Works()
    {
        var client = CreateClient();
        var configName = $"config-{Guid.NewGuid():N}"[..20];

        var request = new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreConfigDataType.Numeric,
            MinValue = 0,
            MaxValue = 1,
            Description = "Test score configuration"
        };

        var config = await client.CreateScoreConfigAsync(request);

        config.ShouldNotBeNull();
        config.Name.ShouldBe(configName);
        config.DataType.ShouldBe(ScoreConfigDataType.Numeric);
    }

    [Fact]
    public async Task CreateScoreConfigAsync_CategoricalType_Works()
    {
        var client = CreateClient();
        var configName = $"cat-config-{Guid.NewGuid():N}"[..20];

        var request = new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreConfigDataType.Categorical,
            Categories = new[]
            {
                new ConfigCategory { Label = "Good", Value = 1 },
                new ConfigCategory { Label = "Bad", Value = 0 }
            },
            Description = "Categorical score configuration"
        };

        var config = await client.CreateScoreConfigAsync(request);

        config.ShouldNotBeNull();
        config.Name.ShouldBe(configName);
        config.DataType.ShouldBe(ScoreConfigDataType.Categorical);
    }

    #endregion
}