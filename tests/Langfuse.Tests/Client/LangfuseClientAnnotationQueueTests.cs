using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using zborek.Langfuse.Client;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models.AnnotationQueue;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientAnnotationQueueTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientAnnotationQueueTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        var config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_Success()
    {
        // Arrange
        var request = new CreateAnnotationQueueRequest
        {
            Name = "Test Queue",
            Description = "Test Description",
            ScoreConfigIds = new[] { "score-1", "score-2" }
        };

        var expectedResponse = new AnnotationQueueModel
        {
            Id = "queue-123",
            Name = "Test Queue",
            Description = "Test Description",
            ScoreConfigIds = new[] { "score-1", "score-2" },
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.CreateAnnotationQueueAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("queue-123", result.Id);
        Assert.Equal("Test Queue", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(2, result.ScoreConfigIds.Length);
        Assert.Equal("score-1", result.ScoreConfigIds[0]);
        Assert.Equal("score-2", result.ScoreConfigIds[1]);

        // Verify correct endpoint and method
        Assert.Equal(HttpMethod.Post, _httpHandler.LastRequest?.Method);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("/api/public/annotation-queues", requestUri);
        Assert.DoesNotContain("?", requestUri);
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_RequestSerialization_Success()
    {
        // Arrange
        var request = new CreateAnnotationQueueRequest
        {
            Name = "Serialization Test",
            Description = "Testing serialization",
            ScoreConfigIds = new[] { "cfg-1", "cfg-2", "cfg-3" }
        };

        var expectedResponse = new AnnotationQueueModel
        {
            Id = "queue-456",
            Name = "Serialization Test",
            Description = "Testing serialization",
            ScoreConfigIds = new[] { "cfg-1", "cfg-2", "cfg-3" },
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.CreateAnnotationQueueAsync(request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"name\"", requestBody);
        Assert.Contains("\"Serialization Test\"", requestBody);
        Assert.Contains("\"description\"", requestBody);
        Assert.Contains("\"Testing serialization\"", requestBody);
        Assert.Contains("\"scoreConfigIds\"", requestBody);
        Assert.Contains("\"cfg-1\"", requestBody);
        Assert.Contains("\"cfg-2\"", requestBody);
        Assert.Contains("\"cfg-3\"", requestBody);
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_ResponseDeserialization_Success()
    {
        // Arrange
        var request = new CreateAnnotationQueueRequest
        {
            Name = "Deserialization Test",
            ScoreConfigIds = new[] { "cfg-x" }
        };

        var now = DateTimeOffset.UtcNow;
        var expectedResponse = new AnnotationQueueModel
        {
            Id = "queue-789",
            Name = "Deserialization Test",
            Description = null,
            ScoreConfigIds = new[] { "cfg-x" },
            CreatedAt = now,
            UpdatedAt = now
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.CreateAnnotationQueueAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("queue-789", result.Id);
        Assert.Equal("Deserialization Test", result.Name);
        Assert.Null(result.Description);
        Assert.Single(result.ScoreConfigIds);
        Assert.Equal("cfg-x", result.ScoreConfigIds[0]);
        Assert.Equal(now, result.CreatedAt);
        Assert.Equal(now, result.UpdatedAt);
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _client.CreateAnnotationQueueAsync(null!));
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        var request = new CreateAnnotationQueueRequest
        {
            Name = "Test",
            ScoreConfigIds = new[] { "cfg-1" }
        };

        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.CreateAnnotationQueueAsync(request));

        Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task CreateQueueAssignmentAsync_Success()
    {
        // Arrange
        var queueId = "queue-123";
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-456"
        };

        var expectedResponse = new CreateAnnotationQueueAssignmentResponse
        {
            UserId = "user-456",
            QueueId = "queue-123",
            ProjectId = "project-789"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.CreateQueueAssignmentAsync(queueId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user-456", result.UserId);
        Assert.Equal("queue-123", result.QueueId);
        Assert.Equal("project-789", result.ProjectId);

        // Verify correct endpoint and method
        Assert.Equal(HttpMethod.Post, _httpHandler.LastRequest?.Method);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("/api/public/annotation-queues/queue-123/assignments", requestUri);
    }

    [Fact]
    public async Task CreateQueueAssignmentAsync_NullQueueId_ThrowsArgumentException()
    {
        // Arrange
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.CreateQueueAssignmentAsync(null!, request));
    }

    [Fact]
    public async Task CreateQueueAssignmentAsync_EmptyQueueId_ThrowsArgumentException()
    {
        // Arrange
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.CreateQueueAssignmentAsync(string.Empty, request));
    }

    [Fact]
    public async Task CreateQueueAssignmentAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _client.CreateQueueAssignmentAsync("queue-123", null!));
    }

    [Fact]
    public async Task CreateQueueAssignmentAsync_Forbidden_ThrowsException()
    {
        // Arrange
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Forbidden, "Forbidden");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.CreateQueueAssignmentAsync("queue-123", request));

        Assert.Equal((int)HttpStatusCode.Forbidden, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteQueueAssignmentAsync_Success()
    {
        // Arrange
        var queueId = "queue-123";
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-456"
        };

        var expectedResponse = new DeleteAnnotationQueueAssignmentResponse
        {
            Success = true
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.DeleteQueueAssignmentAsync(queueId, request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);

        // Verify correct endpoint and method
        Assert.Equal(HttpMethod.Delete, _httpHandler.LastRequest?.Method);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("/api/public/annotation-queues/queue-123/assignments", requestUri);
    }

    [Fact]
    public async Task DeleteQueueAssignmentAsync_RequestBodySentWithDelete()
    {
        // Arrange
        var queueId = "queue-789";
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-xyz"
        };

        var expectedResponse = new DeleteAnnotationQueueAssignmentResponse
        {
            Success = true
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.DeleteQueueAssignmentAsync(queueId, request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"userId\"", requestBody);
        Assert.Contains("\"user-xyz\"", requestBody);
    }

    [Fact]
    public async Task DeleteQueueAssignmentAsync_ResponseIndicatesSuccess()
    {
        // Arrange
        var queueId = "queue-success";
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-success"
        };

        var expectedResponse = new DeleteAnnotationQueueAssignmentResponse
        {
            Success = true
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.DeleteQueueAssignmentAsync(queueId, request);

        // Assert
        Assert.True(result.Success);
    }

    [Fact]
    public async Task DeleteQueueAssignmentAsync_NullQueueId_ThrowsArgumentException()
    {
        // Arrange
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.DeleteQueueAssignmentAsync(null!, request));
    }

    [Fact]
    public async Task DeleteQueueAssignmentAsync_EmptyQueueId_ThrowsArgumentException()
    {
        // Arrange
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.DeleteQueueAssignmentAsync(string.Empty, request));
    }

    [Fact]
    public async Task DeleteQueueAssignmentAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _client.DeleteQueueAssignmentAsync("queue-123", null!));
    }

    [Fact]
    public async Task DeleteQueueAssignmentAsync_NotFound_ThrowsException()
    {
        // Arrange
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.NotFound, "Not Found");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.DeleteQueueAssignmentAsync("queue-123", request));

        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public void CreateAnnotationQueueRequest_Serialization()
    {
        // Arrange
        var request = new CreateAnnotationQueueRequest
        {
            Name = "Test Queue",
            Description = "Test Description",
            ScoreConfigIds = new[] { "cfg-1", "cfg-2" }
        };

        // Act
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains("\"name\":\"Test Queue\"", json);
        Assert.Contains("\"description\":\"Test Description\"", json);
        Assert.Contains("\"scoreConfigIds\"", json);
        Assert.Contains("\"cfg-1\"", json);
        Assert.Contains("\"cfg-2\"", json);
    }

    [Fact]
    public void AnnotationQueueAssignmentRequest_Serialization()
    {
        // Arrange
        var request = new AnnotationQueueAssignmentRequest
        {
            UserId = "user-789"
        };

        // Act
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains("\"userId\":\"user-789\"", json);
    }

    [Fact]
    public void CreateAnnotationQueueAssignmentResponse_Deserialization()
    {
        // Arrange
        var json = "{\"userId\":\"user-123\",\"queueId\":\"queue-456\",\"projectId\":\"project-789\"}";

        // Act
        var response = JsonSerializer.Deserialize<CreateAnnotationQueueAssignmentResponse>(json,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        // Assert
        Assert.NotNull(response);
        Assert.Equal("user-123", response.UserId);
        Assert.Equal("queue-456", response.QueueId);
        Assert.Equal("project-789", response.ProjectId);
    }

    [Fact]
    public void DeleteAnnotationQueueAssignmentResponse_Deserialization()
    {
        // Arrange
        var json = "{\"success\":true}";

        // Act
        var response = JsonSerializer.Deserialize<DeleteAnnotationQueueAssignmentResponse>(json,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage? _response;
        private Exception? _exception;
        private readonly List<HttpRequestMessage> _requests = new();
        private string? _lastResponseBody;

        public HttpRequestMessage? LastRequest => _requests.LastOrDefault();

        public void SetupResponse(HttpStatusCode statusCode, object responseBody)
        {
            var json = JsonSerializer.Serialize(responseBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            _lastResponseBody = json;
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        public void SetupResponse(HttpStatusCode statusCode, string responseBody)
        {
            _lastResponseBody = responseBody;
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

        public Task<string?> GetLastResponseBodyAsync()
        {
            return Task.FromResult(_lastResponseBody);
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
