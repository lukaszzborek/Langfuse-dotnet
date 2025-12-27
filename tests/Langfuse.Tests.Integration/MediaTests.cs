using System.Security.Cryptography;
using System.Text;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Media;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class MediaTests
{
    private readonly LangfuseTestFixture _fixture;

    public MediaTests(LangfuseTestFixture fixture)
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

    private static string ComputeSha256Hash(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToBase64String(hash);
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_ReturnsPresignedUrl()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var fileContent = Encoding.UTF8.GetBytes("This is test file content for media upload");
        var sha256Hash = ComputeSha256Hash(fileContent);

        var request = new MediaUploadRequest
        {
            TraceId = traceId,
            ContentType = "text/plain",
            ContentLength = fileContent.Length,
            Sha256Hash = sha256Hash,
            Field = "input"
        };

        var response = await client.GetMediaUploadUrlAsync(request);

        response.ShouldNotBeNull();
        response.MediaId.ShouldNotBeNull();
        response.UploadUrl.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_WithObservation_ReturnsPresignedUrl()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        var fileContent = Encoding.UTF8.GetBytes("Test content for observation media");
        var sha256Hash = ComputeSha256Hash(fileContent);

        var request = new MediaUploadRequest
        {
            TraceId = traceId,
            ObservationId = generationId,
            ContentType = "text/plain",
            ContentLength = fileContent.Length,
            Sha256Hash = sha256Hash,
            Field = "output"
        };

        var response = await client.GetMediaUploadUrlAsync(request);

        response.ShouldNotBeNull();
        response.MediaId.ShouldNotBeNull();
        response.UploadUrl.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMediaAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetMediaAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_ForDifferentContentTypes()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var pngContent = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG magic bytes
        var sha256Hash = ComputeSha256Hash(pngContent);

        var request = new MediaUploadRequest
        {
            TraceId = traceId,
            ContentType = "image/png",
            ContentLength = pngContent.Length,
            Sha256Hash = sha256Hash,
            Field = "metadata"
        };

        var response = await client.GetMediaUploadUrlAsync(request);

        response.ShouldNotBeNull();
        response.MediaId.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_WithInputField()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var fileContent = Encoding.UTF8.GetBytes("Input field media content");
        var sha256Hash = ComputeSha256Hash(fileContent);

        var request = new MediaUploadRequest
        {
            TraceId = traceId,
            ContentType = "application/json",
            ContentLength = fileContent.Length,
            Sha256Hash = sha256Hash,
            Field = "input"
        };

        var response = await client.GetMediaUploadUrlAsync(request);

        response.ShouldNotBeNull();
        response.UploadUrl.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_WithOutputField()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var fileContent = Encoding.UTF8.GetBytes("Output field media content");
        var sha256Hash = ComputeSha256Hash(fileContent);

        var request = new MediaUploadRequest
        {
            TraceId = traceId,
            ContentType = "application/octet-stream",
            ContentLength = fileContent.Length,
            Sha256Hash = sha256Hash,
            Field = "output"
        };

        var response = await client.GetMediaUploadUrlAsync(request);

        response.ShouldNotBeNull();
        response.MediaId.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_WithMetadataField()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var fileContent = Encoding.UTF8.GetBytes("Metadata field media content");
        var sha256Hash = ComputeSha256Hash(fileContent);

        var request = new MediaUploadRequest
        {
            TraceId = traceId,
            ContentType = "text/csv",
            ContentLength = fileContent.Length,
            Sha256Hash = sha256Hash,
            Field = "metadata"
        };

        var response = await client.GetMediaUploadUrlAsync(request);

        response.ShouldNotBeNull();
        response.MediaId.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var fileContent = Encoding.UTF8.GetBytes("Comprehensive test content for media upload validation");
        var sha256Hash = ComputeSha256Hash(fileContent);

        var request = new MediaUploadRequest
        {
            TraceId = traceId,
            ContentType = "text/plain",
            ContentLength = fileContent.Length,
            Sha256Hash = sha256Hash,
            Field = "input"
        };

        var response = await client.GetMediaUploadUrlAsync(request);

        response.MediaId.ShouldNotBeNullOrEmpty();
        response.UploadUrl.ShouldNotBeNullOrEmpty();
        Uri.TryCreate(response.UploadUrl, UriKind.Absolute, out var parsedUri).ShouldBeTrue();
        parsedUri.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_WithObservation_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, generationId) = traceHelper.CreateTraceWithGeneration();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(generationId);

        var fileContent = Encoding.UTF8.GetBytes("Observation media content for comprehensive validation");
        var sha256Hash = ComputeSha256Hash(fileContent);

        var request = new MediaUploadRequest
        {
            TraceId = traceId,
            ObservationId = generationId,
            ContentType = "application/json",
            ContentLength = fileContent.Length,
            Sha256Hash = sha256Hash,
            Field = "output"
        };

        var response = await client.GetMediaUploadUrlAsync(request);

        response.MediaId.ShouldNotBeNullOrEmpty();
        response.UploadUrl.ShouldNotBeNullOrEmpty();
        Uri.TryCreate(response.UploadUrl, UriKind.Absolute, out var parsedUri).ShouldBeTrue();
        parsedUri.ShouldNotBeNull();
    }
}