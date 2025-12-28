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
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_Success()
    {
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

        var result = await _client.CreateAnnotationQueueAsync(request);

        result.ShouldNotBeNull();
        result.Id.ShouldBe("queue-123");
        result.Name.ShouldBe("Test Queue");
        result.Description.ShouldBe("Test Description");
        result.ScoreConfigIds.Length.ShouldBe(2);
        result.ScoreConfigIds[0].ShouldBe("score-1");
        result.ScoreConfigIds[1].ShouldBe("score-2");

        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/annotation-queues");
        requestUri.ShouldNotContain("?");
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
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"name\"");
        requestBody.ShouldContain("\"Serialization Test\"");
        requestBody.ShouldContain("\"description\"");
        requestBody.ShouldContain("\"Testing serialization\"");
        requestBody.ShouldContain("\"scoreConfigIds\"");
        requestBody.ShouldContain("\"cfg-1\"");
        requestBody.ShouldContain("\"cfg-2\"");
        requestBody.ShouldContain("\"cfg-3\"");
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
        result.ShouldNotBeNull();
        result.Id.ShouldBe("queue-789");
        result.Name.ShouldBe("Deserialization Test");
        result.Description.ShouldBeNull();
        result.ScoreConfigIds.ShouldHaveSingleItem();
        result.ScoreConfigIds[0].ShouldBe("cfg-x");
        result.CreatedAt.ShouldBe(now);
        result.UpdatedAt.ShouldBe(now);
    }

    [Fact]
    public async Task CreateAnnotationQueueAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => await _client.CreateAnnotationQueueAsync(null!));
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
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () =>
                await _client.CreateAnnotationQueueAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
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
        result.ShouldNotBeNull();
        result.UserId.ShouldBe("user-456");
        result.QueueId.ShouldBe("queue-123");
        result.ProjectId.ShouldBe("project-789");

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/annotation-queues/queue-123/assignments");
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
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.CreateQueueAssignmentAsync(null!, request));
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
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.CreateQueueAssignmentAsync(string.Empty, request));
    }

    [Fact]
    public async Task CreateQueueAssignmentAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.CreateQueueAssignmentAsync("queue-123", null!));
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
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.CreateQueueAssignmentAsync("queue-123", request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Forbidden);
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
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/annotation-queues/queue-123/assignments");
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
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"userId\"");
        requestBody.ShouldContain("\"user-xyz\"");
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
        result.Success.ShouldBeTrue();
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
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.DeleteQueueAssignmentAsync(null!, request));
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
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.DeleteQueueAssignmentAsync(string.Empty, request));
    }

    [Fact]
    public async Task DeleteQueueAssignmentAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.DeleteQueueAssignmentAsync("queue-123", null!));
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
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.DeleteQueueAssignmentAsync("queue-123", request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
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
        json.ShouldContain("\"name\":\"Test Queue\"");
        json.ShouldContain("\"description\":\"Test Description\"");
        json.ShouldContain("\"scoreConfigIds\"");
        json.ShouldContain("\"cfg-1\"");
        json.ShouldContain("\"cfg-2\"");
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
        json.ShouldContain("\"userId\":\"user-789\"");
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
        response.ShouldNotBeNull();
        response.UserId.ShouldBe("user-123");
        response.QueueId.ShouldBe("queue-456");
        response.ProjectId.ShouldBe("project-789");
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
        response.ShouldNotBeNull();
        response.Success.ShouldBeTrue();
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly List<HttpRequestMessage> _requests = new();
        private Exception? _exception;
        private string? _lastResponseBody;
        private HttpResponseMessage? _response;

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
            {
                return null;
            }

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