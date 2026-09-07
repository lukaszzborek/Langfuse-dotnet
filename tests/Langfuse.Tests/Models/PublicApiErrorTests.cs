using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Tests.Models;

public class PublicApiErrorTests
{
    [Fact]
    public void Should_Deserialize_Full_Envelope_With_Details_And_Issues()
    {
        const string json = """
                            {
                              "message": "Invalid request body",
                              "code": "invalid_body",
                              "details": {
                                "issues": [
                                  {
                                    "code": "invalid_type",
                                    "message": "Expected string, received number",
                                    "path": ["mapping", 0, "jsonPath"]
                                  }
                                ],
                                "limit": 100,
                                "remaining": 5,
                                "resetAt": "2026-01-01T00:00:00Z",
                                "retryAfterSeconds": 30
                              }
                            }
                            """;

        var error = JsonSerializer.Deserialize<PublicApiError>(json);

        error.ShouldNotBeNull();
        error.Message.ShouldBe("Invalid request body");
        error.Code.ShouldBe(PublicApiErrorCode.InvalidBody);
        error.Details.ShouldNotBeNull();
        error.Details.Limit.ShouldBe(100);
        error.Details.Remaining.ShouldBe(5);
        error.Details.RetryAfterSeconds.ShouldBe(30);
        error.Details.ResetAt.ShouldNotBeNull();
        error.Details.Issues.ShouldNotBeNull();
        error.Details.Issues!.Count.ShouldBe(1);

        var issue = error.Details.Issues[0];
        issue.Code.ShouldBe("invalid_type");
        issue.Message.ShouldBe("Expected string, received number");
        issue.Path.Count.ShouldBe(3);
    }

    [Theory]
    [InlineData(PublicApiErrorCode.AuthenticationFailed, "authentication_failed")]
    [InlineData(PublicApiErrorCode.AccessDenied, "access_denied")]
    [InlineData(PublicApiErrorCode.InvalidRequest, "invalid_request")]
    [InlineData(PublicApiErrorCode.InvalidQuery, "invalid_query")]
    [InlineData(PublicApiErrorCode.InvalidBody, "invalid_body")]
    [InlineData(PublicApiErrorCode.ResourceNotFound, "resource_not_found")]
    [InlineData(PublicApiErrorCode.Conflict, "conflict")]
    [InlineData(PublicApiErrorCode.RateLimited, "rate_limited")]
    [InlineData(PublicApiErrorCode.MethodNotAllowed, "method_not_allowed")]
    [InlineData(PublicApiErrorCode.InternalError, "internal_error")]
    public void Should_RoundTrip_Every_PublicApiErrorCode_Value(PublicApiErrorCode code, string wireValue)
    {
        var json = JsonSerializer.Serialize(code);
        json.ShouldBe($"\"{wireValue}\"");

        var deserialized = JsonSerializer.Deserialize<PublicApiErrorCode>(json);
        deserialized.ShouldBe(code);
    }

    [Fact]
    public void Should_Leave_Error_Null_For_NonJson_Body()
    {
        var exception = new LangfuseApiException(500, "boom", "<html>Internal Server Error</html>");

        exception.Error.ShouldBeNull();
        exception.ResponseBody.ShouldBe("<html>Internal Server Error</html>");
        exception.StatusCode.ShouldBe(500);
    }

    [Fact]
    public void Should_Leave_Error_Null_For_Unrelated_Json_Body()
    {
        var exception = new LangfuseApiException(404, "not found", """{"foo": "bar"}""");

        exception.Error.ShouldBeNull();
    }

    [Fact]
    public void Should_Populate_Error_When_Body_Matches_Envelope()
    {
        const string json = """
                            {
                              "message": "Resource not found",
                              "code": "resource_not_found"
                            }
                            """;

        var exception = new LangfuseApiException(404, "not found", json);

        exception.Error.ShouldNotBeNull();
        exception.Error!.Message.ShouldBe("Resource not found");
        exception.Error.Code.ShouldBe(PublicApiErrorCode.ResourceNotFound);
        exception.Error.Details.ShouldBeNull();
    }

    [Fact]
    public void Should_Leave_Error_Null_For_Null_Or_Empty_Body()
    {
        var exceptionWithNullBody = new LangfuseApiException(500, "boom");
        exceptionWithNullBody.Error.ShouldBeNull();

        var exceptionWithEmptyBody = new LangfuseApiException(500, "boom", "");
        exceptionWithEmptyBody.Error.ShouldBeNull();
    }
}