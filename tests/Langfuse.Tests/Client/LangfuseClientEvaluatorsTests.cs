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
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientEvaluatorsTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientEvaluatorsTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        var config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    private static LlmAsJudgeEvaluator SampleEvaluator(string id = "ev-1")
    {
        return new LlmAsJudgeEvaluator
        {
            Id = id,
            Name = "helpfulness",
            Status = EvaluatorStatus.Active,
            EvaluationRuleAssignments = Array.Empty<EvaluationRuleAssignment>(),
            Prompt = new[]
                { new EvaluatorChatMessage { Role = EvaluatorChatMessageRole.User, Content = "Rate {{input}}" } },
            Variables = new[] { "input" },
            OutputDefinition = new NumericEvaluatorOutputDefinition(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VersionId = "ver-1",
            Version = 1,
            VersionCreatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task CreateEvaluatorAsync_LlmAsJudge_PostsToV2Endpoint_ReturnsEvaluator()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleEvaluator());

        var request = new CreateLlmAsJudgeEvaluatorRequest
        {
            Name = "helpfulness",
            Prompt = "Rate {{input}}",
            OutputDefinition = new NumericEvaluatorOutputDefinition()
        };

        var result = await _client.CreateEvaluatorAsync(request);

        result.ShouldBeOfType<LlmAsJudgeEvaluator>();
        result.Id.ShouldBe("ev-1");
        result.Type.ShouldBe(EvaluatorType.Llm_As_Judge);
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v2/evaluators");

        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        body.ShouldContain("\"type\":\"llm_as_judge\"");
        body.ShouldContain("\"prompt\":\"Rate {{input}}\"");
        body.ShouldContain("\"dataType\":\"NUMERIC\"");
    }

    [Fact]
    public async Task CreateEvaluatorAsync_Code_SendsCodeDiscriminator_ReturnsCodeEvaluator()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, @"{
            ""id"": ""ev-2"",
            ""name"": ""length-check"",
            ""description"": null,
            ""type"": ""code"",
            ""createdBy"": null,
            ""status"": ""active"",
            ""pausedAt"": null,
            ""pausedReason"": null,
            ""pausedMessage"": null,
            ""evaluationRuleAssignments"": [],
            ""sourceCode"": ""def evaluate(*, output, **kwargs): return len(output)"",
            ""sourceCodeLanguage"": ""PYTHON"",
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z"",
            ""versionId"": ""ver-1"",
            ""version"": 1,
            ""versionCreatedAt"": ""2024-01-01T00:00:00Z"",
            ""versionCreatedBy"": null
        }");

        var request = new CreateCodeEvaluatorRequest
        {
            Name = "length-check",
            SourceCode = "def evaluate(*, output, **kwargs): return len(output)",
            SourceCodeLanguage = CodeEvaluatorSourceCodeLanguage.Python
        };

        var result = await _client.CreateEvaluatorAsync(request);

        var codeEvaluator = result.ShouldBeOfType<CodeEvaluator>();
        codeEvaluator.Type.ShouldBe(EvaluatorType.Code);
        codeEvaluator.SourceCodeLanguage.ShouldBe(CodeEvaluatorSourceCodeLanguage.Python);

        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        body.ShouldContain("\"type\":\"code\"");
        body.ShouldContain("\"sourceCodeLanguage\":\"PYTHON\"");
    }

    [Fact]
    public async Task CreateEvaluatorAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.CreateEvaluatorAsync(null!));
    }

    [Fact]
    public async Task GetEvaluatorsAsync_NoPaging_GetsV2Endpoint()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new EvaluatorsPage
        {
            Data = new Evaluator[] { SampleEvaluator() },
            Meta = new CursorMeta { Cursor = "next" }
        });

        var result = await _client.GetEvaluatorsAsync();

        result.Data.Length.ShouldBe(1);
        result.Meta.Cursor.ShouldBe("next");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.PathAndQuery.ShouldBe("/api/public/v2/evaluators");
    }

    [Fact]
    public async Task GetEvaluatorsAsync_WithLimitAndCursor_BuildsQuery()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new EvaluatorsPage
        {
            Data = Array.Empty<Evaluator>(),
            Meta = new CursorMeta()
        });

        await _client.GetEvaluatorsAsync(10, "abc=");

        _httpHandler.LastRequest?.RequestUri?.PathAndQuery
            .ShouldBe("/api/public/v2/evaluators?limit=10&cursor=abc%3d");
    }

    [Fact]
    public async Task GetEvaluatorAsync_GetsById()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleEvaluator("ev/1"));

        var result = await _client.GetEvaluatorAsync("ev/1");

        result.Id.ShouldBe("ev/1");
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v2/evaluators/ev%2F1");
    }

    [Fact]
    public async Task GetEvaluatorAsync_NullId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () => await _client.GetEvaluatorAsync(" "));
    }

    [Fact]
    public async Task UpdateEvaluatorAsync_PatchesById()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleEvaluator());

        await _client.UpdateEvaluatorAsync("ev-1", new UpdateEvaluatorMetadataRequest { Name = "renamed" });

        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Patch);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v2/evaluators/ev-1");
        (await _httpHandler.GetLastRequestBodyAsync()).ShouldBe("{\"name\":\"renamed\"}");
    }

    [Fact]
    public async Task UpdateEvaluatorAsync_ExplicitNullDescription_SendsNullToClearIt()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleEvaluator());

        await _client.UpdateEvaluatorAsync("ev-1",
            new UpdateEvaluatorMetadataRequest { Name = "renamed", Description = null });

        (await _httpHandler.GetLastRequestBodyAsync()).ShouldBe("{\"name\":\"renamed\",\"description\":null}");
    }

    [Fact]
    public async Task UpdateEvaluatorAsync_DescriptionSet_SendsDescription()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleEvaluator());

        await _client.UpdateEvaluatorAsync("ev-1", new UpdateEvaluatorMetadataRequest { Description = "new" });

        (await _httpHandler.GetLastRequestBodyAsync()).ShouldBe("{\"description\":\"new\"}");
    }

    [Fact]
    public async Task UpdateEvaluatorAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.UpdateEvaluatorAsync("ev-1", null!));
    }

    [Fact]
    public async Task DeleteEvaluatorAsync_DeletesById_ReturnsConfirmation()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new DeletedEvaluator { Id = "ev-1" });

        var result = await _client.DeleteEvaluatorAsync("ev-1");

        result.Id.ShouldBe("ev-1");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v2/evaluators/ev-1");
    }

    [Fact]
    public async Task DeleteEvaluatorAsync_NullId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () => await _client.DeleteEvaluatorAsync(null!));
    }

    [Fact]
    public async Task GetEvaluatorVersionsAsync_GetsVersionsWithQuery()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, @"{ ""data"": [
            { ""id"": ""ver-1"", ""version"": 1, ""createdAt"": ""2024-01-01T00:00:00Z"", ""createdBy"": null,
              ""type"": ""code"", ""sourceCode"": ""x"", ""sourceCodeLanguage"": ""PYTHON"" }
        ], ""meta"": {} }");

        var result = await _client.GetEvaluatorVersionsAsync("ev-1", 5, "c");

        result.Data.Single().ShouldBeOfType<CodeEvaluatorVersion>().Version.ShouldBe(1);
        result.Meta.Cursor.ShouldBeNull();
        _httpHandler.LastRequest?.RequestUri?.PathAndQuery
            .ShouldBe("/api/public/v2/evaluators/ev-1/versions?limit=5&cursor=c");
    }

    [Fact]
    public async Task GetEvaluatorVersionsAsync_NullId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () => await _client.GetEvaluatorVersionsAsync(""));
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly List<HttpRequestMessage> _requests = new();
        private HttpResponseMessage? _response;

        public HttpRequestMessage? LastRequest => _requests.LastOrDefault();

        public void SetupResponse(HttpStatusCode statusCode, object responseBody)
        {
            var json = JsonSerializer.Serialize(responseBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            SetupJsonResponse(statusCode, json);
        }

        public void SetupJsonResponse(HttpStatusCode statusCode, string json)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
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