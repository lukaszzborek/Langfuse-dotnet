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
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientScoreV3Tests
{
    private const string EmptyResponseJson = """{ "data": [], "meta": { "limit": 50 } }""";

    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientScoreV3Tests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    [Fact]
    public async Task GetScoresV3Async_NoRequest_GetsV3EndpointWithoutQuery()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, EmptyResponseJson);

        var result = await _client.GetScoresV3Async();

        result.Data.ShouldBeEmpty();
        result.Meta.Limit.ShouldBe(50);
        result.Meta.Cursor.ShouldBeNull();
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Get);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v3/scores");
        _httpHandler.LastRequest?.RequestUri?.Query.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetScoresV3Async_WithRequest_BuildsAllQueryParameters()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, EmptyResponseJson);

        var request = new ScoreV3ListRequest
        {
            Limit = 25,
            Cursor = "bmV4dA",
            Fields = "details,subject,annotation",
            Id = "score-1,score-2",
            Name = "quality",
            Source = "API,EVAL",
            DataType = "NUMERIC",
            Environment = "production",
            ConfigId = "config-1",
            QueueId = "queue-1",
            AuthorUserId = "user-1",
            Value = "0.5,1",
            ValueMin = 0.25,
            ValueMax = 0.75,
            TraceId = "trace-1",
            ObservationId = "obs-1",
            FromTimestamp = new DateTime(2024, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            ToTimestamp = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        await _client.GetScoresV3Async(request);

        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/v3/scores");

        var query = Uri.UnescapeDataString(_httpHandler.LastRequest?.RequestUri?.Query ?? string.Empty);
        query.ShouldContain("limit=25");
        query.ShouldContain("cursor=bmV4dA");
        query.ShouldContain("fields=details,subject,annotation");
        query.ShouldContain("id=score-1,score-2");
        query.ShouldContain("name=quality");
        query.ShouldContain("source=API,EVAL");
        query.ShouldContain("dataType=NUMERIC");
        query.ShouldContain("environment=production");
        query.ShouldContain("configId=config-1");
        query.ShouldContain("queueId=queue-1");
        query.ShouldContain("authorUserId=user-1");
        query.ShouldContain("value=0.5,1");
        query.ShouldContain("valueMin=0.25");
        query.ShouldContain("valueMax=0.75");
        query.ShouldContain("traceId=trace-1");
        query.ShouldContain("observationId=obs-1");
        query.ShouldContain("fromTimestamp=2024-05-01T00:00:00.0000000Z");
        query.ShouldContain("toTimestamp=2024-06-01T00:00:00.0000000Z");
    }

    [Fact]
    public async Task GetScoresV3Async_MutuallyExclusiveSubjectFilters_ArePassedThrough()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, EmptyResponseJson);

        await _client.GetScoresV3Async(new ScoreV3ListRequest { SessionId = "session-1" });
        var query = _httpHandler.LastRequest?.RequestUri?.Query;
        query.ShouldContain("sessionId=session-1");

        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, EmptyResponseJson);
        await _client.GetScoresV3Async(new ScoreV3ListRequest { ExperimentId = "exp-1" });
        query = _httpHandler.LastRequest?.RequestUri?.Query;
        query.ShouldContain("experimentId=exp-1");
    }

    [Fact]
    public async Task GetScoresV3Async_DeserializesPolymorphicScoresAndSubjects()
    {
        // Discriminators (dataType, kind) are intentionally NOT the first JSON properties
        var json = """
                   {
                       "data": [
                           {
                               "id": "score-1",
                               "projectId": "proj-1",
                               "name": "accuracy",
                               "source": "EVAL",
                               "timestamp": "2024-05-01T10:00:00Z",
                               "environment": "production",
                               "createdAt": "2024-05-01T10:00:01Z",
                               "updatedAt": "2024-05-01T10:00:02Z",
                               "comment": "auto eval",
                               "configId": "config-1",
                               "subject": { "id": "obs-1", "traceId": "trace-1", "kind": "observation" },
                               "value": 0.92,
                               "dataType": "NUMERIC"
                           },
                           {
                               "id": "score-2",
                               "projectId": "proj-1",
                               "name": "passed",
                               "source": "API",
                               "timestamp": "2024-05-01T11:00:00Z",
                               "environment": "production",
                               "createdAt": "2024-05-01T11:00:01Z",
                               "updatedAt": "2024-05-01T11:00:02Z",
                               "subject": { "id": "exp-1", "kind": "experiment" },
                               "value": false,
                               "dataType": "BOOLEAN"
                           },
                           {
                               "id": "score-3",
                               "projectId": "proj-1",
                               "name": "correction",
                               "source": "ANNOTATION",
                               "timestamp": "2024-05-01T12:00:00Z",
                               "environment": "production",
                               "createdAt": "2024-05-01T12:00:01Z",
                               "updatedAt": "2024-05-01T12:00:02Z",
                               "authorUserId": "user-1",
                               "queueId": "queue-1",
                               "subject": { "id": "session-1", "kind": "session" },
                               "value": "fixed answer",
                               "dataType": "CORRECTION"
                           }
                       ],
                       "meta": { "limit": 3, "cursor": "bmV4dC1wYWdl" }
                   }
                   """;
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, json);

        var result = await _client.GetScoresV3Async(new ScoreV3ListRequest { Fields = "details,subject,annotation" });

        result.Data.Length.ShouldBe(3);

        var numeric = result.Data[0].ShouldBeOfType<NumericScoreV3>();
        numeric.Value.ShouldBe(0.92);
        numeric.Source.ShouldBe(ScoreSource.Eval);
        numeric.Comment.ShouldBe("auto eval");
        numeric.ConfigId.ShouldBe("config-1");
        var observationSubject = numeric.Subject.ShouldBeOfType<ScoreSubjectObservationV3>();
        observationSubject.Id.ShouldBe("obs-1");
        observationSubject.TraceId.ShouldBe("trace-1");

        var boolean = result.Data[1].ShouldBeOfType<BooleanScoreV3>();
        boolean.Value.ShouldBeFalse();
        boolean.Subject.ShouldBeOfType<ScoreSubjectExperimentV3>().Id.ShouldBe("exp-1");

        var correction = result.Data[2].ShouldBeOfType<CorrectionScoreV3>();
        correction.Value.ShouldBe("fixed answer");
        correction.AuthorUserId.ShouldBe("user-1");
        correction.QueueId.ShouldBe("queue-1");
        correction.Subject.ShouldBeOfType<ScoreSubjectSessionV3>().Id.ShouldBe("session-1");

        result.Meta.Limit.ShouldBe(3);
        result.Meta.Cursor.ShouldBe("bmV4dC1wYWdl");
    }

    [Fact]
    public async Task GetScoresV3Async_PartialRequest_OmitsNullParametersFromQueryString()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, EmptyResponseJson);

        await _client.GetScoresV3Async(new ScoreV3ListRequest
        {
            Name = "quality",
            Limit = 10
        });

        var query = _httpHandler.LastRequest?.RequestUri?.Query;
        query.ShouldBe("?limit=10&name=quality");
    }

    [Fact]
    public async Task GetScoresV3Async_SpecialCharacters_AreUrlEncoded()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, EmptyResponseJson);

        await _client.GetScoresV3Async(new ScoreV3ListRequest
        {
            Name = "quality score/v1&next=1",
            TraceId = "trace+id 123"
        });

        var query = _httpHandler.LastRequest?.RequestUri?.Query;
        query.ShouldContain("name=quality+score%2fv1%26next%3d1");
        query.ShouldContain("traceId=trace%2bid+123");
    }

    [Fact]
    public async Task GetScoresV3Async_Unauthorized_ThrowsException()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.Unauthorized, "\"Unauthorized\"");

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.GetScoresV3Async());

        exception.StatusCode.ShouldBe((int)HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetScoresV3Async_NotFound_ThrowsException()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.NotFound, "\"Not Found\"");

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await _client.GetScoresV3Async());

        exception.StatusCode.ShouldBe((int)HttpStatusCode.NotFound);
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