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
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldHaveSingleItem();
        result.Data[0].Id.ShouldBe("conn-1");
        result.Data[0].Provider.ShouldBe("openai");
        result.Data[0].Adapter.ShouldBe(LlmAdapter.OpenAi);

        // Verify correct endpoint was called (no query string)
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/llm-connections");
        requestUri.ShouldNotContain("?");
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("?page=2");
        requestUri.ShouldNotContain("limit=");
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("?limit=25");
        requestUri.ShouldNotContain("page=");
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("?page=3&limit=100");
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
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldBeEmpty();
        result.Meta.ShouldNotBeNull();
        result.Meta.TotalItems.ShouldBe(0);
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
        result.ShouldNotBeNull();
        result.Data.Length.ShouldBe(2);
        result.Data[0].Provider.ShouldBe("openai");
        result.Data[1].Provider.ShouldBe("anthropic");
        result.Meta.TotalItems.ShouldBe(2);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_Unauthorized_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.Unauthorized, "Unauthorized");

        // Act & Assert
        await Should.ThrowAsync<LangfuseApiException>(async () => await _client.GetLlmConnectionsAsync());
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_Forbidden_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.Forbidden, "Forbidden");

        // Act & Assert
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.GetLlmConnectionsAsync());

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Forbidden);
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
        result.ShouldNotBeNull();
        result.Id.ShouldBe("conn-new");
        result.Provider.ShouldBe("my-provider");
        result.Adapter.ShouldBe(LlmAdapter.OpenAi);
        result.DisplaySecretKey.ShouldBe("sk-sec***456");

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Put);
        _httpHandler.LastRequest?.RequestUri?.ToString().ShouldContain("/api/public/llm-connections");
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
        result.ShouldNotBeNull();
        result.Id.ShouldBe("conn-existing");
        result.Provider.ShouldBe("existing-provider");
        result.BaseURL.ShouldBe("https://custom.api.com");
        result.CustomModels.ShouldHaveSingleItem();
        result.WithDefaultModels.ShouldBeFalse();
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
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"provider\"");
        requestBody.ShouldContain("\"test-provider\"");
        requestBody.ShouldContain("\"secretKey\"");
        requestBody.ShouldContain("\"sk-secret123\"");
        requestBody.ShouldContain("\"adapter\"");
        requestBody.ShouldContain("\"baseURL\"");
        requestBody.ShouldContain("\"customModels\"");
        requestBody.ShouldContain("\"extraHeaders\"");
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
        result.DisplaySecretKey.ShouldBe("sk-sec***789");

        // Verify response body doesn't contain full secret key
        var responseBody = await _httpHandler.GetLastResponseBodyAsync();
        responseBody.ShouldNotBeNull();
        responseBody.ShouldNotContain("sk-secret123456789");
        responseBody.ShouldContain("sk-sec***789");
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
            result.Adapter.ShouldBe(adapter);
        }
    }

    [Fact]
    public async Task UpsertLlmConnectionAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () => await _client.UpsertLlmConnectionAsync(null!));
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
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.BadRequest);
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
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
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
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Forbidden);
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
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_NetworkError_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupException(new HttpRequestException("Network error"));

        // Act & Assert
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.GetLlmConnectionsAsync());

        exception.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
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
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.UpsertLlmConnectionAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetLlmConnectionsAsync_InternalServerError_ThrowsException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.InternalServerError, "Internal server error");

        // Act & Assert
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () => await _client.GetLlmConnectionsAsync());

        exception.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
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
        result.ShouldNotBeNull();
        result.CustomModels.ShouldBeEmpty();
        result.WithDefaultModels.ShouldBeTrue();
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
        result.ShouldNotBeNull();
        result.CustomModels.ShouldBeEmpty();
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
        result.ShouldNotBeNull();
        result.ExtraHeaderKeys.ShouldBeEmpty();
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
        result.ShouldNotBeNull();
        result.ExtraHeaderKeys.ShouldBeEmpty();
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
        result.ShouldNotBeNull();
        result.BaseURL.ShouldBeNull();
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
        result.ShouldNotBeNull();
        result.BaseURL.ShouldBe(customBaseUrl);
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
        firstResult.Id.ShouldBe("conn-123");
        secondResult.Id.ShouldBe("conn-123");
        secondResult.Provider.ShouldBe("my-unique-provider");
        secondResult.CustomModels.ShouldHaveSingleItem();
        secondResult.WithDefaultModels.ShouldBeFalse();
        secondResult.UpdatedAt.ShouldBeGreaterThan(secondResult.CreatedAt);
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
        result.ShouldNotBeNull();
        result.CustomModels.Length.ShouldBe(2);
        result.WithDefaultModels.ShouldBeTrue();
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
        result.ShouldNotBeNull();
        result.Data.Length.ShouldBe(2);
        result.Data[0].Adapter.ShouldBe(LlmAdapter.GoogleVertexAi);
        result.Data[1].Adapter.ShouldBe(LlmAdapter.GoogleAiStudio);
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
        result.DisplaySecretKey.ShouldNotBeNull();
        result.DisplaySecretKey.ShouldContain("***");
        result.DisplaySecretKey.ShouldNotContain("very-long-secret-key");

        // Verify request body contained full secret
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        requestBody!.ShouldContain(secretKey);

        // Verify response body contains masked secret only
        var responseBody = await _httpHandler.GetLastResponseBodyAsync();
        responseBody!.ShouldNotContain(secretKey);
        responseBody!.ShouldContain("***");
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
        result.Meta.ShouldNotBeNull();
        result.Meta.Page.ShouldBe(2);
        result.Meta.Limit.ShouldBe(10);
        result.Meta.TotalItems.ShouldBe(25);
        result.Meta.TotalPages.ShouldBe(3);
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