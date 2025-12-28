using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using zborek.Langfuse.Client;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models.BlobStorageIntegration;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientBlobStorageIntegrationsTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientBlobStorageIntegrationsTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    [Fact]
    public async Task GetBlobStorageIntegrationsAsync_Success_ReturnsIntegrations()
    {
        // Arrange
        var expectedResponse = new BlobStorageIntegrationsResponse
        {
            Data = new[]
            {
                new BlobStorageIntegrationResponse
                {
                    Id = "int-1",
                    ProjectId = "proj-1",
                    Type = BlobStorageIntegrationType.S3,
                    BucketName = "test-bucket",
                    Region = "us-east-1",
                    ExportFrequency = BlobStorageExportFrequency.Daily,
                    Enabled = true,
                    ForcePathStyle = false,
                    FileType = BlobStorageIntegrationFileType.Json,
                    ExportMode = BlobStorageExportMode.FullHistory,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.GetBlobStorageIntegrationsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldHaveSingleItem();
        result.Data[0].Id.ShouldBe("int-1");
        result.Data[0].Type.ShouldBe(BlobStorageIntegrationType.S3);

        // Verify correct endpoint was called
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.ToString().ShouldContain("/api/public/integrations/blob-storage");
    }

    [Fact]
    public async Task GetBlobStorageIntegrationsAsync_Success_HandlesEmptyArray()
    {
        // Arrange
        var expectedResponse = new BlobStorageIntegrationsResponse
        {
            Data = Array.Empty<BlobStorageIntegrationResponse>()
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.GetBlobStorageIntegrationsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetBlobStorageIntegrationsAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        await Should.ThrowAsync<LangfuseApiException>(async () => await _client.GetBlobStorageIntegrationsAsync());
    }

    [Fact]
    public async Task UpsertBlobStorageIntegrationAsync_Success_ReturnsCreatedIntegration()
    {
        // Arrange
        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = "proj-123",
            Type = BlobStorageIntegrationType.S3,
            BucketName = "my-bucket",
            Region = "us-east-1",
            ExportFrequency = BlobStorageExportFrequency.Daily,
            Enabled = true,
            ForcePathStyle = false,
            FileType = BlobStorageIntegrationFileType.Json,
            ExportMode = BlobStorageExportMode.FullHistory
        };

        var expectedResponse = new BlobStorageIntegrationResponse
        {
            Id = "int-new",
            ProjectId = request.ProjectId,
            Type = request.Type,
            BucketName = request.BucketName,
            Region = request.Region,
            ExportFrequency = request.ExportFrequency,
            Enabled = request.Enabled,
            ForcePathStyle = request.ForcePathStyle,
            FileType = request.FileType,
            ExportMode = request.ExportMode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertBlobStorageIntegrationAsync(request);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("int-new");
        result.ProjectId.ShouldBe(request.ProjectId);
        result.Type.ShouldBe(request.Type);

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Put);
        _httpHandler.LastRequest?.RequestUri?.ToString().ShouldContain("/api/public/integrations/blob-storage");

        // Verify request body was serialized
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"projectId\"");
        requestBody.ShouldContain("proj-123");
    }

    [Fact]
    public async Task UpsertBlobStorageIntegrationAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.UpsertBlobStorageIntegrationAsync(null!));
    }

    [Fact]
    public async Task UpsertBlobStorageIntegrationAsync_Forbidden_ThrowsException()
    {
        // Arrange
        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = "proj-123",
            Type = BlobStorageIntegrationType.S3,
            BucketName = "my-bucket",
            Region = "us-east-1",
            ExportFrequency = BlobStorageExportFrequency.Daily,
            Enabled = true,
            ForcePathStyle = false,
            FileType = BlobStorageIntegrationFileType.Json,
            ExportMode = BlobStorageExportMode.FullHistory
        };

        _httpHandler.SetupResponse(HttpStatusCode.Forbidden, "Access forbidden. Organization-scoped API key required.");

        // Act & Assert
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () =>
                await _client.UpsertBlobStorageIntegrationAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpsertBlobStorageIntegrationAsync_SerializesRequestCorrectly()
    {
        // Arrange
        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = "proj-123",
            Type = BlobStorageIntegrationType.S3Compatible,
            BucketName = "my-bucket",
            Endpoint = "https://s3.example.com",
            Region = "us-east-1",
            AccessKeyId = "AKIATEST",
            SecretAccessKey = "secret123",
            Prefix = "exports/",
            ExportFrequency = BlobStorageExportFrequency.Hourly,
            Enabled = true,
            ForcePathStyle = true,
            FileType = BlobStorageIntegrationFileType.Jsonl,
            ExportMode = BlobStorageExportMode.FromCustomDate,
            ExportStartDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var expectedResponse = new BlobStorageIntegrationResponse
        {
            Id = "int-new",
            ProjectId = request.ProjectId,
            Type = request.Type,
            BucketName = request.BucketName,
            Region = request.Region,
            ExportFrequency = request.ExportFrequency,
            Enabled = request.Enabled,
            ForcePathStyle = request.ForcePathStyle,
            FileType = request.FileType,
            ExportMode = request.ExportMode,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.UpsertBlobStorageIntegrationAsync(request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"projectId\"");
        requestBody.ShouldContain("\"S3_COMPATIBLE\"");
        requestBody.ShouldContain("\"endpoint\"");
        requestBody.ShouldContain("\"accessKeyId\"");
        requestBody.ShouldContain("\"secretAccessKey\"");
        requestBody.ShouldContain("\"exportStartDate\"");
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_Success_ReturnsConfirmation()
    {
        // Arrange
        var integrationId = "int-to-delete";
        var expectedResponse = new BlobStorageIntegrationDeletionResponse
        {
            Message = "Blob storage integration deleted successfully"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.DeleteBlobStorageIntegrationAsync(integrationId);

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldContain("deleted successfully");

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        _httpHandler.LastRequest?.RequestUri?.ToString()
            .ShouldContain($"/api/public/integrations/blob-storage/{integrationId}");
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_IdWithSpecialCharacters_UrlEncoded()
    {
        // Arrange
        var integrationId = "int-123/special";
        var expectedResponse = new BlobStorageIntegrationDeletionResponse
        {
            Message = "Deleted"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.DeleteBlobStorageIntegrationAsync(integrationId);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("int-123%2Fspecial"); // "/" should be URL encoded as %2F
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await _client.DeleteBlobStorageIntegrationAsync(null!));
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_EmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.DeleteBlobStorageIntegrationAsync(string.Empty));
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_WhitespaceId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await _client.DeleteBlobStorageIntegrationAsync("   "));
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_NotFound_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.NotFound, "Integration not found");

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.DeleteBlobStorageIntegrationAsync("non-existent-id"));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBlobStorageIntegrationsAsync_NetworkError_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupException(new HttpRequestException("Network error"));

        // Act & Assert
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.GetBlobStorageIntegrationsAsync());

        exception.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task UpsertBlobStorageIntegrationAsync_UnprocessableEntity_ThrowsException()
    {
        // Arrange
        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = "proj-123",
            Type = BlobStorageIntegrationType.S3,
            BucketName = "invalid-bucket",
            Region = "us-east-1",
            ExportFrequency = BlobStorageExportFrequency.Daily,
            Enabled = true,
            ForcePathStyle = false,
            FileType = BlobStorageIntegrationFileType.Json,
            ExportMode = BlobStorageExportMode.FullHistory
        };

        _httpHandler.SetupResponse(HttpStatusCode.UnprocessableEntity, "Validation failed");

        // Act & Assert
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () =>
                await _client.UpsertBlobStorageIntegrationAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_InternalServerError_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.InternalServerError, "Internal server error");

        // Act & Assert
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () =>
                await _client.DeleteBlobStorageIntegrationAsync("int-123"));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly List<HttpRequestMessage> _requests = new();
        private Exception? _exception;
        private HttpResponseMessage? _response;

        public HttpRequestMessage? LastRequest => _requests.LastOrDefault();

        public void SetupResponse(HttpStatusCode statusCode, object responseBody)
        {
            var json = JsonSerializer.Serialize(responseBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public void SetupResponse(HttpStatusCode statusCode, string responseBody)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        }

        public void SetupException(Exception exception)
        {
            _exception = exception;
        }

        public async Task<string?> GetLastRequestBodyAsync()
        {
            if (LastRequest?.Content == null)
            {
                return null;
            }

            return await LastRequest.Content.ReadAsStringAsync();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _requests.Add(request);

            if (_exception != null)
            {
                throw _exception;
            }

            if (_response != null)
            {
                _response.RequestMessage = request;
                return Task.FromResult(_response);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            });
        }
    }
}