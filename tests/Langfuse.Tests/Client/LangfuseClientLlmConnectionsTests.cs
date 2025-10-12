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
using zborek.Langfuse.Models.LlmConnection;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientLlmConnectionsTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientLlmConnectionsTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_WithoutPagination_Success()
    {
        // Arrange
        var expectedResponse = new PaginatedLlmConnections
        {
            Data = new[]
            {
                new LlmConnection
                {
                    Id = "conn-1",
                    Provider = "openai",
                    Adapter = LlmAdapter.OpenAi,
                    DisplaySecretKey = "sk-...abc",
                    CustomModels = Array.Empty<string>(),
                    WithDefaultModels = true,
                    ExtraHeaderKeys = Array.Empty<string>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            },
            Meta = new ApiMetadata
            {
                Page = 1,
                Limit = 50,
                TotalItems = 1,
                TotalPages = 1
            }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.GetLlmConnectionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data);
        Assert.Equal("conn-1", result.Data[0].Id);
        Assert.Equal("openai", result.Data[0].Provider);
        Assert.Equal(LlmAdapter.OpenAi, result.Data[0].Adapter);

        // Verify correct endpoint was called (no query string)
        Assert.Equal(HttpMethod.Get, _httpHandler.LastRequest?.Method);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("/api/public/llm-connections", requestUri);
        Assert.DoesNotContain("?", requestUri);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_WithPageOnly_BuildsCorrectUrl()
    {
        // Arrange
        var expectedResponse = new PaginatedLlmConnections
        {
            Data = Array.Empty<LlmConnection>(),
            Meta = new ApiMetadata { Page = 2, Limit = 50, TotalItems = 0, TotalPages = 0 }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.GetLlmConnectionsAsync(2);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("?page=2", requestUri);
        Assert.DoesNotContain("limit=", requestUri);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_WithLimitOnly_BuildsCorrectUrl()
    {
        // Arrange
        var expectedResponse = new PaginatedLlmConnections
        {
            Data = Array.Empty<LlmConnection>(),
            Meta = new ApiMetadata { Page = 1, Limit = 25, TotalItems = 0, TotalPages = 0 }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.GetLlmConnectionsAsync(limit: 25);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("?limit=25", requestUri);
        Assert.DoesNotContain("page=", requestUri);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_WithBothPageAndLimit_BuildsCorrectUrl()
    {
        // Arrange
        var expectedResponse = new PaginatedLlmConnections
        {
            Data = Array.Empty<LlmConnection>(),
            Meta = new ApiMetadata { Page = 3, Limit = 100, TotalItems = 0, TotalPages = 0 }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.GetLlmConnectionsAsync(3, 100);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("?page=3&limit=100", requestUri);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_EmptyResults_Success()
    {
        // Arrange
        var expectedResponse = new PaginatedLlmConnections
        {
            Data = Array.Empty<LlmConnection>(),
            Meta = new ApiMetadata { Page = 1, Limit = 50, TotalItems = 0, TotalPages = 0 }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.GetLlmConnectionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
        Assert.NotNull(result.Meta);
        Assert.Equal(0, result.Meta.TotalItems);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_MultipleConnections_Success()
    {
        // Arrange
        var expectedResponse = new PaginatedLlmConnections
        {
            Data = new[]
            {
                new LlmConnection
                {
                    Id = "conn-1",
                    Provider = "openai",
                    Adapter = LlmAdapter.OpenAi,
                    DisplaySecretKey = "sk-...abc",
                    CustomModels = Array.Empty<string>(),
                    WithDefaultModels = true,
                    ExtraHeaderKeys = Array.Empty<string>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new LlmConnection
                {
                    Id = "conn-2",
                    Provider = "anthropic",
                    Adapter = LlmAdapter.Anthropic,
                    DisplaySecretKey = "sk-...xyz",
                    BaseURL = "https://api.anthropic.com",
                    CustomModels = new[] { "claude-custom" },
                    WithDefaultModels = false,
                    ExtraHeaderKeys = new[] { "X-Custom-Header" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            },
            Meta = new ApiMetadata { Page = 1, Limit = 50, TotalItems = 2, TotalPages = 1 }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.GetLlmConnectionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Length);
        Assert.Equal("openai", result.Data[0].Provider);
        Assert.Equal("anthropic", result.Data[1].Provider);
        Assert.Equal(2, result.Meta.TotalItems);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.GetLlmConnectionsAsync());
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_Forbidden_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.Forbidden, "Forbidden");

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.GetLlmConnectionsAsync());

        Assert.Equal((int)HttpStatusCode.Forbidden, exception.StatusCode);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_Create_Success()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "my-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-secret123456"
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-new",
            Provider = "my-provider",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-sec***456",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("conn-new", result.Id);
        Assert.Equal("my-provider", result.Provider);
        Assert.Equal(LlmAdapter.OpenAi, result.Adapter);
        Assert.Equal("sk-sec***456", result.DisplaySecretKey);

        // Verify correct endpoint and method
        Assert.Equal(HttpMethod.Put, _httpHandler.LastRequest?.Method);
        Assert.Contains("/api/public/llm-connections", _httpHandler.LastRequest?.RequestUri?.ToString());
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_Update_Success()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "existing-provider",
            Adapter = LlmAdapter.Anthropic,
            SecretKey = "sk-newsecret789",
            BaseURL = "https://custom.api.com",
            CustomModels = new[] { "custom-model-1" },
            WithDefaultModels = false
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-existing",
            Provider = "existing-provider",
            Adapter = LlmAdapter.Anthropic,
            DisplaySecretKey = "sk-new***789",
            BaseURL = "https://custom.api.com",
            CustomModels = new[] { "custom-model-1" },
            WithDefaultModels = false,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("conn-existing", result.Id);
        Assert.Equal("existing-provider", result.Provider);
        Assert.Equal("https://custom.api.com", result.BaseURL);
        Assert.Single(result.CustomModels);
        Assert.False(result.WithDefaultModels);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_RequestSerialization_IncludesSecretKey()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-secret123",
            BaseURL = "https://api.example.com",
            CustomModels = new[] { "model-1", "model-2" },
            WithDefaultModels = true,
            ExtraHeaders = new Dictionary<string, string>
            {
                { "X-Custom-Header", "value1" },
                { "X-Another-Header", "value2" }
            }
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-sec***123",
            CustomModels = new[] { "model-1", "model-2" },
            WithDefaultModels = true,
            ExtraHeaderKeys = new[] { "X-Custom-Header", "X-Another-Header" },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.UpsertLlmConnectionAsync(request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"provider\"", requestBody);
        Assert.Contains("\"test-provider\"", requestBody);
        Assert.Contains("\"secretKey\"", requestBody);
        Assert.Contains("\"sk-secret123\"", requestBody);
        Assert.Contains("\"adapter\"", requestBody);
        Assert.Contains("\"baseURL\"", requestBody);
        Assert.Contains("\"customModels\"", requestBody);
        Assert.Contains("\"extraHeaders\"", requestBody);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_ResponseDeserialization_NoSecretKeyInResponse()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-secret123456789"
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-sec***789",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.Equal("sk-sec***789", result.DisplaySecretKey);

        // Verify response body doesn't contain full secret key
        var responseBody = await _httpHandler.GetLastResponseBodyAsync();
        Assert.NotNull(responseBody);
        Assert.DoesNotContain("sk-secret123456789", responseBody);
        Assert.Contains("sk-sec***789", responseBody);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_WithAllAdapters_Success()
    {
        // Test different adapters
        var adapters = new[]
        {
            LlmAdapter.OpenAi,
            LlmAdapter.Anthropic,
            LlmAdapter.Azure,
            LlmAdapter.Bedrock,
            LlmAdapter.GoogleVertexAi,
            LlmAdapter.GoogleAiStudio
        };

        foreach (var adapter in adapters)
        {
            // Arrange
            var request = new UpsertLlmConnectionRequest
            {
                Provider = $"provider-{adapter}",
                Adapter = adapter,
                SecretKey = "sk-test"
            };

            var expectedResponse = new LlmConnection
            {
                Id = $"conn-{adapter}",
                Provider = $"provider-{adapter}",
                Adapter = adapter,
                DisplaySecretKey = "sk-***st",
                CustomModels = Array.Empty<string>(),
                WithDefaultModels = true,
                ExtraHeaderKeys = Array.Empty<string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

            // Act
            var result = await _client.UpsertLlmConnectionAsync(request);

            // Assert
            Assert.Equal(adapter, result.Adapter);
        }
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _client.UpsertLlmConnectionAsync(null!));
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_BadRequest_InvalidAdapter_ThrowsException()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test"
        };

        _httpHandler.SetupResponse(HttpStatusCode.BadRequest, "Invalid adapter specified");

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        Assert.Equal((int)HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        Assert.Equal((int)HttpStatusCode.Unauthorized, exception.StatusCode);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_Forbidden_ThrowsException()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Forbidden, "Forbidden");

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        Assert.Equal((int)HttpStatusCode.Forbidden, exception.StatusCode);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_Conflict_ThrowsException()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "duplicate-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Conflict, "Provider already exists");

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        Assert.Equal((int)HttpStatusCode.Conflict, exception.StatusCode);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_NetworkError_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupException(new HttpRequestException("Network error"));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.GetLlmConnectionsAsync());

        Assert.Equal((int)HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_NetworkError_ThrowsException()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test"
        };

        _httpHandler.SetupException(new HttpRequestException("Network error"));

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        Assert.Equal((int)HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_InternalServerError_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.InternalServerError, "Internal server error");

        // Act & Assert
        var exception =
            await Assert.ThrowsAsync<LangfuseApiException>(async () => await _client.GetLlmConnectionsAsync());

        Assert.Equal((int)HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_WithEmptyCustomModels_Success()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-***st",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.CustomModels);
        Assert.True(result.WithDefaultModels);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_WithNullCustomModels_Success()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.Anthropic,
            SecretKey = "sk-test",
            CustomModels = null,
            WithDefaultModels = null
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.Anthropic,
            DisplaySecretKey = "sk-***st",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.CustomModels);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_WithEmptyExtraHeaders_Success()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.Bedrock,
            SecretKey = "sk-test",
            ExtraHeaders = new Dictionary<string, string>()
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.Bedrock,
            DisplaySecretKey = "sk-***st",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.ExtraHeaderKeys);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_WithNullExtraHeaders_Success()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.Azure,
            SecretKey = "sk-test",
            ExtraHeaders = null
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.Azure,
            DisplaySecretKey = "sk-***st",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.ExtraHeaderKeys);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_WithNullBaseURL_Success()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test",
            BaseURL = null
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-***st",
            BaseURL = null,
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.BaseURL);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_WithCustomBaseURL_Success()
    {
        // Arrange
        var customBaseUrl = "https://my-custom-gateway.example.com/v1";
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "custom-gateway",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test",
            BaseURL = customBaseUrl
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "custom-gateway",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-***st",
            BaseURL = customBaseUrl,
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(customBaseUrl, result.BaseURL);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_ProviderUniqueness_UpdatesExisting()
    {
        // Arrange - First create
        var initialRequest = new UpsertLlmConnectionRequest
        {
            Provider = "my-unique-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-initial"
        };

        var initialResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "my-unique-provider",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-ini***al",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, initialResponse);

        // Act - First upsert (create)
        var firstResult = await _client.UpsertLlmConnectionAsync(initialRequest);

        // Arrange - Second upsert with same provider (should update)
        var updateRequest = new UpsertLlmConnectionRequest
        {
            Provider = "my-unique-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-updated",
            CustomModels = new[] { "gpt-4-custom" },
            WithDefaultModels = false
        };

        var updatedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "my-unique-provider",
            Adapter = LlmAdapter.OpenAi,
            DisplaySecretKey = "sk-upd***ed",
            CustomModels = new[] { "gpt-4-custom" },
            WithDefaultModels = false,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, updatedResponse);

        // Act - Second upsert (update)
        var secondResult = await _client.UpsertLlmConnectionAsync(updateRequest);

        // Assert
        Assert.Equal("conn-123", firstResult.Id);
        Assert.Equal("conn-123", secondResult.Id);
        Assert.Equal("my-unique-provider", secondResult.Provider);
        Assert.Single(secondResult.CustomModels);
        Assert.False(secondResult.WithDefaultModels);
        Assert.True(secondResult.UpdatedAt > secondResult.CreatedAt);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_CustomModelsWithDefaultModels_Success()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "hybrid-models",
            Adapter = LlmAdapter.Anthropic,
            SecretKey = "sk-test",
            CustomModels = new[] { "claude-custom-1", "claude-custom-2" },
            WithDefaultModels = true
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "hybrid-models",
            Adapter = LlmAdapter.Anthropic,
            DisplaySecretKey = "sk-***st",
            CustomModels = new[] { "claude-custom-1", "claude-custom-2" },
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.CustomModels.Length);
        Assert.True(result.WithDefaultModels);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_WithAllGoogleAdapters_Success()
    {
        // Arrange
        var expectedResponse = new PaginatedLlmConnections
        {
            Data = new[]
            {
                new LlmConnection
                {
                    Id = "conn-1",
                    Provider = "google-vertex",
                    Adapter = LlmAdapter.GoogleVertexAi,
                    DisplaySecretKey = "***",
                    CustomModels = Array.Empty<string>(),
                    WithDefaultModels = true,
                    ExtraHeaderKeys = Array.Empty<string>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new LlmConnection
                {
                    Id = "conn-2",
                    Provider = "google-studio",
                    Adapter = LlmAdapter.GoogleAiStudio,
                    DisplaySecretKey = "***",
                    CustomModels = Array.Empty<string>(),
                    WithDefaultModels = true,
                    ExtraHeaderKeys = Array.Empty<string>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            },
            Meta = new ApiMetadata { Page = 1, Limit = 50, TotalItems = 2, TotalPages = 1 }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.GetLlmConnectionsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Data.Length);
        Assert.Equal(LlmAdapter.GoogleVertexAi, result.Data[0].Adapter);
        Assert.Equal(LlmAdapter.GoogleAiStudio, result.Data[1].Adapter);
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_ResponseContainsDisplaySecretKeyNotSecretKey()
    {
        // Arrange
        var secretKey = "sk-ant-api03-very-long-secret-key-12345678";
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.Anthropic,
            SecretKey = secretKey
        };

        var expectedResponse = new LlmConnection
        {
            Id = "conn-123",
            Provider = "test-provider",
            Adapter = LlmAdapter.Anthropic,
            DisplaySecretKey = "sk-ant-***678",
            CustomModels = Array.Empty<string>(),
            WithDefaultModels = true,
            ExtraHeaderKeys = Array.Empty<string>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.UpsertLlmConnectionAsync(request);

        // Assert
        Assert.NotNull(result.DisplaySecretKey);
        Assert.Contains("***", result.DisplaySecretKey);
        Assert.DoesNotContain("very-long-secret-key", result.DisplaySecretKey);

        // Verify request body contained full secret
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.Contains(secretKey, requestBody!);

        // Verify response body contains masked secret only
        var responseBody = await _httpHandler.GetLastResponseBodyAsync();
        Assert.DoesNotContain(secretKey, responseBody!);
        Assert.Contains("***", responseBody!);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_PaginationMeta_ReflectsCorrectValues()
    {
        // Arrange
        var expectedResponse = new PaginatedLlmConnections
        {
            Data = new[]
            {
                new LlmConnection
                {
                    Id = "conn-1",
                    Provider = "provider-1",
                    Adapter = LlmAdapter.OpenAi,
                    DisplaySecretKey = "***",
                    CustomModels = Array.Empty<string>(),
                    WithDefaultModels = true,
                    ExtraHeaderKeys = Array.Empty<string>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            },
            Meta = new ApiMetadata
            {
                Page = 2,
                Limit = 10,
                TotalItems = 25,
                TotalPages = 3
            }
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.GetLlmConnectionsAsync(2, 10);

        // Assert
        Assert.NotNull(result.Meta);
        Assert.Equal(2, result.Meta.Page);
        Assert.Equal(10, result.Meta.Limit);
        Assert.Equal(25, result.Meta.TotalItems);
        Assert.Equal(3, result.Meta.TotalPages);
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