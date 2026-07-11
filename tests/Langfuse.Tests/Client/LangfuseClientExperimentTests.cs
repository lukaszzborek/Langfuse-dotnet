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
using zborek.Langfuse.Models.Experiment;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientExperimentTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientExperimentTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    [Fact]
    public async Task GetExperimentsAsync_BuildsUrlAndQueryParams()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, """{"data":[],"meta":{}}""");

        var request = new ExperimentListRequest
        {
            Fields = "core,metadata,scores",
            Limit = 10,
            ScoreLimit = 5,
            Cursor = "djEuY3Vyc29y",
            FromStartTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ToStartTime = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            Id = "exp-1,exp-2",
            Name = "prompt-v2-eval",
            DatasetId = "ds-1",
            Filter = """[{"column":"name","operator":"=","value":"prompt-v2-eval"}]"""
        };

        await _client.GetExperimentsAsync(request);

        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/experiments");

        var decodedQuery = Uri.UnescapeDataString(_httpHandler.LastRequest!.RequestUri!.Query);
        decodedQuery.ShouldContain("fields=core,metadata,scores");
        decodedQuery.ShouldContain("limit=10");
        decodedQuery.ShouldContain("scoreLimit=5");
        decodedQuery.ShouldContain("cursor=djEuY3Vyc29y");
        decodedQuery.ShouldContain("fromStartTime=2026-01-01T00:00:00.0000000Z");
        decodedQuery.ShouldContain("toStartTime=2026-02-01T00:00:00.0000000Z");
        decodedQuery.ShouldContain("id=exp-1,exp-2");
        decodedQuery.ShouldContain("name=prompt-v2-eval");
        decodedQuery.ShouldContain("datasetId=ds-1");
        decodedQuery.ShouldContain("""filter=[{"column":"name","operator":"=","value":"prompt-v2-eval"}]""");
    }

    [Fact]
    public async Task GetExperimentsAsync_MinimalRequest_SendsOnlyFromStartTime()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, """{"data":[],"meta":{}}""");

        var request = new ExperimentListRequest
        {
            FromStartTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        await _client.GetExperimentsAsync(request);

        var decodedQuery = Uri.UnescapeDataString(_httpHandler.LastRequest!.RequestUri!.Query);
        decodedQuery.ShouldBe("?fromStartTime=2026-01-01T00:00:00.0000000Z");
    }

    [Fact]
    public async Task GetExperimentsAsync_DeserializesResponse()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, """
                                                          {
                                                            "data": [
                                                              {
                                                                "id": "exp-1",
                                                                "name": "prompt-v2-eval",
                                                                "description": "Evaluates prompt v2 against the golden dataset",
                                                                "startTime": "2026-01-05T10:00:00.000Z",
                                                                "endTime": "2026-01-05T10:15:30.000Z",
                                                                "itemCount": 42,
                                                                "datasetId": "ds-1",
                                                                "metadata": { "model": "gpt-4o" },
                                                                "scores": []
                                                              },
                                                              {
                                                                "id": "exp-2",
                                                                "name": "adhoc-run",
                                                                "description": null,
                                                                "startTime": "2026-01-06T08:00:00.000Z",
                                                                "endTime": "2026-01-06T08:01:00.000Z",
                                                                "itemCount": 3,
                                                                "datasetId": null
                                                              }
                                                            ],
                                                            "meta": { "cursor": "djEuMTcwNTQ4" }
                                                          }
                                                          """);

        var result = await _client.GetExperimentsAsync(new ExperimentListRequest
        {
            FromStartTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        result.Data.Length.ShouldBe(2);

        var first = result.Data[0];
        first.Id.ShouldBe("exp-1");
        first.Name.ShouldBe("prompt-v2-eval");
        first.Description.ShouldBe("Evaluates prompt v2 against the golden dataset");
        first.StartTime.ShouldBe(new DateTime(2026, 1, 5, 10, 0, 0, DateTimeKind.Utc));
        first.EndTime.ShouldBe(new DateTime(2026, 1, 5, 10, 15, 30, DateTimeKind.Utc));
        first.ItemCount.ShouldBe(42);
        first.DatasetId.ShouldBe("ds-1");
        first.Metadata.ShouldNotBeNull();
        first.Scores.ShouldNotBeNull();
        first.Scores.ShouldBeEmpty();

        var second = result.Data[1];
        second.Description.ShouldBeNull();
        second.DatasetId.ShouldBeNull();
        second.Metadata.ShouldBeNull();
        second.Scores.ShouldBeNull();

        result.Meta.Cursor.ShouldBe("djEuMTcwNTQ4");
    }

    [Fact]
    public async Task GetExperimentsAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.GetExperimentsAsync(null!));
    }

    [Fact]
    public async Task GetExperimentItemsAsync_BuildsUrlAndQueryParams()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, """{"data":[],"meta":{}}""");

        var request = new ExperimentItemListRequest
        {
            Fields = "core,dataset,io,scores",
            Limit = 25,
            ScoreLimit = 10,
            Cursor = "djEuY3Vyc29y",
            FromStartTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ToStartTime = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc),
            ExperimentId = "exp-1",
            ExperimentName = "prompt-v2-eval",
            ExperimentItemId = "item-1,item-2",
            DatasetId = "ds-1",
            Filter = """[{"column":"experimentId","operator":"=","value":"exp-1"}]"""
        };

        await _client.GetExperimentItemsAsync(request);

        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/experiment-items");

        var decodedQuery = Uri.UnescapeDataString(_httpHandler.LastRequest!.RequestUri!.Query);
        decodedQuery.ShouldContain("fields=core,dataset,io,scores");
        decodedQuery.ShouldContain("limit=25");
        decodedQuery.ShouldContain("scoreLimit=10");
        decodedQuery.ShouldContain("cursor=djEuY3Vyc29y");
        decodedQuery.ShouldContain("fromStartTime=2026-01-01T00:00:00.0000000Z");
        decodedQuery.ShouldContain("toStartTime=2026-02-01T00:00:00.0000000Z");
        decodedQuery.ShouldContain("experimentId=exp-1");
        decodedQuery.ShouldContain("experimentName=prompt-v2-eval");
        decodedQuery.ShouldContain("experimentItemId=item-1,item-2");
        decodedQuery.ShouldContain("datasetId=ds-1");
        decodedQuery.ShouldContain("""filter=[{"column":"experimentId","operator":"=","value":"exp-1"}]""");
    }

    [Fact]
    public async Task GetExperimentItemsAsync_DeserializesResponse()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, """
                                                          {
                                                            "data": [
                                                              {
                                                                "id": "run-item-1",
                                                                "traceId": "trace-1",
                                                                "startTime": "2026-01-05T10:00:00.000Z",
                                                                "endTime": "2026-01-05T10:00:05.000Z",
                                                                "level": "DEFAULT",
                                                                "environment": "production",
                                                                "experimentId": "exp-1",
                                                                "experimentName": "prompt-v2-eval",
                                                                "experimentItemId": "item-1",
                                                                "experimentDatasetId": "ds-1",
                                                                "experimentItemVersion": "2026-01-04T00:00:00.000Z",
                                                                "input": { "question": "What is Langfuse?" },
                                                                "output": "Langfuse is an LLM observability platform.",
                                                                "expectedOutput": "An observability platform for LLM applications.",
                                                                "metadata": { "region": "eu" },
                                                                "experimentItemMetadata": { "attempt": 1 },
                                                                "experimentMetadata": { "promptVersion": 2 },
                                                                "experimentDescription": "Evaluates prompt v2",
                                                                "scores": []
                                                              },
                                                              {
                                                                "id": "run-item-2",
                                                                "traceId": "trace-2",
                                                                "startTime": "2026-01-05T10:01:00.000Z",
                                                                "endTime": null,
                                                                "level": "ERROR",
                                                                "environment": "default",
                                                                "experimentId": "exp-1",
                                                                "experimentName": "prompt-v2-eval",
                                                                "experimentItemId": "item-2"
                                                              }
                                                            ],
                                                            "meta": {}
                                                          }
                                                          """);

        var result = await _client.GetExperimentItemsAsync(new ExperimentItemListRequest
        {
            FromStartTime = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        result.Data.Length.ShouldBe(2);

        var first = result.Data[0];
        first.Id.ShouldBe("run-item-1");
        first.TraceId.ShouldBe("trace-1");
        first.StartTime.ShouldBe(new DateTime(2026, 1, 5, 10, 0, 0, DateTimeKind.Utc));
        first.EndTime.ShouldBe(new DateTime(2026, 1, 5, 10, 0, 5, DateTimeKind.Utc));
        first.Level.ShouldBe(LangfuseLogLevel.Default);
        first.Environment.ShouldBe("production");
        first.ExperimentId.ShouldBe("exp-1");
        first.ExperimentName.ShouldBe("prompt-v2-eval");
        first.ExperimentItemId.ShouldBe("item-1");
        first.ExperimentDatasetId.ShouldBe("ds-1");
        first.ExperimentItemVersion.ShouldBe(new DateTime(2026, 1, 4, 0, 0, 0, DateTimeKind.Utc));
        first.Input.ShouldNotBeNull();
        first.Output.ShouldNotBeNull();
        first.ExpectedOutput.ShouldNotBeNull();
        first.Metadata.ShouldNotBeNull();
        first.ExperimentItemMetadata.ShouldNotBeNull();
        first.ExperimentMetadata.ShouldNotBeNull();
        first.ExperimentDescription.ShouldBe("Evaluates prompt v2");
        first.Scores.ShouldNotBeNull();
        first.Scores.ShouldBeEmpty();

        var second = result.Data[1];
        second.EndTime.ShouldBeNull();
        second.Level.ShouldBe(LangfuseLogLevel.Error);
        second.ExperimentDatasetId.ShouldBeNull();
        second.Scores.ShouldBeNull();

        result.Meta.Cursor.ShouldBeNull();
    }

    [Fact]
    public async Task GetExperimentItemsAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.GetExperimentItemsAsync(null!));
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