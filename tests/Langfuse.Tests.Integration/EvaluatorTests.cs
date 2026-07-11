using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Evaluation;

namespace Langfuse.Tests.Integration;

/// <summary>
///     Integration tests for the unstable Evaluators API.
///     Note: creating a runnable evaluator requires a working LLM connection because the create endpoint
///     preflights the resolved model configuration (422 evaluator_preflight_failed). The test project has no
///     live LLM credentials, so the create-success path is covered by unit tests; here we cover the read paths
///     and the preflight-failure contract.
/// </summary>
[Collection(LangfuseTestCollection.Name)]
public class EvaluatorTests
{
    private readonly LangfuseTestFixture _fixture;

    public EvaluatorTests(LangfuseTestFixture fixture)
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

    private static EvaluatorOutputDefinition NumericOutput()
    {
        return new EvaluatorOutputDefinition
        {
            DataType = EvaluatorOutputDataType.Numeric,
            Reasoning = new EvaluatorOutputFieldDefinition { Description = "Explain the score" },
            Score = new EvaluatorOutputScoreDefinition { Description = "Score between 0 and 1" }
        };
    }

    [Fact]
    public async Task GetEvaluatorsAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();

        var result = await client.GetEvaluatorsAsync();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Meta.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetEvaluatorsAsync_RespectsPagination()
    {
        var client = CreateClient();

        var result = await client.GetEvaluatorsAsync(1, 5);

        result.ShouldNotBeNull();
        result.Meta.Page.ShouldBe(1);
        result.Meta.Limit.ShouldBe(5);
    }

    [Fact]
    public async Task GetEvaluatorAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetEvaluatorAsync($"evaltmpl-{Guid.NewGuid():N}"));
    }

    [Fact]
    public async Task GetEvaluatorAsync_NullId_ThrowsArgumentException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentException>(async () =>
            await client.GetEvaluatorAsync(null!));
    }

    [Fact]
    public async Task CreateEvaluatorAsync_WithoutResolvableModel_FailsPreflight()
    {
        var client = CreateClient();

        var request = new CreateLlmAsJudgeEvaluatorRequest
        {
            Name = $"eval-{Guid.NewGuid():N}"[..16],
            Prompt = "Rate the helpfulness of {{input}} given {{output}}.",
            OutputDefinition = NumericOutput()
        };

        // No project default evaluation model and no explicit modelConfig => preflight cannot resolve a model.
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.CreateEvaluatorAsync(request));

        exception.StatusCode.ShouldBe(422);
    }

    [Fact]
    public async Task CreateEvaluatorAsync_NullRequest_ThrowsArgumentNullException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await client.CreateEvaluatorAsync(null!));
    }
}