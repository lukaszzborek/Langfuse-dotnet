using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Evaluation;

namespace Langfuse.Tests.Integration;

/// <summary>
///     Integration tests for the Evaluation Rules API (v2). Uses a code evaluator (no LLM connection needed)
///     to exercise the rule lifecycle, plus failure contracts (missing evaluator, not-found ids).
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
    public async Task GetEvaluationRulesAsync_RespectsLimit()
    {
        var client = CreateClient();

        var result = await client.GetEvaluationRulesAsync(5);

        result.ShouldNotBeNull();
        result.Data.Length.ShouldBeLessThanOrEqualTo(5);
    }

    [Fact]
    public async Task EvaluationRule_Lifecycle_WithCodeEvaluator()
    {
        var client = CreateClient();

        var evaluator = await client.CreateEvaluatorAsync(new CreateCodeEvaluatorRequest
        {
            Name = $"eval-{Guid.NewGuid():N}"[..16],
            SourceCode =
                "export function evaluate(ctx) { return { scores: [{ name: \"length\", value: String(ctx.output ?? \"\").length, dataType: \"NUMERIC\" }] }; }",
            SourceCodeLanguage = CodeEvaluatorSourceCodeLanguage.Typescript
        });

        try
        {
            var rule = await client.CreateEvaluationRuleAsync(new CreateEvaluationRuleRequest
            {
                Name = $"rule-{Guid.NewGuid():N}"[..16],
                Enabled = false,
                Sampling = 0.5,
                Filter = new[]
                {
                    new StringOptionsEvaluationRuleFilter
                    {
                        Column = "type",
                        Operator = EvaluationRuleOptionsFilterOperator.AnyOf,
                        Value = new[] { "GENERATION" }
                    }
                },
                EvaluatorAssignments = new[]
                {
                    new EvaluationRuleEvaluatorAssignmentInput { EvaluatorId = evaluator.Id }
                }
            });

            try
            {
                rule.Enabled.ShouldBeFalse();
                rule.Sampling.ShouldBe(0.5);
                rule.Filter.Length.ShouldBe(1);
                rule.EvaluatorAssignments.Single().EvaluatorId.ShouldBe(evaluator.Id);

                var updated = await client.UpdateEvaluationRuleAsync(rule.Id,
                    new UpdateEvaluationRuleRequest { Name = "renamed" });
                updated.Name.ShouldBe("renamed");

                var fetched = await client.GetEvaluationRuleAsync(rule.Id);
                fetched.Id.ShouldBe(rule.Id);

                var evaluatorWithRules = await client.GetEvaluatorAsync(evaluator.Id);
                evaluatorWithRules.EvaluationRuleAssignments.ShouldContain(a => a.EvaluationRuleId == rule.Id);
            }
            finally
            {
                var deleted = await client.DeleteEvaluationRuleAsync(rule.Id);
                deleted.Id.ShouldBe(rule.Id);
            }
        }
        finally
        {
            await client.DeleteEvaluatorAsync(evaluator.Id);
        }
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
            Enabled = false,
            EvaluatorAssignments = new[]
            {
                new EvaluationRuleEvaluatorAssignmentInput { EvaluatorId = $"missing-{Guid.NewGuid():N}" }
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