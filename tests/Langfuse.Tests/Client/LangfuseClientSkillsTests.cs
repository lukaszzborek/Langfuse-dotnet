using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Skill;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientSkillsTests
{
    private const string SkillVersionJson = """
                                            {
                                              "id": "sv-1",
                                              "createdAt": "2026-10-01T10:00:00Z",
                                              "updatedAt": "2026-10-02T10:00:00Z",
                                              "projectId": "project-1",
                                              "createdBy": "user-1",
                                              "name": "my-skill",
                                              "description": "Does things",
                                              "frontmatter": { "name": "my-skill" },
                                              "version": 3,
                                              "tags": ["tools"],
                                              "labels": ["production"],
                                              "commitMessage": null,
                                              "files": [
                                                {
                                                  "id": "file-1",
                                                  "path": "SKILL.md",
                                                  "blobId": "blob-1",
                                                  "sha256Hash": "abc+/=",
                                                  "contentType": "text/markdown",
                                                  "contentLength": 5000000000,
                                                  "content": "# My skill"
                                                }
                                              ]
                                            }
                                            """;

    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientSkillsTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, logger);
    }

    [Fact]
    public async Task GetSkillListAsync_WithoutRequest_CallsEndpointWithoutQuery()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.OK, """
                                                      {
                                                        "data": [
                                                          {
                                                            "name": "my-skill",
                                                            "description": "Does things",
                                                            "tags": ["tools"],
                                                            "latestVersion": 3,
                                                            "latestVersionCreatedAt": "2026-10-01T10:00:00Z",
                                                            "latestVersionLastUpdatedAt": "2026-10-02T10:00:00Z",
                                                            "productionVersion": null
                                                          }
                                                        ],
                                                        "meta": { "page": 1, "limit": 50, "totalItems": 1, "totalPages": 1, "hasNextPage": false }
                                                      }
                                                      """);

        // Act
        var result = await _client.GetSkillListAsync();

        // Assert
        result.Data.ShouldHaveSingleItem();
        result.Data[0].Name.ShouldBe("my-skill");
        result.Data[0].LatestVersion.ShouldBe(3);
        result.Data[0].ProductionVersion.ShouldBeNull();
        result.Meta.TotalItems.ShouldBe(1);
        result.Meta.HasNextPage.ShouldBeFalse();
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.PathAndQuery.ShouldBe("/api/public/skills");
    }

    [Fact]
    public async Task GetSkillListAsync_WithFilters_BuildsQueryString()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.OK, """
                                                      {
                                                        "data": [],
                                                        "meta": { "page": 2, "limit": 10, "totalItems": 0, "totalPages": 0, "hasNextPage": false }
                                                      }
                                                      """);

        // Act
        await _client.GetSkillListAsync(new SkillListRequest
        {
            Name = "my-skill",
            Search = "pdf tools",
            Tag = "tools",
            Page = 2,
            Limit = 10,
            FromUpdatedAt = new DateTimeOffset(2026, 10, 1, 0, 0, 0, TimeSpan.Zero)
        });

        // Assert
        var query = _httpHandler.LastRequest?.RequestUri?.Query;
        query.ShouldNotBeNull();
        query.ShouldContain("name=my-skill");
        query.ShouldContain("search=pdf+tools");
        query.ShouldContain("tag=tools");
        query.ShouldContain("page=2");
        query.ShouldContain("limit=10");
        query.ShouldContain("fromUpdatedAt=2026-10-01T00%3a00%3a00.0000000%2b00%3a00");
        query.ShouldNotContain("toUpdatedAt");
    }

    [Fact]
    public async Task GetSkillAsync_WithAllParameters_BuildsQueryAndDeserializesFiles()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.OK, SkillVersionJson);

        // Act
        var result = await _client.GetSkillAsync("my skill", 3, "production", true);

        // Assert
        result.Id.ShouldBe("sv-1");
        result.Version.ShouldBe(3);
        result.Labels.ShouldBe(["production"]);
        result.Frontmatter.ShouldNotBeNull();
        result.CommitMessage.ShouldBeNull();
        result.Files.ShouldHaveSingleItem();
        result.Files[0].Path.ShouldBe("SKILL.md");
        result.Files[0].ContentLength.ShouldBe(5000000000L);
        result.Files[0].Content.ShouldBe("# My skill");
        _httpHandler.LastRequest?.RequestUri?.PathAndQuery
            .ShouldBe("/api/public/skills/my%20skill?version=3&label=production&includeContent=true");
    }

    [Fact]
    public async Task GetSkillAsync_WithoutOptionalParameters_CallsEndpointWithoutQuery()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.OK, SkillVersionJson);

        // Act
        await _client.GetSkillAsync("my-skill");

        // Assert
        _httpHandler.LastRequest?.RequestUri?.PathAndQuery.ShouldBe("/api/public/skills/my-skill");
    }

    [Fact]
    public async Task GetSkillAsync_EmptyName_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () => await _client.GetSkillAsync(" "));
    }

    [Fact]
    public async Task CreateSkillVersionAsync_SerializesOnlyTheSetFileField()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.Created, SkillVersionJson);
        var request = new CreateSkillVersionRequest
        {
            Files =
            [
                new SkillVersionFileCreateInput { Path = "SKILL.md", Content = "# My skill" },
                new SkillVersionFileCreateInput { Path = "scripts/run.py", Sha256Hash = "abc+/=" }
            ],
            CommitMessage = "Initial version"
        };

        // Act
        var result = await _client.CreateSkillVersionAsync(request);

        // Assert
        result.Id.ShouldBe("sv-1");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/skills");
        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;
        root.GetProperty("commitMessage").GetString().ShouldBe("Initial version");

        var files = root.GetProperty("files");
        files[0].GetProperty("content").GetString().ShouldBe("# My skill");
        files[0].TryGetProperty("sha256Hash", out _).ShouldBeFalse();
        files[1].GetProperty("sha256Hash").GetString().ShouldBe("abc+/=");
        files[1].TryGetProperty("content", out _).ShouldBeFalse();
    }

    [Fact]
    public async Task CreateSkillVersionAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () => await _client.CreateSkillVersionAsync(null!));
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("# My skill", "abc+/=")]
    public async Task CreateSkillVersionAsync_FileWithoutExactlyOneSource_ThrowsArgumentException(string? content,
        string? sha256Hash)
    {
        var request = new CreateSkillVersionRequest
        {
            Files = [new SkillVersionFileCreateInput { Path = "SKILL.md", Content = content, Sha256Hash = sha256Hash }]
        };

        var exception = await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.CreateSkillVersionAsync(request));

        exception.Message.ShouldContain("SKILL.md");
    }

    [Fact]
    public async Task CreateSkillVersionAsync_NoFiles_ThrowsArgumentException()
    {
        var request = new CreateSkillVersionRequest { Files = [] };

        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.CreateSkillVersionAsync(request));
    }

    [Fact]
    public async Task UpdateSkillAsync_PatchesTags()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.OK, SkillVersionJson);

        // Act
        await _client.UpdateSkillAsync("my-skill", new UpdateSkillRequest { Tags = [] });

        // Assert
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Patch);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/skills/my-skill");
        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldBe("""{"tags":[]}""");
    }

    [Fact]
    public async Task UpdateSkillAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.UpdateSkillAsync("my-skill", null!));
    }

    [Fact]
    public async Task GetSkillFileContentsAsync_EncodesHashes()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.OK, """
                                                      { "data": [ { "sha256Hash": "abc+/=", "content": "# My skill" } ] }
                                                      """);

        // Act
        var result = await _client.GetSkillFileContentsAsync(["abc+/=", "def"]);

        // Assert
        result.Data.ShouldHaveSingleItem();
        result.Data[0].Content.ShouldBe("# My skill");
        _httpHandler.LastRequest?.RequestUri?.PathAndQuery
            .ShouldBe("/api/public/skills/files/content?sha256Hashes=abc%2b%2f%3d%2cdef");
    }

    [Fact]
    public async Task GetSkillFileContentsAsync_EmptyHashes_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () => await _client.GetSkillFileContentsAsync([]));
    }

    [Fact]
    public async Task GetSkillFileContentsAsync_MoreThan50Hashes_ThrowsArgumentException()
    {
        var hashes = Enumerable.Range(0, 51).Select(i => $"hash-{i}").ToList();

        await Should.ThrowAsync<ArgumentException>(async () => await _client.GetSkillFileContentsAsync(hashes));
    }

    [Fact]
    public async Task UpdateSkillVersionLabelsAsync_PatchesVersionLabels()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.OK, SkillVersionJson);

        // Act
        await _client.UpdateSkillVersionLabelsAsync("my-skill", 3,
            new UpdateSkillLabelsRequest { Labels = ["production"] });

        // Assert
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Patch);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/skills/my-skill/versions/3");
        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldBe("""{"labels":["production"]}""");
    }

    [Fact]
    public async Task DeleteSkillVersionAsync_ReturnsDeleted()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.OK, """{ "deleted": true }""");

        // Act
        var result = await _client.DeleteSkillVersionAsync("my-skill", 3);

        // Assert
        result.Deleted.ShouldBeTrue();
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/skills/my-skill/versions/3");
    }

    [Fact]
    public async Task DeleteSkillVersionAsync_NotFound_ThrowsLangfuseApiException()
    {
        // Arrange
        _httpHandler.SetupResponse(HttpStatusCode.NotFound, """{ "message": "Skill version not found" }""");

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.DeleteSkillVersionAsync("my-skill", 99));
        exception.StatusCode.ShouldBe(404);
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly List<HttpRequestMessage> _requests = new();
        private HttpResponseMessage? _response;

        public HttpRequestMessage? LastRequest => _requests.LastOrDefault();

        public void SetupResponse(HttpStatusCode statusCode, string responseBody)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(responseBody, Encoding.UTF8, "application/json")
            };
        }

        public async Task<string?> GetLastRequestBodyAsync()
        {
            if (LastRequest?.Content == null)
            {
                return null;
            }

            return await LastRequest.Content.ReadAsStringAsync();
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            _requests.Add(request);

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
