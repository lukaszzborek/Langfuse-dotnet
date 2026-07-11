using System.Net;
using System.Text;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using zborek.Langfuse.Client;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Media;

namespace zborek.Langfuse.Tests.Client;

public class LangfuseClientMediaTests
{
    private const string UploadResponseJson = """{ "mediaId": "media-1", "uploadUrl": "https://upload.test" }""";

    private readonly LangfuseClient _client;
    private readonly TestHttpMessageHandler _httpHandler;

    public LangfuseClientMediaTests()
    {
        _httpHandler = new TestHttpMessageHandler();
        var httpClient = new HttpClient(_httpHandler) { BaseAddress = new Uri("https://api.test.com/") };
        var channel = Channel.CreateUnbounded<IIngestionEvent>();
        IOptions<LangfuseConfig> config = Options.Create(new LangfuseConfig());
        var logger = Substitute.For<ILogger<LangfuseClient>>();

        _client = new LangfuseClient(httpClient, channel, config, logger);
    }

    private static MediaUploadRequest CreateRequest(string? traceId = null, string? observationId = null,
        string? datasetId = null, string? datasetItemId = null)
    {
        return new MediaUploadRequest
        {
            TraceId = traceId,
            ObservationId = observationId,
            DatasetId = datasetId,
            DatasetItemId = datasetItemId,
            ContentType = "image/png",
            ContentLength = 1024,
            Sha256Hash = "abc123",
            Field = "input"
        };
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_TraceContext_SendsRequest()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, UploadResponseJson);

        var result = await _client.GetMediaUploadUrlAsync(CreateRequest(traceId: "trace-1", observationId: "obs-1"));

        result.MediaId.ShouldBe("media-1");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/media");
    }

    [Fact]
    public async Task GetMediaUploadUrlAsync_DatasetItemContext_SendsRequest()
    {
        _httpHandler.SetupJsonResponse(HttpStatusCode.OK, UploadResponseJson);

        var result = await _client.GetMediaUploadUrlAsync(CreateRequest(datasetId: "ds-1", datasetItemId: "item-1"));

        result.MediaId.ShouldBe("media-1");
        _httpHandler.LastRequest?.Method.ShouldBe(HttpMethod.Post);
        _httpHandler.LastRequest?.RequestUri?.AbsolutePath.ShouldBe("/api/public/media");
    }

    [Theory]
    [InlineData(null, null, null, null)] // no context at all
    [InlineData("trace-1", null, "ds-1", "item-1")] // both contexts
    [InlineData("trace-1", null, "ds-1", null)] // trace mixed with dataset field
    [InlineData("trace-1", null, null, "item-1")] // trace mixed with dataset item field
    [InlineData(null, null, "ds-1", null)] // dataset id without item id
    [InlineData(null, null, null, "item-1")] // dataset item id without dataset id
    [InlineData(null, "obs-1", "ds-1", "item-1")] // observation id with dataset context
    [InlineData(null, "obs-1", null, null)] // observation id without trace id
    public async Task GetMediaUploadUrlAsync_InvalidContext_ThrowsArgumentException(string? traceId,
        string? observationId, string? datasetId, string? datasetItemId)
    {
        var request = CreateRequest(traceId, observationId, datasetId, datasetItemId);

        var exception = await Should.ThrowAsync<ArgumentException>(() => _client.GetMediaUploadUrlAsync(request));

        exception.Message.ShouldContain("exactly one context");
        _httpHandler.LastRequest.ShouldBeNull();
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly List<HttpRequestMessage> _requests = new();
        private HttpResponseMessage? _response;

        public HttpRequestMessage? LastRequest => _requests.LastOrDefault();

        public void SetupJsonResponse(HttpStatusCode statusCode, string json)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
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
