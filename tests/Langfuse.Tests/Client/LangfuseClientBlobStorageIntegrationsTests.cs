using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
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
        var config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    #region GetBlobStorageIntegrationsAsync Tests

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
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("int-1", result.Data[0].Id);
        Assert.Equal(BlobStorageIntegrationType.S3, result.Data[0].Type);

        // Verify correct endpoint was called
        Assert.Equal(HttpMethod.Get, _httpHandler.LastRequest?.Method);
        Assert.Contains("/api/public/integrations/blob-storage", _httpHandler.LastRequest?.RequestUri?.ToString());
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
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetBlobStorageIntegrationsAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.GetBlobStorageIntegrationsAsync());
    }

    #endregion

    #region UpsertBlobStorageIntegrationAsync Tests

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
        Assert.NotNull(result);
        Assert.Equal("int-new", result.Id);
        Assert.Equal(request.ProjectId, result.ProjectId);
        Assert.Equal(request.Type, result.Type);

        // Verify correct endpoint and method
        Assert.Equal(HttpMethod.Put, _httpHandler.LastRequest?.Method);
        Assert.Contains("/api/public/integrations/blob-storage", _httpHandler.LastRequest?.RequestUri?.ToString());

        // Verify request body was serialized
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"projectId\"", requestBody);
        Assert.Contains("proj-123", requestBody);
    }

    [Fact]
    public async Task UpsertBlobStorageIntegrationAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _client.UpsertBlobStorageIntegrationAsync(null!));
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
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.UpsertBlobStorageIntegrationAsync(request));

        Assert.Equal((int)HttpStatusCode.Forbidden, exception.StatusCode);
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
        Assert.NotNull(requestBody);
        Assert.Contains("\"projectId\"", requestBody);
        Assert.Contains("\"S3_COMPATIBLE\"", requestBody);
        Assert.Contains("\"endpoint\"", requestBody);
        Assert.Contains("\"accessKeyId\"", requestBody);
        Assert.Contains("\"secretAccessKey\"", requestBody);
        Assert.Contains("\"exportStartDate\"", requestBody);
    }

    #endregion

    #region DeleteBlobStorageIntegrationAsync Tests

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
        Assert.NotNull(result);
        Assert.Contains("deleted successfully", result.Message);

        // Verify correct endpoint and method
        Assert.Equal(HttpMethod.Delete, _httpHandler.LastRequest?.Method);
        Assert.Contains($"/api/public/integrations/blob-storage/{integrationId}",
            _httpHandler.LastRequest?.RequestUri?.ToString());
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
        Assert.NotNull(requestUri);
        Assert.Contains("int-123%2Fspecial", requestUri); // "/" should be URL encoded as %2F
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_NullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.DeleteBlobStorageIntegrationAsync(null!));
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_EmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.DeleteBlobStorageIntegrationAsync(string.Empty));
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_WhitespaceId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.DeleteBlobStorageIntegrationAsync("   "));
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_NotFound_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.NotFound, "Integration not found");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.DeleteBlobStorageIntegrationAsync("non-existent-id"));

        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task GetBlobStorageIntegrationsAsync_NetworkError_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupException(new HttpRequestException("Network error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.GetBlobStorageIntegrationsAsync());

        Assert.Equal((int)HttpStatusCode.InternalServerError, exception.StatusCode);
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
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.UpsertBlobStorageIntegrationAsync(request));

        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteBlobStorageIntegrationAsync_InternalServerError_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.InternalServerError, "Internal server error");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.DeleteBlobStorageIntegrationAsync("int-123"));

        Assert.Equal((int)HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    #endregion

    #region Test Helper Class

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage? _response;
        private Exception? _exception;
        private readonly List<HttpRequestMessage> _requests = new();

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
                return null;

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

    #endregion
}
