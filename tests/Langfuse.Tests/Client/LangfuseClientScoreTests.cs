using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
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
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
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
        result.ShouldNotBeNull();
        result.Id.ShouldBe(configId);
        result.Name.ShouldBe("Updated Config");
        result.Description.ShouldBe("Updated Description");
        result.IsArchived.ShouldBeTrue();

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.Method.ShouldBe("PATCH");
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/score-configs/config-123");
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
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Partially Updated");
        result.Description.ShouldBe("Original Description");
        result.IsArchived.ShouldBeFalse();
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
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Fully Updated");
        result.Description.ShouldBe("New Description");
        result.IsArchived.ShouldBeTrue();
        result.MinValue.ShouldBe(0.0);
        result.MaxValue.ShouldBe(10.0);
        result.Categories.ShouldNotBeNull();
        result.Categories.Length.ShouldBe(2);
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
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"name\"");
        requestBody.ShouldContain("\"Serialization Test\"");
        requestBody.ShouldContain("\"description\"");
        requestBody.ShouldContain("\"Testing serialization\"");
        requestBody.ShouldContain("\"minValue\"");
        requestBody.ShouldContain("1.5");
        requestBody.ShouldContain("\"maxValue\"");
        requestBody.ShouldContain("9.5");
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
        result.ShouldNotBeNull();
        result.Id.ShouldBe(configId);
        result.Name.ShouldBe("Deserialization Test");
        result.Description.ShouldBe("Some description");
        result.IsArchived.ShouldBeFalse();
        result.DataType.ShouldBe(ScoreDataType.Boolean);
        result.MinValue.ShouldBeNull();
        result.MaxValue.ShouldBeNull();
        result.Categories.ShouldBeNull();
        result.CreatedAt.ShouldBe(now.AddDays(-10));
        result.UpdatedAt.ShouldBe(now);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_NullConfigId_ThrowsArgumentException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await _client.UpdateScoreConfigAsync(null!, request));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_EmptyConfigId_ThrowsArgumentException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.UpdateScoreConfigAsync(string.Empty, request));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_WhitespaceConfigId_ThrowsArgumentException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await _client.UpdateScoreConfigAsync("   ", request));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.UpdateScoreConfigAsync("config-123", null!));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_NotFound_ThrowsException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };
        _httpHandler.SetupResponse(HttpStatusCode.NotFound, "Not Found");

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.UpdateScoreConfigAsync("config-404", request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        var request = new UpdateScoreConfigRequest { Name = "Test" };
        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.UpdateScoreConfigAsync("config-123", request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("sessionId=session-123");
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("sessionId=session+with+spaces");
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("sessionId=session-456");
        requestUri.ShouldContain("userId=user-789");
        requestUri.ShouldContain("name=score-name");
        requestUri.ShouldContain("page=2");
        requestUri.ShouldContain("limit=20");
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldNotContain("sessionId");
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldNotContain("sessionId");
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
        json.ShouldContain("\"name\":\"Test Config\"");
        json.ShouldContain("\"description\":\"Test Description\"");
        json.ShouldContain("\"isArchived\":true");
        json.ShouldContain("\"minValue\":0");
        json.ShouldContain("\"maxValue\":100");
        json.ShouldContain("\"categories\"");
        json.ShouldContain("\"label\":\"Excellent\"");
        json.ShouldContain("\"value\":5");
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
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        // Assert
        json.ShouldContain("\"name\":\"Minimal Config\"");
        json.ShouldNotContain("\"description\"");
        json.ShouldNotContain("\"isArchived\"");
        json.ShouldNotContain("\"minValue\"");
        json.ShouldNotContain("\"maxValue\"");
        json.ShouldNotContain("\"categories\"");
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
        json.ShouldContain("\"sessionId\":\"session-xyz\"");
        json.ShouldContain("\"page\":1");
        json.ShouldContain("\"limit\":25");
    }

    #endregion
}