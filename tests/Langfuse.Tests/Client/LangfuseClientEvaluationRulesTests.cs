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
        var config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    private static EvaluationRule SampleRule(string id = "rule-1")
    {
        return new EvaluationRule
        {
            Id = id,
            Name = "My rule",
            Enabled = true,
            Sampling = 1.0,
            Filter = Array.Empty<EvaluationRuleReadFilter>(),
            EvaluatorAssignments = new[] { new EvaluatorAssignment { EvaluatorId = "ev-1" } },
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    [Fact]
    public async Task CreateEvaluationRuleAsync_PostsToV2Endpoint_ReturnsRule()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleRule());

        var request = new CreateEvaluationRuleRequest
        {
            Name = "My rule",
            Enabled = true,
            Sampling = 0.5,
            Filter = new[]
            {
                new StringOptionsEvaluationRuleFilter
                {
                    Column = "type",
                    Operator = EvaluationRuleOptionsFilterOperator.AnyOf,
                    Value = new[] { "GENERATION" }
                }
            },
            EvaluatorAssignments = new[] { new EvaluationRuleEvaluatorAssignmentInput { EvaluatorId = "ev-1" } }
        };

        var result = await _client.CreateEvaluationRuleAsync(request);

        result.Id.ShouldBe("rule-1");
        result.EvaluatorAssignments.Single().EvaluatorId.ShouldBe("ev-1");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v2/evaluation-rules");

        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        body.ShouldContain("\"sampling\":0.5");
        body.ShouldContain("\"operator\":\"any of\"");
        body.ShouldContain("\"evaluatorAssignments\":[{\"evaluatorId\":\"ev-1\"}]");
    }

    [Fact]
    public async Task CreateEvaluationRuleAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.CreateEvaluationRuleAsync(null!));
    }

    [Fact]
    public async Task GetEvaluationRulesAsync_WithLimitAndCursor_BuildsQuery()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new EvaluationRulesPage
        {
            Data = new[] { SampleRule() },
            Meta = new CursorMeta { Cursor = "next" }
        });

        var result = await _client.GetEvaluationRulesAsync(20, "cur");

        result.Data.Length.ShouldBe(1);
        result.Meta.Cursor.ShouldBe("next");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.PathAndQuery
            .ShouldBe("/api/public/v2/evaluation-rules?limit=20&cursor=cur");
    }

    [Fact]
    public async Task GetEvaluationRulesAsync_NoParams_NoQuery()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new EvaluationRulesPage
        {
            Data = Array.Empty<EvaluationRule>(),
            Meta = new CursorMeta()
        });

        await _client.GetEvaluationRulesAsync();

        _httpHandler.LastRequest?.RequestUri?.PathAndQuery.ShouldBe("/api/public/v2/evaluation-rules");
    }

    [Fact]
    public async Task GetEvaluationRuleAsync_GetsById()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleRule("rule 1"));

        var result = await _client.GetEvaluationRuleAsync("rule 1");

        result.Id.ShouldBe("rule 1");
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v2/evaluation-rules/rule%201");
    }

    [Fact]
    public async Task GetEvaluationRuleAsync_NullId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () => await _client.GetEvaluationRuleAsync(null!));
    }

    [Fact]
    public async Task UpdateEvaluationRuleAsync_PatchesById_OmitsNulls()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleRule());

        await _client.UpdateEvaluationRuleAsync("rule-1", new UpdateEvaluationRuleRequest
        {
            Enabled = false,
            EvaluatorAssignments = Array.Empty<EvaluationRuleEvaluatorAssignmentInput>()
        });

        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Patch);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v2/evaluation-rules/rule-1");
        (await _httpHandler.GetLastRequestBodyAsync()).ShouldBe("{\"enabled\":false,\"evaluatorAssignments\":[]}");
    }

    [Fact]
    public async Task UpdateEvaluationRuleAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.UpdateEvaluationRuleAsync("rule-1", null!));
    }

    [Fact]
    public async Task DeleteEvaluationRuleAsync_DeletesById_ReturnsConfirmation()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, new DeletedEvaluationRule { Id = "rule-1" });

        var result = await _client.DeleteEvaluationRuleAsync("rule-1");

        result.Id.ShouldBe("rule-1");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Delete);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v2/evaluation-rules/rule-1");
    }

    [Fact]
    public async Task DeleteEvaluationRuleAsync_NullId_ThrowsArgumentException()
    {
        await Should.ThrowAsync<ArgumentException>(async () => await _client.DeleteEvaluationRuleAsync(""));
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