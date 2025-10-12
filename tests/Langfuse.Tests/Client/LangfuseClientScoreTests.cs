using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using zborek.Langfuse.Client;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientScoreTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientScoreTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        var config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    #region UpdateScoreConfigAsync Tests

    [Fact]
    public async Task UpdateScoreConfigAsync_Success()
    {
        // Arrange
        var configId = "config-123";
        var request = new UpdateScoreConfigRequest
        {
            Name = "Updated Config",
            Description = "Updated Description",
            IsArchived = true
        };

        var expectedResponse = new ScoreConfig
        {
            Id = configId,
            Name = "Updated Config",
            Description = "Updated Description",
            IsArchived = true,
            DataType = ScoreDataType.Numeric,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            ProjectId = "project-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpdateScoreConfigAsync(configId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(configId, result.Id);
        Assert.Equal("Updated Config", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.True(result.IsArchived);

        // Verify correct endpoint and method
        Assert.Equal("PATCH", _httpHandler.LastRequest?.Method.Method);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("/api/public/score-configs/config-123", requestUri);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_PartialUpdate_OnlySomeFields()
    {
        // Arrange
        var configId = "config-456";
        var request = new UpdateScoreConfigRequest
        {
            Name = "Partially Updated",
            IsArchived = false
        };

        var expectedResponse = new ScoreConfig
        {
            Id = configId,
            Name = "Partially Updated",
            Description = "Original Description",
            IsArchived = false,
            DataType = ScoreDataType.Categorical,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-7),
            UpdatedAt = DateTimeOffset.UtcNow,
            ProjectId = "project-456"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpdateScoreConfigAsync(configId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Partially Updated", result.Name);
        Assert.Equal("Original Description", result.Description);
        Assert.False(result.IsArchived);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_AllFields()
    {
        // Arrange
        var configId = "config-789";
        var request = new UpdateScoreConfigRequest
        {
            Name = "Fully Updated",
            Description = "New Description",
            IsArchived = true,
            MinValue = 0.0,
            MaxValue = 10.0,
            Categories = new[]
            {
                new ConfigCategory { Label = "Good", Value = 1 },
                new ConfigCategory { Label = "Bad", Value = 0 }
            }
        };

        var expectedResponse = new ScoreConfig
        {
            Id = configId,
            Name = "Fully Updated",
            Description = "New Description",
            IsArchived = true,
            MinValue = 0.0,
            MaxValue = 10.0,
            Categories = new[]
            {
                new ConfigCategory { Label = "Good", Value = 1 },
                new ConfigCategory { Label = "Bad", Value = 0 }
            },
            DataType = ScoreDataType.Numeric,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            ProjectId = "project-789"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpdateScoreConfigAsync(configId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Fully Updated", result.Name);
        Assert.Equal("New Description", result.Description);
        Assert.True(result.IsArchived);
        Assert.Equal(0.0, result.MinValue);
        Assert.Equal(10.0, result.MaxValue);
        Assert.NotNull(result.Categories);
        Assert.Equal(2, result.Categories.Length);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_RequestSerialization()
    {
        // Arrange
        var configId = "config-serialize";
        var request = new UpdateScoreConfigRequest
        {
            Name = "Serialization Test",
            Description = "Testing serialization",
            MinValue = 1.5,
            MaxValue = 9.5
        };

        var expectedResponse = new ScoreConfig
        {
            Id = configId,
            Name = "Serialization Test",
            DataType = ScoreDataType.Numeric,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            ProjectId = "project-test"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.UpdateScoreConfigAsync(configId, request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"name\"", requestBody);
        Assert.Contains("\"Serialization Test\"", requestBody);
        Assert.Contains("\"description\"", requestBody);
        Assert.Contains("\"Testing serialization\"", requestBody);
        Assert.Contains("\"minValue\"", requestBody);
        Assert.Contains("1.5", requestBody);
        Assert.Contains("\"maxValue\"", requestBody);
        Assert.Contains("9.5", requestBody);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_ResponseDeserialization()
    {
        // Arrange
        var configId = "config-deserialize";
        var now = DateTimeOffset.UtcNow;
        var request = new UpdateScoreConfigRequest
        {
            Name = "Deserialization Test"
        };

        var expectedResponse = new ScoreConfig
        {
            Id = configId,
            Name = "Deserialization Test",
            Description = "Some description",
            IsArchived = false,
            DataType = ScoreDataType.Boolean,
            MinValue = null,
            MaxValue = null,
            Categories = null,
            CreatedAt = now.AddDays(-10),
            UpdatedAt = now,
            ProjectId = "project-deserialize"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpdateScoreConfigAsync(configId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(configId, result.Id);
        Assert.Equal("Deserialization Test", result.Name);
        Assert.Equal("Some description", result.Description);
        Assert.False(result.IsArchived);
        Assert.Equal(ScoreDataType.Boolean, result.DataType);
        Assert.Null(result.MinValue);
        Assert.Null(result.MaxValue);
        Assert.Null(result.Categories);
        Assert.Equal(now.AddDays(-10), result.CreatedAt);
        Assert.Equal(now, result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_NullConfigId_ThrowsArgumentException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.UpdateScoreConfigAsync(null!, request));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_EmptyConfigId_ThrowsArgumentException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.UpdateScoreConfigAsync(string.Empty, request));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_WhitespaceConfigId_ThrowsArgumentException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.UpdateScoreConfigAsync("   ", request));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _client.UpdateScoreConfigAsync("config-123", null!));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_NotFound_ThrowsException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };
        _httpHandler.SetupResponse(HttpStatusCode.NotFound, "Not Found");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.UpdateScoreConfigAsync("config-404", request));

        Assert.Equal((int)HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };
        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.UpdateScoreConfigAsync("config-123", request));

        Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    #endregion

    #region GetScoreListAsync with SessionId Tests

    [Fact]
    public async Task GetScoreListAsync_WithSessionId_IncludesInQueryString()
    {
        // Arrange
        var request = new ScoreListRequest
        {
            SessionId = "session-123",
            Page = 1,
            Limit = 10
        };

        var expectedResponse = new ScoreListResponse
        {
            Data = Array.Empty<ScoreModel>(),
            Meta = new ApiMetadata
            {
                Page = 1,
                Limit = 10,
                TotalItems = 0,
                TotalPages = 0
            }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.GetScoreListAsync(request);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("sessionId=session-123", requestUri);
    }

    [Fact]
    public async Task GetScoreListAsync_SessionIdUrlEncoded()
    {
        // Arrange
        var request = new ScoreListRequest
        {
            SessionId = "session with spaces"
        };

        var expectedResponse = new ScoreListResponse
        {
            Data = Array.Empty<ScoreModel>(),
            Meta = new ApiMetadata
            {
                Page = 1,
                Limit = 50,
                TotalItems = 0,
                TotalPages = 0
            }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.GetScoreListAsync(request);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("sessionId=session+with+spaces", requestUri);
    }

    [Fact]
    public async Task GetScoreListAsync_WithSessionIdAndOtherParameters()
    {
        // Arrange
        var request = new ScoreListRequest
        {
            SessionId = "session-456",
            UserId = "user-789",
            Name = "score-name",
            Page = 2,
            Limit = 20
        };

        var expectedResponse = new ScoreListResponse
        {
            Data = Array.Empty<ScoreModel>(),
            Meta = new ApiMetadata
            {
                Page = 2,
                Limit = 20,
                TotalItems = 0,
                TotalPages = 0
            }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.GetScoreListAsync(request);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("sessionId=session-456", requestUri);
        Assert.Contains("userId=user-789", requestUri);
        Assert.Contains("name=score-name", requestUri);
        Assert.Contains("page=2", requestUri);
        Assert.Contains("limit=20", requestUri);
    }

    [Fact]
    public async Task GetScoreListAsync_NullSessionId_NotIncludedInQueryString()
    {
        // Arrange
        var request = new ScoreListRequest
        {
            SessionId = null,
            Page = 1
        };

        var expectedResponse = new ScoreListResponse
        {
            Data = Array.Empty<ScoreModel>(),
            Meta = new ApiMetadata
            {
                Page = 1,
                Limit = 50,
                TotalItems = 0,
                TotalPages = 0
            }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.GetScoreListAsync(request);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.DoesNotContain("sessionId", requestUri);
    }

    [Fact]
    public async Task GetScoreListAsync_EmptySessionId_NotIncludedInQueryString()
    {
        // Arrange
        var request = new ScoreListRequest
        {
            SessionId = string.Empty,
            Page = 1
        };

        var expectedResponse = new ScoreListResponse
        {
            Data = Array.Empty<ScoreModel>(),
            Meta = new ApiMetadata
            {
                Page = 1,
                Limit = 50,
                TotalItems = 0,
                TotalPages = 0
            }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.GetScoreListAsync(request);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.DoesNotContain("sessionId", requestUri);
    }

    #endregion

    #region Model Serialization Tests

    [Fact]
    public void UpdateScoreConfigRequest_Serialization_AllFields()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest
        {
            Name = "Test Config",
            Description = "Test Description",
            IsArchived = true,
            MinValue = 0.0,
            MaxValue = 100.0,
            Categories = new[]
            {
                new ConfigCategory { Label = "Excellent", Value = 5 },
                new ConfigCategory { Label = "Poor", Value = 1 }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains("\"name\":\"Test Config\"", json);
        Assert.Contains("\"description\":\"Test Description\"", json);
        Assert.Contains("\"isArchived\":true", json);
        Assert.Contains("\"minValue\":0", json);
        Assert.Contains("\"maxValue\":100", json);
        Assert.Contains("\"categories\"", json);
        Assert.Contains("\"label\":\"Excellent\"", json);
        Assert.Contains("\"value\":5", json);
    }

    [Fact]
    public void UpdateScoreConfigRequest_Serialization_NullableFieldsOmitted()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest
        {
            Name = "Minimal Config"
        };

        // Act
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        // Assert
        Assert.Contains("\"name\":\"Minimal Config\"", json);
        Assert.DoesNotContain("\"description\"", json);
        Assert.DoesNotContain("\"isArchived\"", json);
        Assert.DoesNotContain("\"minValue\"", json);
        Assert.DoesNotContain("\"maxValue\"", json);
        Assert.DoesNotContain("\"categories\"", json);
    }

    [Fact]
    public void ScoreListRequest_Serialization_WithSessionId()
    {
        // Arrange
        var request = new ScoreListRequest
        {
            SessionId = "session-xyz",
            Page = 1,
            Limit = 25
        };

        // Act
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains("\"sessionId\":\"session-xyz\"", json);
        Assert.Contains("\"page\":1", json);
        Assert.Contains("\"limit\":25", json);
    }

    #endregion

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
