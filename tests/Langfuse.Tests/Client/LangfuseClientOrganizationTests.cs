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
using zborek.Langfuse.Models.Organization;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientOrganizationTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientOrganizationTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        var config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    [Fact]
    public async Task DeleteOrganizationMembershipAsync_Success()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-123"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "Membership deleted successfully",
            UserId = "user-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.DeleteOrganizationMembershipAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Membership deleted successfully", result.Message);
        Assert.Equal("user-123", result.UserId);

        // Verify correct endpoint and method
        Assert.Equal(HttpMethod.Delete, _httpHandler.LastRequest?.Method);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("/api/public/organizations/memberships", requestUri);
    }

    [Fact]
    public async Task DeleteOrganizationMembershipAsync_RequestBodySentWithDelete()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-456"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "Success",
            UserId = "user-456"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.DeleteOrganizationMembershipAsync(request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"userId\"", requestBody);
        Assert.Contains("\"user-456\"", requestBody);
    }

    [Fact]
    public async Task DeleteOrganizationMembershipAsync_RequestSerialization_Success()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-serialization-test"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "Deleted",
            UserId = "user-serialization-test"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.DeleteOrganizationMembershipAsync(request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"userId\":\"user-serialization-test\"", requestBody);
    }

    [Fact]
    public async Task DeleteOrganizationMembershipAsync_ResponseDeserialization_Success()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-789"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "User removed from organization",
            UserId = "user-789"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.DeleteOrganizationMembershipAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User removed from organization", result.Message);
        Assert.Equal("user-789", result.UserId);
    }

    [Fact]
    public async Task DeleteOrganizationMembershipAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _client.DeleteOrganizationMembershipAsync(null!));
    }

    [Fact]
    public async Task DeleteOrganizationMembershipAsync_Forbidden_ThrowsException()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Forbidden, "Access forbidden");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.DeleteOrganizationMembershipAsync(request));

        Assert.Equal((int)HttpStatusCode.Forbidden, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_Success()
    {
        // Arrange
        var projectId = "project-123";
        var request = new DeleteMembershipRequest
        {
            UserId = "user-456"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "Membership deleted successfully",
            UserId = "user-456"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.DeleteProjectMembershipAsync(projectId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Membership deleted successfully", result.Message);
        Assert.Equal("user-456", result.UserId);

        // Verify correct endpoint and method
        Assert.Equal(HttpMethod.Delete, _httpHandler.LastRequest?.Method);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("/api/public/projects/project-123/memberships", requestUri);
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_ProjectIdIsUrlEncoded()
    {
        // Arrange
        var projectId = "project-with-special-chars!@#";
        var request = new DeleteMembershipRequest
        {
            UserId = "user-789"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "Success",
            UserId = "user-789"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.DeleteProjectMembershipAsync(projectId, request);

        // Assert
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        Assert.NotNull(requestUri);
        Assert.Contains("/api/public/projects/", requestUri);
        Assert.Contains("project-with-special-chars", requestUri);
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_RequestBodySentWithDelete()
    {
        // Arrange
        var projectId = "project-xyz";
        var request = new DeleteMembershipRequest
        {
            UserId = "user-abc"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "Deleted",
            UserId = "user-abc"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.DeleteProjectMembershipAsync(projectId, request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"userId\"", requestBody);
        Assert.Contains("\"user-abc\"", requestBody);
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_RequestSerialization_Success()
    {
        // Arrange
        var projectId = "project-serialize";
        var request = new DeleteMembershipRequest
        {
            UserId = "user-serialize"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "OK",
            UserId = "user-serialize"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        await _client.DeleteProjectMembershipAsync(projectId, request);

        // Assert
        var requestBody = await _httpHandler.GetLastRequestBodyAsync();
        Assert.NotNull(requestBody);
        Assert.Contains("\"userId\":\"user-serialize\"", requestBody);
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_ResponseDeserialization_Success()
    {
        // Arrange
        var projectId = "project-deserialize";
        var request = new DeleteMembershipRequest
        {
            UserId = "user-deserialize"
        };

        var expectedResponse = new MembershipDeletionResponse
        {
            Message = "User removed from project",
            UserId = "user-deserialize"
        };

        _httpHandler.SetupResponse(HttpStatusCode.OK, expectedResponse);

        // Act
        var result = await _client.DeleteProjectMembershipAsync(projectId, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("User removed from project", result.Message);
        Assert.Equal("user-deserialize", result.UserId);
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_NullProjectId_ThrowsArgumentException()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.DeleteProjectMembershipAsync(null!, request));
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_EmptyProjectId_ThrowsArgumentException()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _client.DeleteProjectMembershipAsync(string.Empty, request));
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(
            async () => await _client.DeleteProjectMembershipAsync("project-123", null!));
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_Forbidden_ThrowsException()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-123"
        };

        _httpHandler.SetupResponse(HttpStatusCode.Forbidden, "Access forbidden");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(
            async () => await _client.DeleteProjectMembershipAsync("project-123", request));

        Assert.Equal((int)HttpStatusCode.Forbidden, exception.StatusCode);
    }

    [Fact]
    public void DeleteMembershipRequest_Serialization()
    {
        // Arrange
        var request = new DeleteMembershipRequest
        {
            UserId = "user-serialize-test"
        };

        // Act
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains("\"userId\":\"user-serialize-test\"", json);
    }

    [Fact]
    public void MembershipDeletionResponse_Deserialization()
    {
        // Arrange
        var json = "{\"message\":\"Successfully deleted\",\"userId\":\"user-test-123\"}";

        // Act
        var response = JsonSerializer.Deserialize<MembershipDeletionResponse>(json,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        // Assert
        Assert.NotNull(response);
        Assert.Equal("Successfully deleted", response.Message);
        Assert.Equal("user-test-123", response.UserId);
    }

    [Fact]
    public void MembershipDeletionResponse_DeserializationWithAllFields()
    {
        // Arrange
        var json = "{\"message\":\"User removed from organization\",\"userId\":\"user-456\"}";

        // Act
        var response = JsonSerializer.Deserialize<MembershipDeletionResponse>(json,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        // Assert
        Assert.NotNull(response);
        Assert.Equal("User removed from organization", response.Message);
        Assert.Equal("user-456", response.UserId);
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
