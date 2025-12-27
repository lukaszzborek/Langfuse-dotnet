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
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
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
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Membership deleted successfully");
        result.UserId.ShouldBe("user-123");

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/organizations/memberships");
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
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"userId\"");
        requestBody.ShouldContain("\"user-456\"");
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
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"userId\":\"user-serialization-test\"");
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
        result.ShouldNotBeNull();
        result.Message.ShouldBe("User removed from organization");
        result.UserId.ShouldBe("user-789");
    }

    [Fact]
    public async Task DeleteOrganizationMembershipAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.DeleteOrganizationMembershipAsync(null!));
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
        var exception =
            await Should.ThrowAsync<LangfuseApiException>(async () =>
                await _client.DeleteOrganizationMembershipAsync(request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Forbidden);
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
        result.ShouldNotBeNull();
        result.Message.ShouldBe("Membership deleted successfully");
        result.UserId.ShouldBe("user-456");

        // Verify correct endpoint and method
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        var requestUri = _httpHandler.LastRequest?.RequestUri?.ToString();
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/projects/project-123/memberships");
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
        requestUri.ShouldNotBeNull();
        requestUri.ShouldContain("/api/public/projects/");
        requestUri.ShouldContain("project-with-special-chars");
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
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"userId\"");
        requestBody.ShouldContain("\"user-abc\"");
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
        requestBody.ShouldNotBeNull();
        requestBody.ShouldContain("\"userId\":\"user-serialize\"");
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
        result.ShouldNotBeNull();
        result.Message.ShouldBe("User removed from project");
        result.UserId.ShouldBe("user-deserialize");
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
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.DeleteProjectMembershipAsync(null!, request));
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
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.DeleteProjectMembershipAsync(string.Empty, request));
    }

    [Fact]
    public async Task DeleteProjectMembershipAsync_NullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.DeleteProjectMembershipAsync("project-123", null!));
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
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.DeleteProjectMembershipAsync("project-123", request));

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Forbidden);
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
        json.ShouldContain("\"userId\":\"user-serialize-test\"");
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
        response.ShouldNotBeNull();
        response.Message.ShouldBe("Successfully deleted");
        response.UserId.ShouldBe("user-test-123");
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
        response.ShouldNotBeNull();
        response.Message.ShouldBe("User removed from organization");
        response.UserId.ShouldBe("user-456");
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