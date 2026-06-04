using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.LlmConnection;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class LlmConnectionTests
{
    private readonly LangfuseTestFixture _fixture;

    public LlmConnectionTests(LangfuseTestFixture fixture)
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

    private static string NewProvider()
    {
        return $"prov-{Guid.NewGuid():N}"[..16];
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();

        var result = await client.GetLlmConnectionsAsync();

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Meta.ShouldNotBeNull();
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_CreatesConnection()
    {
        var client = CreateClient();
        var provider = NewProvider();

        var created = await client.UpsertLlmConnectionAsync(new UpsertLlmConnectionRequest
        {
            Provider = provider,
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-integration-test-secret-1234567890"
        });

        created.ShouldNotBeNull();
        created.Id.ShouldNotBeNullOrEmpty();
        created.Provider.ShouldBe(provider);
        created.Adapter.ShouldBe(LlmAdapter.OpenAi);

        var list = await client.GetLlmConnectionsAsync(1, 100);
        list.Data.ShouldContain(c => c.Id == created.Id && c.Provider == provider);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_MasksSecretKey()
    {
        var client = CreateClient();
        var secret = "sk-very-secret-value-abcdefghijklmnop";

        var created = await client.UpsertLlmConnectionAsync(new UpsertLlmConnectionRequest
        {
            Provider = NewProvider(),
            Adapter = LlmAdapter.Anthropic,
            SecretKey = secret
        });

        created.DisplaySecretKey.ShouldNotBeNullOrEmpty();
        created.DisplaySecretKey.ShouldNotBe(secret);
        created.DisplaySecretKey.ShouldNotContain("abcdefghijklmnop");
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_UpdatesExistingByProvider()
    {
        var client = CreateClient();
        var provider = NewProvider();

        var created = await client.UpsertLlmConnectionAsync(new UpsertLlmConnectionRequest
        {
            Provider = provider,
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-initial-secret-0000000000",
            WithDefaultModels = true,
            CustomModels = Array.Empty<string>()
        });

        var updated = await client.UpsertLlmConnectionAsync(new UpsertLlmConnectionRequest
        {
            Provider = provider,
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-updated-secret-1111111111",
            WithDefaultModels = false,
            CustomModels = new[] { "custom-model-1" }
        });

        updated.Id.ShouldBe(created.Id);
        updated.Provider.ShouldBe(provider);
        updated.WithDefaultModels.ShouldBeFalse();
        updated.CustomModels.ShouldContain("custom-model-1");
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_WithCustomModels()
    {
        var client = CreateClient();
        var provider = NewProvider();

        var created = await client.UpsertLlmConnectionAsync(new UpsertLlmConnectionRequest
        {
            Provider = provider,
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test-secret-2222222222",
            CustomModels = new[] { "model-a", "model-b" },
            WithDefaultModels = false
        });

        created.CustomModels.Length.ShouldBe(2);
        created.CustomModels.ShouldContain("model-a");
        created.CustomModels.ShouldContain("model-b");
        created.WithDefaultModels.ShouldBeFalse();
    }

    [Fact]
    public async Task DeleteLlmConnectionAsync_DeletesConnection()
    {
        var client = CreateClient();
        var provider = NewProvider();

        var created = await client.UpsertLlmConnectionAsync(new UpsertLlmConnectionRequest
        {
            Provider = provider,
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-to-delete-3333333333"
        });

        var deleted = await client.DeleteLlmConnectionAsync(created.Id);
        deleted.ShouldNotBeNull();
        deleted.Message.ShouldNotBeNullOrEmpty();

        var list = await client.GetLlmConnectionsAsync(1, 100);
        list.Data.ShouldNotContain(c => c.Id == created.Id);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_RespectsPagination()
    {
        var client = CreateClient();

        await client.UpsertLlmConnectionAsync(new UpsertLlmConnectionRequest
        {
            Provider = NewProvider(),
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-page-aaaa"
        });
        await client.UpsertLlmConnectionAsync(new UpsertLlmConnectionRequest
        {
            Provider = NewProvider(),
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-page-bbbb"
        });

        var page = await client.GetLlmConnectionsAsync(1, 1);

        page.Data.Length.ShouldBe(1);
        page.Meta.Limit.ShouldBe(1);
        page.Meta.Page.ShouldBe(1);
        page.Meta.TotalItems.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task DeleteLlmConnectionAsync_NullId_ThrowsArgumentException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentException>(async () =>
            await client.DeleteLlmConnectionAsync(null!));
    }
}