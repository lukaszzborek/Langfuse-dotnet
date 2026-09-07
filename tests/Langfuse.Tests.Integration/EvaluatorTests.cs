using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Evaluation;

namespace Langfuse.Tests.Integration;

/// <summary>
///     Integration tests for the Evaluators API (v2). Uses a code evaluator (no LLM connection needed)
///     to exercise the create/update/versions/delete lifecycle, plus failure contracts (LLM-as-a-judge without a
///     resolvable model, not-found ids).
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

    private static NumericEvaluatorOutputDefinition NumericOutput()
    {
        return new NumericEvaluatorOutputDefinition
        {
            ScoreReasoningInstructions = "Explain the score",
            ScoreValueInstructions = "Score between 0 and 1",
            MinValue = 0,
            MaxValue = 1
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
    public async Task GetEvaluatorsAsync_RespectsLimit()
    {
        var client = CreateClient();

        var result = await client.GetEvaluatorsAsync(5);

        result.ShouldNotBeNull();
        result.Data.Length.ShouldBeLessThanOrEqualTo(5);
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
    public async Task CreateEvaluatorAsync_Code_CreatesUpdatesListsVersionsAndDeletes()
    {
        var client = CreateClient();

        var created = await client.CreateEvaluatorAsync(new CreateCodeEvaluatorRequest
        {
            Name = $"eval-{Guid.NewGuid():N}"[..16],
            SourceCode =
                "export function evaluate(ctx) { return { scores: [{ name: \"length\", value: String(ctx.output ?? \"\").length, dataType: \"NUMERIC\" }] }; }",
            SourceCodeLanguage = CodeEvaluatorSourceCodeLanguage.Typescript
        });

        try
        {
            var codeEvaluator = created.ShouldBeOfType<CodeEvaluator>();
            codeEvaluator.Version.ShouldBe(1);

            var updated = await client.UpdateEvaluatorAsync(created.Id,
                new UpdateEvaluatorMetadataRequest { Description = "updated" });
            updated.Description.ShouldBe("updated");
            updated.Version.ShouldBe(1);

            var fetched = await client.GetEvaluatorAsync(created.Id);
            fetched.Id.ShouldBe(created.Id);

            var versions = await client.GetEvaluatorVersionsAsync(created.Id);
            versions.Data.ShouldContain(v => v.Version == 1);
        }
        finally
        {
            var deleted = await client.DeleteEvaluatorAsync(created.Id);
            deleted.Id.ShouldBe(created.Id);
        }
    }

    [Fact]
    public async Task CreateEvaluatorAsync_LlmAsJudge_WithoutResolvableModel_IsPausedOrRejected()
    {
        var client = CreateClient();

        var request = new CreateLlmAsJudgeEvaluatorRequest
        {
            Name = $"eval-{Guid.NewGuid():N}"[..16],
            Prompt = "Rate the helpfulness of {{input}} given {{output}}.",
            OutputDefinition = NumericOutput()
        };

        // No project default evaluation model and no explicit modelConfig: the API either rejects the
        // request or returns an evaluator paused by a missing model configuration.
        try
        {
            var created = await client.CreateEvaluatorAsync(request);
            try
            {
                created.Status.ShouldBe(EvaluatorStatus.Paused);
            }
            finally
            {
                await client.DeleteEvaluatorAsync(created.Id);
            }
        }
        catch (LangfuseApiException exception)
        {
            exception.StatusCode.ShouldBeOneOf(400, 412, 422);
        }
    }

    [Fact]
    public async Task CreateEvaluatorAsync_NullRequest_ThrowsArgumentNullException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await client.CreateEvaluatorAsync(null!));
    }
}