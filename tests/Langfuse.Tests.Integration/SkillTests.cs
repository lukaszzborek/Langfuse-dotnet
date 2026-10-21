using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Skill;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class SkillTests
{
    private readonly LangfuseTestFixture _fixture;

    public SkillTests(LangfuseTestFixture fixture)
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

    private static CreateSkillVersionRequest CreateRequest(string skillName, string body)
    {
        return new CreateSkillVersionRequest
        {
            Files =
            [
                new SkillVersionFileCreateInput
                {
                    Path = "SKILL.md",
                    Content = $"---\nname: {skillName}\ndescription: Integration test skill\n---\n\n{body}\n"
                }
            ],
            CommitMessage = "integration test"
        };
    }

    [Fact]
    public async Task SkillLifecycle_CreateGetLabelListDelete()
    {
        var client = CreateClient();
        var skillName = $"test-skill-{Guid.NewGuid():N}";

        var first = await client.CreateSkillVersionAsync(CreateRequest(skillName, "First version"));
        first.Name.ShouldBe(skillName);
        first.Files.ShouldContain(f => f.Path == "SKILL.md");

        var second = await client.CreateSkillVersionAsync(new CreateSkillVersionRequest
        {
            Files =
            [
                new SkillVersionFileCreateInput
                {
                    Path = "SKILL.md",
                    Sha256Hash = first.Files.Single(f => f.Path == "SKILL.md").Sha256Hash
                }
            ]
        });
        second.Version.ShouldBeGreaterThan(first.Version);

        var labeled = await client.UpdateSkillVersionLabelsAsync(skillName, second.Version,
            new UpdateSkillLabelsRequest { Labels = ["production"] });
        labeled.Labels.ShouldContain("production");

        var resolved = await client.GetSkillAsync(skillName, includeContent: true);
        resolved.Version.ShouldBe(second.Version);
        resolved.Files.Single(f => f.Path == "SKILL.md").Content.ShouldNotBeNull().ShouldContain("First version");

        var contents = await client.GetSkillFileContentsAsync([resolved.Files[0].Sha256Hash]);
        contents.Data.ShouldHaveSingleItem().Content.ShouldContain("First version");

        var tagged = await client.UpdateSkillAsync(skillName, new UpdateSkillRequest { Tags = ["integration-test"] });
        tagged.Tags.ShouldBe(["integration-test"]);

        var list = await client.GetSkillListAsync(new SkillListRequest { Name = skillName });
        var meta = list.Data.ShouldHaveSingleItem();
        meta.LatestVersion.ShouldBe(second.Version);
        meta.ProductionVersion.ShouldBe(second.Version);

        var deleted = await client.DeleteSkillVersionAsync(skillName, first.Version);
        deleted.Deleted.ShouldBeTrue();
    }
}
