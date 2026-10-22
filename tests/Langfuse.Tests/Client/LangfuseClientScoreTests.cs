using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using zborek.Langfuse.Client;
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
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, logger);
    }

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
            DataType = ScoreConfigDataType.Numeric,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            ProjectId = "project-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpdateScoreConfigAsync(configId, request, TestContext.Current.CancellationToken);

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
            DataType = ScoreConfigDataType.Categorical,
            CreatedAt = DateTimeOffset.UtcNow.AddDays(-7),
            UpdatedAt = DateTimeOffset.UtcNow,
            ProjectId = "project-456"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpdateScoreConfigAsync(configId, request, TestContext.Current.CancellationToken);

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
            DataType = ScoreConfigDataType.Numeric,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            ProjectId = "project-789"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpdateScoreConfigAsync(configId, request, TestContext.Current.CancellationToken);

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
            DataType = ScoreConfigDataType.Numeric,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow,
            ProjectId = "project-test"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.UpdateScoreConfigAsync(configId, request, TestContext.Current.CancellationToken);

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
            DataType = ScoreConfigDataType.Boolean,
            MinValue = null,
            MaxValue = null,
            Categories = null,
            CreatedAt = now.AddDays(-10),
            UpdatedAt = now,
            ProjectId = "project-deserialize"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpdateScoreConfigAsync(configId, request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(configId);
        result.Name.ShouldBe("Deserialization Test");
        result.Description.ShouldBe("Some description");
        result.IsArchived.ShouldBeFalse();
        result.DataType.ShouldBe(ScoreConfigDataType.Boolean);
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

    [Fact]
    public async Task CreateScoreAsync_Success_SendsSingleItemArray()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "quality",
            Value = 0.95,
            TraceId = "trace-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Accepted, """{"message":"Accepted"}""");

        // Act
        var result = await _client.CreateScoreAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Accepted");

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/scores");
        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        body.ShouldStartWith("[");
        body.ShouldContain("trace-123");
    }

    [Fact]
    public async Task CreateScoreAsync_WithObservationId_Success()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "relevance",
            Value = 1.0,
            ObservationId = "observation-789"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Accepted, """{"message":"Accepted"}""");

        // Act
        var result = await _client.CreateScoreAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Message.ShouldBe("Accepted");
    }

    [Fact]
    public async Task CreateScoreAsync_WithSessionId_Success()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "session-quality",
            Value = 0.8,
            SessionId = "session-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Accepted, """{"message":"Accepted"}""");

        // Act
        var result = await _client.CreateScoreAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.Message.ShouldBe("Accepted");
    }

    [Fact]
    public async Task CreateScoreAsync_Rejected_ThrowsLangfuseApiException()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "quality",
            Value = 0.95,
            TraceId = "trace-123"
        };

        _httpHandler.SetupResponse((HttpStatusCode)207,
            """{"accepted":0,"rejected":1,"errors":[{"message":"invalid score"}]}""");

        // Act
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.CreateScoreAsync(request));

        // Assert
        exception.StatusCode.ShouldBe(207);
        exception.Message.ShouldContain("invalid score");
        exception.ResponseBody.ShouldNotBeNull().ShouldContain("invalid score");
    }

    [Fact]
    public async Task CreateScoreAsync_RejectedWithoutErrors_ThrowsLangfuseApiException()
    {
        var request = new ScoreCreateRequest { Name = "quality", Value = 0.95, TraceId = "trace-123" };

        _httpHandler.SetupResponse((HttpStatusCode)207, """{"accepted":0,"rejected":1,"errors":null}""");

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.CreateScoreAsync(request));

        exception.StatusCode.ShouldBe(207);
    }

    [Fact]
    public async Task CreateScoreAsync_WithoutId_AssignsId()
    {
        var request = new ScoreCreateRequest { Name = "quality", Value = 0.95, TraceId = "trace-123" };

        _httpHandler.SetupResponse(HttpStatusCode.Accepted, """{"message":"Accepted"}""");

        await _client.CreateScoreAsync(request, TestContext.Current.CancellationToken);

        request.Id.ShouldNotBeNullOrEmpty();
        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull().ShouldContain(request.Id);
    }

    [Fact]
    public async Task CreateScoreAsync_HttpClientTimeout_ThrowsRequestTimeout()
    {
        var httpClient = new HttpClient(new HangingHttpMessageHandler())
        {
            BaseAddress = new Uri("https://api.test.com/"),
            Timeout = TimeSpan.FromMilliseconds(50)
        };
        var client = new LangfuseClient(httpClient, Substitute.For<ILogger<LangfuseClient>>());
        var request = new ScoreCreateRequest { Name = "quality", Value = 0.95, TraceId = "trace-123" };

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.CreateScoreAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.RequestTimeout);
    }

    [Fact]
    public async Task CreateScoreAsync_RequestSerialization()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "test-score",
            Value = 0.75,
            TraceId = "trace-abc",
            Comment = "Test comment",
            DataType = ScoreDataType.Numeric,
            Timestamp = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.Zero)
        };

        _httpHandler.SetupResponse(HttpStatusCode.Accepted, """{"message":"Accepted"}""");

        // Act
        await _client.CreateScoreAsync(request, TestContext.Current.CancellationToken);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"timestamp\":\"2024-06-15T10:00:00+00:00\"");
        requestBody.ShouldContain("\"name\"");
        requestBody.ShouldContain("\"test-score\"");
        requestBody.ShouldContain("\"value\"");
        requestBody.ShouldContain("0.75");
        requestBody.ShouldContain("\"traceId\"");
        requestBody.ShouldContain("\"trace-abc\"");
        requestBody.ShouldContain("\"comment\"");
        requestBody.ShouldContain("\"Test comment\"");
    }

    [Fact]
    public async Task CreateScoreAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => await _client.CreateScoreAsync(null!));
    }

    [Fact]
    public async Task CreateScoreAsync_EmptyName_ThrowsArgumentException()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "",
            Value = 1.0,
            TraceId = "trace-123"
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await _client.CreateScoreAsync(request));
    }

    [Fact]
    public async Task CreateScoreAsync_NullName_ThrowsArgumentException()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = null!,
            Value = 1.0,
            TraceId = "trace-123"
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await _client.CreateScoreAsync(request));
    }

    [Fact]
    public async Task CreateScoreAsync_NullValue_ThrowsArgumentException()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "test",
            Value = null!,
            TraceId = "trace-123"
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await _client.CreateScoreAsync(request));
    }

    [Fact]
    public async Task CreateScoreAsync_NoTraceObservationOrSession_ThrowsArgumentException()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "test",
            Value = 1.0,
            TraceId = null,
            ObservationId = null,
            SessionId = null
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await _client.CreateScoreAsync(request));
    }

    [Fact]
    public async Task CreateScoreAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "test",
            Value = 1.0,
            TraceId = "trace-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.CreateScoreAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateScoreAsync_BadRequest_ThrowsException()
    {
        // Arrange
        var request = new ScoreCreateRequest
        {
            Name = "test",
            Value = 1.0,
            TraceId = "trace-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.BadRequest, "Bad Request");

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.CreateScoreAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateScoresAsync_Accepted_SendsArrayAndReturnsMessage()
    {
        // Arrange
        var requests = new[]
        {
            new ScoreCreateRequest { Name = "quality", Value = 0.9, TraceId = "trace-1" },
            new ScoreCreateRequest { Name = "quality", Value = 0.5, TraceId = "trace-2" }
        };

        _httpHandler.SetupResponse(HttpStatusCode.Accepted, """{"message":"Accepted"}""");

        // Act
        var result = await _client.CreateScoresAsync(requests, TestContext.Current.CancellationToken);

        // Assert
        result.Message.ShouldBe("Accepted");
        result.Accepted.ShouldBeNull();
        result.Errors.ShouldBeNull();
        requests.ShouldAllBe(r => !string.IsNullOrEmpty(r.Id));

        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.ToString().ShouldContain("/api/public/scores");
        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        body.ShouldStartWith("[");
        body.ShouldContain("trace-1");
        body.ShouldContain("trace-2");
    }

    [Fact]
    public async Task CreateScoresAsync_MultiStatus_ReturnsCountsAndErrors()
    {
        // Arrange
        var requests = new[]
        {
            new ScoreCreateRequest { Name = "quality", Value = 0.9, TraceId = "trace-1" },
            new ScoreCreateRequest { Name = "quality", Value = 0.5, TraceId = "trace-2" }
        };

        _httpHandler.SetupResponse((HttpStatusCode)207,
            """{"accepted":1,"rejected":1,"errors":[{"message":"invalid score"}]}""");

        // Act
        var result = await _client.CreateScoresAsync(requests, TestContext.Current.CancellationToken);

        // Assert
        result.Message.ShouldBeNull();
        result.Accepted.ShouldBe(1);
        result.Rejected.ShouldBe(1);
        result.Errors.ShouldHaveSingleItem().Message.ShouldBe("invalid score");
    }

    [Fact]
    public async Task CreateScoresAsync_EmptyBatch_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.CreateScoresAsync([]));
    }

    [Fact]
    public async Task CreateScoresAsync_InvalidItem_ThrowsArgumentException()
    {
        var requests = new[]
        {
            new ScoreCreateRequest { Name = "quality", Value = 0.9, TraceId = "trace-1" },
            new ScoreCreateRequest { Name = "", Value = 0.5, TraceId = "trace-2" }
        };

        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.CreateScoresAsync(requests));
    }

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

    private class HangingHttpMessageHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.Infinite, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}