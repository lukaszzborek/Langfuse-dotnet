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

public class LangfuseClientEvaluationRulesTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientEvaluationRulesTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    private static EvaluationRule SampleRule(string id = "rule-1") => new()
    {
        Id = id,
        Name = "My rule",
        Evaluator = new EvaluationRuleEvaluator { Id = "ev-1", Name = "helpfulness", Scope = EvaluatorScope.Project },
        Target = EvaluationRuleTarget.Observation,
        Enabled = true,
        Status = EvaluationRuleStatus.Active,
        Sampling = 1.0,
        Filter = Array.Empty<EvaluationRuleFilter>(),
        Mapping = new[]
        {
            new EvaluationRuleMapping { Variable = "input", Source = EvaluationRuleMappingSource.Input }
        },
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task CreateEvaluationRuleAsync_PostsToUnstableEndpoint_ReturnsRule()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleRule());

        var request = new CreateEvaluationRuleRequest
        {
            Name = "My rule",
            Evaluator = new EvaluationRuleEvaluatorReference { Name = "helpfulness", Scope = EvaluatorScope.Project },
            Target = EvaluationRuleTarget.Observation,
            Enabled = true,
            Mapping = new[]
            {
                new EvaluationRuleMapping { Variable = "input", Source = EvaluationRuleMappingSource.Input }
            }
        };

        var result = await _client.CreateEvaluationRuleAsync(request);

        result.ShouldNotBeNull();
        result.Id.ShouldBe("rule-1");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluation-rules");

        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        body.ShouldContain("\"target\":\"observation\"");
        body.ShouldContain("\"source\":\"input\"");
    }

    [Fact]
    public async Task CreateEvaluationRuleAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.CreateEvaluationRuleAsync(null!));
    }

    [Fact]
    public async Task GetEvaluationRulesAsync_NoPaging_GetsUnstableEndpoint()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new PaginatedEvaluationRules
        {
            Data = new[] { SampleRule() },
            Meta = new ApiMetadata { Page = 1, Limit = 50, TotalItems = 1, TotalPages = 1 }
        });

        var result = await _client.GetEvaluationRulesAsync();

        result.Data.Length.ShouldBe(1);
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluation-rules");
        _httpHandler.LastRequest?.RequestUri?.Query.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetEvaluationRulesAsync_WithPaging_BuildsQuery()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new PaginatedEvaluationRules
        {
            Data = Array.Empty<EvaluationRule>(),
            Meta = new ApiMetadata { Page = 2, Limit = 10, TotalItems = 0, TotalPages = 0 }
        });

        await _client.GetEvaluationRulesAsync(2, 10);

        var query = _httpHandler.LastRequest?.RequestUri?.Query;
        query.ShouldContain("page=2");
        query.ShouldContain("limit=10");
    }

    [Fact]
    public async Task GetEvaluationRuleAsync_GetsByIdOnUnstableEndpoint()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleRule("rule-42"));

        var result = await _client.GetEvaluationRuleAsync("rule-42");

        result.Id.ShouldBe("rule-42");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluation-rules/rule-42");
    }

    [Fact]
    public async Task GetEvaluationRuleAsync_NullId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _client.GetEvaluationRuleAsync(null!));
    }

    [Fact]
    public async Task UpdateEvaluationRuleAsync_PatchesUnstableEndpoint()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleRule("rule-7"));

        var result = await _client.UpdateEvaluationRuleAsync("rule-7",
            new UpdateEvaluationRuleRequest { Enabled = false });

        result.Id.ShouldBe("rule-7");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Patch);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluation-rules/rule-7");
    }

    [Fact]
    public async Task DeleteEvaluationRuleAsync_DeletesUnstableEndpoint_ReturnsMessage()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK,
            new DeleteEvaluationRuleResponse { Message = "deleted" });

        var result = await _client.DeleteEvaluationRuleAsync("rule-9");

        result.Message.ShouldBe("deleted");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/evaluation-rules/rule-9");
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
