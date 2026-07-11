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
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    private static EvaluatorOutputDefinition SampleOutput()
    {
        return new EvaluatorOutputDefinition
        {
            DataType = EvaluatorOutputDataType.Numeric,
            Reasoning = new EvaluatorOutputFieldDefinition { Description = "why" },
            Score = new EvaluatorOutputScoreDefinition { Description = "0..1" }
        };
    }

    private static LlmAsJudgeEvaluator SampleEvaluator(string id = "ev-1")
    {
        return new LlmAsJudgeEvaluator
        {
            Id = id,
            Name = "helpfulness",
            Version = 1,
            Scope = EvaluatorScope.Project,
            Prompt = "Rate {{input}}",
            Variables = new[] { "input" },
            OutputDefinition = SampleOutput(),
            EvaluationRuleCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task CreateEvaluatorAsync_LlmAsJudge_PostsToUnstableEndpoint_ReturnsEvaluator()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleEvaluator());

        var request = new CreateLlmAsJudgeEvaluatorRequest
        {
            Name = "helpfulness",
            Prompt = "Rate {{input}}",
            OutputDefinition = SampleOutput()
        };

        var result = await _client.CreateEvaluatorAsync(request);

        result.ShouldNotBeNull();
        result.ShouldBeOfType<LlmAsJudgeEvaluator>();
        result.Id.ShouldBe("ev-1");
        result.Type.ShouldBe(EvaluatorType.Llm_As_Judge);
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluators");

        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        body.ShouldContain("\"type\":\"llm_as_judge\"");
        body.ShouldContain("\"dataType\":\"NUMERIC\"");
    }

    [Fact]
    public async Task CreateEvaluatorAsync_Code_SendsCodeDiscriminator_ReturnsCodeEvaluator()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, @"{
            ""id"": ""ev-2"",
            ""name"": ""length-check"",
            ""version"": 1,
            ""scope"": ""project"",
            ""type"": ""code"",
            ""variables"": [""input"", ""output""],
            ""sourceCode"": ""def evaluate(*, output, **kwargs): return len(output)"",
            ""sourceCodeLanguage"": ""PYTHON"",
            ""evaluationRuleCount"": 0,
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
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
        codeEvaluator.SourceCode.ShouldBe("def evaluate(*, output, **kwargs): return len(output)");
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
    public async Task GetEvaluatorsAsync_NoPaging_GetsUnstableEndpoint()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new PaginatedEvaluators
        {
            Data = new Evaluator[] { SampleEvaluator() },
            Meta = new ApiMetadata { Page = 1, Limit = 50, TotalItems = 1, TotalPages = 1 }
        });

        var result = await _client.GetEvaluatorsAsync();

        result.Data.Length.ShouldBe(1);
        result.Data[0].ShouldBeOfType<LlmAsJudgeEvaluator>();
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluators");
        _httpHandler.LastRequest?.RequestUri?.Query.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetEvaluatorsAsync_WithPaging_BuildsQuery()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new PaginatedEvaluators
        {
            Data = Array.Empty<Evaluator>(),
            Meta = new ApiMetadata { Page = 3, Limit = 5, TotalItems = 0, TotalPages = 0 }
        });

        await _client.GetEvaluatorsAsync(3, 5);

        var query = _httpHandler.LastRequest?.RequestUri?.Query;
        query.ShouldContain("page=3");
        query.ShouldContain("limit=5");
    }

    [Fact]
    public async Task GetEvaluatorAsync_GetsByIdOnUnstableEndpoint()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleEvaluator("ev-99"));

        var result = await _client.GetEvaluatorAsync("ev-99");

        result.Id.ShouldBe("ev-99");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluators/ev-99");
    }

    [Fact]
    public async Task GetEvaluatorAsync_NullId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.GetEvaluatorAsync(null!));
    }

    [Fact]
    public async Task DeleteEvaluatorAsync_DeletesUnstableEndpoint_ReturnsMessage()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK,
            new DeleteEvaluatorResponse { Message = "Evaluator successfully deleted" });

        var result = await _client.DeleteEvaluatorAsync("ev-5");

        result.Message.ShouldBe("Evaluator successfully deleted");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluators/ev-5");
    }

    [Fact]
    public async Task DeleteEvaluatorAsync_NullId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.DeleteEvaluatorAsync(null!));
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly List<HttpRequestMessage> _requests = new();
        private Exception? _exception;
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