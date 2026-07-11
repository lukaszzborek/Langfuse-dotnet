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
using zborek.Langfuse.Models.DashboardWidget;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientDashboardWidgetsTests
{
    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientDashboardWidgetsTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    private static DashboardWidget SampleWidget(string id = "widget-1")
    {
        return new DashboardWidget
        {
            Id = id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Latency by model",
            Description = "p95 latency per model",
            View = DashboardWidgetView.Observations,
            Dimensions = new[] { new DashboardWidgetDimension { Field = "providedModelName" } },
            Metrics = new[]
            {
                new DashboardWidgetMetric { Measure = "latency", Agg = DashboardWidgetMetricAggregation.P95 }
            },
            Filters = Array.Empty<DashboardWidgetFilter>(),
            ChartType = DashboardWidgetChartType.LineTimeSeries,
            ChartConfig = new DashboardWidgetChartConfig { Type = DashboardWidgetChartType.LineTimeSeries },
            MinVersion = 2
        };
    }

    private static CreateDashboardWidgetRequest SampleRequest()
    {
        return new CreateDashboardWidgetRequest
        {
            Name = "Score distribution",
            Description = "p95 score per name",
            View = DashboardWidgetView.ScoresNumeric,
            Dimensions = new[] { new DashboardWidgetDimension { Field = "name" } },
            Metrics = new[]
            {
                new DashboardWidgetMetric { Measure = "value", Agg = DashboardWidgetMetricAggregation.P95 }
            },
            Filters = new[]
            {
                new DashboardWidgetFilter
                {
                    Column = "environment",
                    Operator = "any of",
                    Type = "stringOptions",
                    Value = new[] { "production" }
                }
            },
            ChartType = DashboardWidgetChartType.PivotTable,
            ChartConfig = new DashboardWidgetChartConfig
            {
                Type = DashboardWidgetChartType.PivotTable,
                RowLimit = 10,
                DefaultSort = new DashboardWidgetDefaultSort
                {
                    Column = "value_p95",
                    Order = DashboardWidgetSortOrder.Desc
                }
            }
        };
    }

    [Fact]
    public async Task CreateDashboardWidgetAsync_PostsToUnstableEndpoint_ReturnsWidget()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleWidget());

        var result = await _client.CreateDashboardWidgetAsync(SampleRequest());

        result.ShouldNotBeNull();
        result.Id.ShouldBe("widget-1");
        result.View.ShouldBe(DashboardWidgetView.Observations);
        result.ChartType.ShouldBe(DashboardWidgetChartType.LineTimeSeries);
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath
            .ShouldBe("/api/public/unstable/dashboard-widgets");
    }

    [Fact]
    public async Task CreateDashboardWidgetAsync_SerializesEnumsToWireFormat()
    {
        _httpHandler.SetupResponse(HttpStatusCode.OK, SampleWidget());

        await _client.CreateDashboardWidgetAsync(SampleRequest());

        var body = await _httpHandler.GetLastRequestBodyAsync();
        body.ShouldNotBeNull();
        body.ShouldContain("\"view\":\"scores-numeric\"");
        body.ShouldContain("\"chartType\":\"PIVOT_TABLE\"");
        body.ShouldContain("\"agg\":\"p95\"");
        body.ShouldContain("\"order\":\"DESC\"");
        body.ShouldContain("\"row_limit\":10");
        body.ShouldNotContain("minVersion");
    }

    [Fact]
    public async Task CreateDashboardWidgetAsync_NullRequest_ThrowsArgumentNullException()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _client.CreateDashboardWidgetAsync(null!));
    }

    [Fact]
    public async Task CreateDashboardWidgetAsync_DeserializesRealisticPayload()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, """
                                                          {
                                                            "id": "cw-123",
                                                            "createdAt": "2026-07-01T10:00:00.000Z",
                                                            "updatedAt": "2026-07-02T11:30:00.000Z",
                                                            "name": "Score categories",
                                                            "description": "Categorical score counts",
                                                            "view": "scores-categorical",
                                                            "dimensions": [{ "field": "stringValue" }],
                                                            "metrics": [{ "measure": "count", "agg": "count" }],
                                                            "filters": [
                                                              {
                                                                "column": "metadata",
                                                                "operator": "contains",
                                                                "type": "stringObject",
                                                                "value": "beta",
                                                                "key": "cohort"
                                                              }
                                                            ],
                                                            "chartType": "VERTICAL_BAR",
                                                            "chartConfig": {
                                                              "type": "VERTICAL_BAR",
                                                              "row_limit": 50,
                                                              "show_value_labels": true,
                                                              "bins": null,
                                                              "defaultSort": null
                                                            },
                                                            "minVersion": 2
                                                          }
                                                          """);

        var result = await _client.CreateDashboardWidgetAsync(SampleRequest());

        result.Id.ShouldBe("cw-123");
        result.CreatedAt.ShouldBe(new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc));
        result.View.ShouldBe(DashboardWidgetView.ScoresCategorical);
        result.Dimensions.Length.ShouldBe(1);
        result.Dimensions[0].Field.ShouldBe("stringValue");
        result.Metrics[0].Agg.ShouldBe(DashboardWidgetMetricAggregation.Count);
        result.Filters[0].Key.ShouldBe("cohort");
        result.ChartType.ShouldBe(DashboardWidgetChartType.VerticalBar);
        result.ChartConfig.Type.ShouldBe(DashboardWidgetChartType.VerticalBar);
        result.ChartConfig.RowLimit.ShouldBe(50);
        result.ChartConfig.ShowValueLabels.ShouldBe(true);
        result.ChartConfig.Bins.ShouldBeNull();
        result.ChartConfig.DefaultSort.ShouldBeNull();
        result.MinVersion.ShouldBe(2);
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
