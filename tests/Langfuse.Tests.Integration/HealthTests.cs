using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class HealthTests
{
    private readonly LangfuseTestFixture _fixture;

    public HealthTests(LangfuseTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetHealthAsync_ReturnsHealthyStatus()
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

        await using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ILangfuseClient>();

        var health = await client.GetHealthAsync();

        health.ShouldNotBeNull();
        health.Status.ShouldBe("OK");
    }
}