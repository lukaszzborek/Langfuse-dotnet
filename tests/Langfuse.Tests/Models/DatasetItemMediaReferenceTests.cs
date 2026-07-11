using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.Dataset;

namespace zborek.Langfuse.Tests.Models;

public class DatasetItemMediaReferenceTests
{
    [Fact]
    public void Should_Deserialize_DatasetItem_With_MediaReferences()
    {
        var json = @"{
            ""id"": ""item-1"",
            ""status"": ""ACTIVE"",
            ""input"": { ""image"": ""@@@langfuseMedia:type=image/png|id=media-1|source=bytes@@@"" },
            ""expectedOutput"": ""a cat"",
            ""metadata"": null,
            ""sourceTraceId"": null,
            ""sourceObservationId"": null,
            ""datasetId"": ""ds-1"",
            ""datasetName"": ""my-dataset"",
            ""createdAt"": ""2026-01-01T00:00:00Z"",
            ""updatedAt"": ""2026-01-02T00:00:00Z"",
            ""mediaReferences"": [
                {
                    ""field"": ""input"",
                    ""referenceString"": ""@@@langfuseMedia:type=image/png|id=media-1|source=bytes@@@"",
                    ""jsonPath"": ""$['image']"",
                    ""media"": {
                        ""mediaId"": ""media-1"",
                        ""contentType"": ""image/png"",
                        ""contentLength"": 12345,
                        ""url"": ""https://example.com/media-1"",
                        ""urlExpiry"": ""2026-01-03T00:00:00Z""
                    }
                },
                {
                    ""field"": ""expectedOutput"",
                    ""referenceString"": ""@@@langfuseMedia:type=audio/mpeg|id=media-2|source=bytes@@@"",
                    ""jsonPath"": ""$['audio']"",
                    ""media"": {
                        ""mediaId"": ""media-2"",
                        ""contentType"": ""audio/mpeg"",
                        ""contentLength"": 678,
                        ""url"": ""https://example.com/media-2"",
                        ""urlExpiry"": ""2026-01-04T00:00:00Z""
                    }
                }
            ]
        }";

        var item = JsonSerializer.Deserialize<DatasetItem>(json);

        item.ShouldNotBeNull();
        item.Id.ShouldBe("item-1");
        item.MediaReferences.ShouldNotBeNull();
        item.MediaReferences.Length.ShouldBe(2);

        var first = item.MediaReferences[0];
        first.Field.ShouldBe(DatasetItemMediaReferenceField.Input);
        first.ReferenceString.ShouldBe("@@@langfuseMedia:type=image/png|id=media-1|source=bytes@@@");
        first.JsonPath.ShouldBe("$['image']");
        first.Media.MediaId.ShouldBe("media-1");
        first.Media.ContentType.ShouldBe("image/png");
        first.Media.ContentLength.ShouldBe(12345);
        first.Media.Url.ShouldBe("https://example.com/media-1");
        first.Media.UrlExpiry.ShouldBe("2026-01-03T00:00:00Z");

        var second = item.MediaReferences[1];
        second.Field.ShouldBe(DatasetItemMediaReferenceField.ExpectedOutput);
        second.Media.MediaId.ShouldBe("media-2");
    }

    [Fact]
    public void Should_RoundTrip_DatasetItem_MediaReferences()
    {
        var item = new DatasetItem
        {
            Id = "item-1",
            DatasetId = "ds-1",
            DatasetName = "my-dataset",
            MediaReferences = new[]
            {
                new DatasetItemMediaReference
                {
                    Field = DatasetItemMediaReferenceField.ExpectedOutput,
                    ReferenceString = "@@@langfuseMedia:type=image/png|id=media-1|source=bytes@@@",
                    JsonPath = "$['image']",
                    Media = new DatasetItemMediaReferenceMedia
                    {
                        MediaId = "media-1",
                        ContentType = "image/png",
                        ContentLength = 12345,
                        Url = "https://example.com/media-1",
                        UrlExpiry = "2026-01-03T00:00:00Z"
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(item);

        json.ShouldContain("\"field\":\"expectedOutput\"");
        json.ShouldContain("\"mediaId\":\"media-1\"");

        var roundTripped = JsonSerializer.Deserialize<DatasetItem>(json);

        roundTripped.ShouldNotBeNull();
        roundTripped.MediaReferences.ShouldNotBeNull();
        roundTripped.MediaReferences[0].Field.ShouldBe(DatasetItemMediaReferenceField.ExpectedOutput);
        roundTripped.MediaReferences[0].JsonPath.ShouldBe("$['image']");
        roundTripped.MediaReferences[0].Media.ContentLength.ShouldBe(12345);
    }

    [Fact]
    public void Should_Deserialize_DatasetItem_Without_MediaReferences_As_Null()
    {
        var json = @"{
            ""id"": ""item-1"",
            ""status"": ""ACTIVE"",
            ""datasetId"": ""ds-1"",
            ""datasetName"": ""my-dataset"",
            ""createdAt"": ""2026-01-01T00:00:00Z"",
            ""updatedAt"": ""2026-01-02T00:00:00Z""
        }";

        var item = JsonSerializer.Deserialize<DatasetItem>(json);

        item.ShouldNotBeNull();
        item.MediaReferences.ShouldBeNull();
    }

    [Fact]
    public void Should_Serialize_DatasetItemMediaReferenceField_To_CamelCase()
    {
        JsonSerializer.Serialize(DatasetItemMediaReferenceField.Input).ShouldBe("\"input\"");
        JsonSerializer.Serialize(DatasetItemMediaReferenceField.ExpectedOutput).ShouldBe("\"expectedOutput\"");
        JsonSerializer.Serialize(DatasetItemMediaReferenceField.Metadata).ShouldBe("\"metadata\"");
    }

    [Fact]
    public void Should_Deserialize_DatasetItemMediaReferenceField_From_CamelCase()
    {
        JsonSerializer.Deserialize<DatasetItemMediaReferenceField>("\"input\"")
            .ShouldBe(DatasetItemMediaReferenceField.Input);
        JsonSerializer.Deserialize<DatasetItemMediaReferenceField>("\"expectedOutput\"")
            .ShouldBe(DatasetItemMediaReferenceField.ExpectedOutput);
        JsonSerializer.Deserialize<DatasetItemMediaReferenceField>("\"metadata\"")
            .ShouldBe(DatasetItemMediaReferenceField.Metadata);
    }
}