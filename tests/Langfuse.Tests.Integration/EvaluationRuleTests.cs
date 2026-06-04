using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Evaluation;

namespace Langfuse.Tests.Integration;

/// <summary>
///     Integration tests for the unstable Evaluation Rules API.
///     Note: creating an evaluation rule requires an existing evaluator, and creating a runnable evaluator
///     requires a working LLM connection (the evaluator create endpoint preflights the model). The test
///     project has no live LLM credentials, so the create-success path is covered by unit tests; here we cover
///     the read paths and failure contracts (missing evaluator, not-found ids).
/// </summary>
[Collection(LangfuseTestCollection.Name)]
public class EvaluationRuleTests
{
    private readonly LangfuseTestFixture _fixture;

    public EvaluationRuleTests(LangfuseTestFixture fixture)
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

    [Fact]
    public async Task GetEvaluationRulesAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();

        var result = await client.GetEvaluationRulesAsync();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Meta.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetEvaluationRulesAsync_RespectsPagination()
    {
        var client = CreateClient();

        var result = await client.GetEvaluationRulesAsync(1, 5);

        result.ShouldNotBeNull();
        result.Meta.Page.ShouldBe(1);
        result.Meta.Limit.ShouldBe(5);
    }

    [Fact]
    public async Task GetEvaluationRuleAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetEvaluationRuleAsync($"rule-{Guid.NewGuid():N}"));
    }

    [Fact]
    public async Task GetEvaluationRuleAsync_NullId_ThrowsArgumentException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentException>(async () =>
            await client.GetEvaluationRuleAsync(null!));
    }

    [Fact]
    public async Task CreateEvaluationRuleAsync_WithNonexistentEvaluator_ThrowsException()
    {
        var client = CreateClient();

        var request = new CreateEvaluationRuleRequest
        {
            Name = $"rule-{Guid.NewGuid():N}"[..16],
            Evaluator = new EvaluationRuleEvaluatorReference
            {
                Name = $"missing-{Guid.NewGuid():N}",
                Scope = EvaluatorScope.Project
            },
            Target = EvaluationRuleTarget.Observation,
            Enabled = false,
            Mapping = new[]
            {
                new EvaluationRuleMapping { Variable = "input", Source = EvaluationRuleMappingSource.Input }
            }
        };

        await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.CreateEvaluationRuleAsync(request));
    }

    [Fact]
    public async Task CreateEvaluationRuleAsync_NullRequest_ThrowsArgumentNullException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await client.CreateEvaluationRuleAsync(null!));
    }

    [Fact]
    public async Task UpdateEvaluationRuleAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.UpdateEvaluationRuleAsync(
                $"rule-{Guid.NewGuid():N}",
                new UpdateEvaluationRuleRequest { Enabled = false }));
    }

    [Fact]
    public async Task DeleteEvaluationRuleAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.DeleteEvaluationRuleAsync($"rule-{Guid.NewGuid():N}"));
    }
}