using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.BlobStorageIntegration;
using zborek.Langfuse.Models.Core;

namespace Langfuse.Tests.Integration;

/// <summary>
///     Integration tests for the Blob Storage Integrations API.
///     These endpoints require an organization-scoped API key. The test fixture is provisioned with a
///     project-scoped key, so the server rejects every call with 403 Forbidden. We verify that contract plus
///     client-side argument validation. The success paths are covered by unit tests.
/// </summary>
[Collection(LangfuseTestCollection.Name)]
public class BlobStorageIntegrationTests
{
    private readonly LangfuseTestFixture _fixture;

    public BlobStorageIntegrationTests(LangfuseTestFixture fixture)
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
    public async Task GetBlobStorageIntegrationsAsync_WithProjectKey_Forbidden()
    {
        var client = CreateClient();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetBlobStorageIntegrationsAsync());

        exception.StatusCode.ShouldBe(403);
    }

    [Fact]
    public async Task GetBlobStorageIntegrationStatusAsync_WithProjectKey_Forbidden()
    {
        var client = CreateClient();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetBlobStorageIntegrationStatusAsync($"int-{Guid.NewGuid():N}"));

        exception.StatusCode.ShouldBe(403);
    }

    [Fact]
    public async Task UpsertBlobStorageIntegrationAsync_WithProjectKey_Forbidden()
    {
        var client = CreateClient();

        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = _fixture.ProjectId,
            Type = BlobStorageIntegrationType.S3,
            BucketName = "test-bucket",
            Region = "us-east-1",
            ExportFrequency = BlobStorageExportFrequency.Daily,
            Enabled = true,
            ForcePathStyle = false,
            FileType = BlobStorageIntegrationFileType.Json,
            ExportMode = BlobStorageExportMode.FullHistory
        };

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.UpsertBlobStorageIntegrationAsync(request));

        exception.StatusCode.ShouldBe(403);
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_WithProjectKey_Forbidden()
    {
        var client = CreateClient();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.DeleteBlobStorageIntegrationAsync($"int-{Guid.NewGuid():N}"));

        exception.StatusCode.ShouldBe(403);
    }

    [Fact]
    public async Task UpsertBlobStorageIntegrationAsync_NullRequest_ThrowsArgumentNullException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await client.UpsertBlobStorageIntegrationAsync(null!));
    }

    [Fact]
    public async Task GetBlobStorageIntegrationStatusAsync_NullId_ThrowsArgumentException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentException>(async () =>
            await client.GetBlobStorageIntegrationStatusAsync(null!));
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_NullId_ThrowsArgumentException()
    {
        var client = CreateClient();

        await Should.ThrowAsync<ArgumentException>(async () =>
            await client.DeleteBlobStorageIntegrationAsync(null!));
    }
}
