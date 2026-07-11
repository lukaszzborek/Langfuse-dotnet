using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.Media;

namespace zborek.Langfuse.Tests.Models;

public class MediaUploadRequestTests
{
    [Fact]
    public void Should_Serialize_TraceContext_And_Omit_DatasetFields()
    {
        var request = new MediaUploadRequest
        {
            TraceId = "trace-1",
            ObservationId = "obs-1",
            ContentType = "image/png",
            ContentLength = 1024,
            Sha256Hash = "abc123",
            Field = "input"
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"traceId\":\"trace-1\"");
        json.ShouldContain("\"observationId\":\"obs-1\"");
        json.ShouldContain("\"contentType\":\"image/png\"");
        json.ShouldContain("\"contentLength\":1024");
        json.ShouldContain("\"sha256Hash\":\"abc123\"");
        json.ShouldContain("\"field\":\"input\"");
        json.ShouldNotContain("datasetId");
        json.ShouldNotContain("datasetItemId");
    }

    [Fact]
    public void Should_Serialize_DatasetItemContext_And_Omit_TraceFields()
    {
        var request = new MediaUploadRequest
        {
            DatasetId = "ds-1",
            DatasetItemId = "item-1",
            ContentType = "image/png",
            ContentLength = 2048,
            Sha256Hash = "def456",
            Field = "expectedOutput"
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"datasetId\":\"ds-1\"");
        json.ShouldContain("\"datasetItemId\":\"item-1\"");
        json.ShouldContain("\"field\":\"expectedOutput\"");
        json.ShouldNotContain("traceId");
        json.ShouldNotContain("observationId");
    }

    [Fact]
    public void Should_Deserialize_DatasetItemContext()
    {
        var json = @"{
            ""datasetId"": ""ds-1"",
            ""datasetItemId"": ""item-1"",
            ""contentType"": ""image/png"",
            ""contentLength"": 2048,
            ""sha256Hash"": ""def456"",
            ""field"": ""expectedOutput""
        }";

        var request = JsonSerializer.Deserialize<MediaUploadRequest>(json);

        request.ShouldNotBeNull();
        request.TraceId.ShouldBeNull();
        request.ObservationId.ShouldBeNull();
        request.DatasetId.ShouldBe("ds-1");
        request.DatasetItemId.ShouldBe("item-1");
        request.ContentType.ShouldBe("image/png");
        request.ContentLength.ShouldBe(2048);
        request.Sha256Hash.ShouldBe("def456");
        request.Field.ShouldBe("expectedOutput");
    }
}