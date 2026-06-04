using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.BlobStorageIntegration;

namespace zborek.Langfuse.Tests.Models;

public class BlobStorageIntegrationTests
{
    [Fact]
    public void Should_Serialize_BlobStorageIntegrationType_To_SnakeCaseUpper()
    {
        var testData = new
        {
            Type1 = BlobStorageIntegrationType.S3,
            Type2 = BlobStorageIntegrationType.S3Compatible,
            Type3 = BlobStorageIntegrationType.AzureBlobStorage
        };

        var json = JsonSerializer.Serialize(testData);

        json.ShouldContain("\"S3\"");
        json.ShouldContain("\"S3_COMPATIBLE\"");
        json.ShouldContain("\"AZURE_BLOB_STORAGE\"");
    }

    [Fact]
    public void Should_Deserialize_BlobStorageIntegrationType_From_SnakeCaseUpper()
    {
        var json1 = "{\"Type\":\"S3\"}";
        var json2 = "{\"Type\":\"S3_COMPATIBLE\"}";
        var json3 = "{\"Type\":\"AZURE_BLOB_STORAGE\"}";

        var result1 = JsonSerializer.Deserialize<TestTypeWrapper>(json1);
        var result2 = JsonSerializer.Deserialize<TestTypeWrapper>(json2);
        var result3 = JsonSerializer.Deserialize<TestTypeWrapper>(json3);

        result1!.Type.ShouldBe(BlobStorageIntegrationType.S3);
        result2!.Type.ShouldBe(BlobStorageIntegrationType.S3Compatible);
        result3!.Type.ShouldBe(BlobStorageIntegrationType.AzureBlobStorage);
    }

    [Fact]
    public void Should_Serialize_BlobStorageExportFrequency_To_Lowercase()
    {
        var testData = new
        {
            Freq1 = BlobStorageExportFrequency.Hourly,
            Freq2 = BlobStorageExportFrequency.Daily,
            Freq3 = BlobStorageExportFrequency.Weekly
        };

        var json = JsonSerializer.Serialize(testData);

        json.ShouldContain("\"hourly\"");
        json.ShouldContain("\"daily\"");
        json.ShouldContain("\"weekly\"");
    }

    [Fact]
    public void Should_Deserialize_BlobStorageExportFrequency_From_Lowercase()
    {
        var json1 = "{\"Frequency\":\"hourly\"}";
        var json2 = "{\"Frequency\":\"daily\"}";
        var json3 = "{\"Frequency\":\"weekly\"}";

        var result1 = JsonSerializer.Deserialize<TestFrequencyWrapper>(json1);
        var result2 = JsonSerializer.Deserialize<TestFrequencyWrapper>(json2);
        var result3 = JsonSerializer.Deserialize<TestFrequencyWrapper>(json3);

        result1!.Frequency.ShouldBe(BlobStorageExportFrequency.Hourly);
        result2!.Frequency.ShouldBe(BlobStorageExportFrequency.Daily);
        result3!.Frequency.ShouldBe(BlobStorageExportFrequency.Weekly);
    }

    [Fact]
    public void Should_Serialize_BlobStorageExportMode_To_SnakeCaseUpper()
    {
        var testData = new
        {
            Mode1 = BlobStorageExportMode.FullHistory,
            Mode2 = BlobStorageExportMode.FromToday,
            Mode3 = BlobStorageExportMode.FromCustomDate
        };

        var json = JsonSerializer.Serialize(testData);

        json.ShouldContain("\"FULL_HISTORY\"");
        json.ShouldContain("\"FROM_TODAY\"");
        json.ShouldContain("\"FROM_CUSTOM_DATE\"");
    }

    [Fact]
    public void Should_Serialize_BlobStorageIntegrationFileType_To_Uppercase()
    {
        var testData = new
        {
            File1 = BlobStorageIntegrationFileType.Json,
            File2 = BlobStorageIntegrationFileType.Csv,
            File3 = BlobStorageIntegrationFileType.Jsonl
        };

        var json = JsonSerializer.Serialize(testData);

        json.ShouldContain("\"JSON\"");
        json.ShouldContain("\"CSV\"");
        json.ShouldContain("\"JSONL\"");
    }

    [Fact]
    public void Should_Serialize_CreateBlobStorageIntegrationRequest_With_All_Fields()
    {
        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = "project-123",
            Type = BlobStorageIntegrationType.S3Compatible,
            BucketName = "my-bucket",
            Endpoint = "https://s3.example.com",
            Region = "us-east-1",
            AccessKeyId = "AKIAIOSFODNN7EXAMPLE",
            SecretAccessKey = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY",
            Prefix = "exports/",
            ExportFrequency = BlobStorageExportFrequency.Daily,
            Enabled = true,
            ForcePathStyle = true,
            FileType = BlobStorageIntegrationFileType.Jsonl,
            ExportMode = BlobStorageExportMode.FromCustomDate,
            ExportStartDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"projectId\"");
        json.ShouldContain("\"project-123\"");
        json.ShouldContain("\"type\"");
        json.ShouldContain("\"S3_COMPATIBLE\"");
        json.ShouldContain("\"bucketName\"");
        json.ShouldContain("\"my-bucket\"");
        json.ShouldContain("\"endpoint\"");
        json.ShouldContain("\"https://s3.example.com\"");
        json.ShouldContain("\"region\"");
        json.ShouldContain("\"us-east-1\"");
        json.ShouldContain("\"accessKeyId\"");
        json.ShouldContain("\"secretAccessKey\"");
        json.ShouldContain("\"prefix\"");
        json.ShouldContain("\"exports/\"");
        json.ShouldContain("\"exportFrequency\"");
        json.ShouldContain("\"daily\"");
        json.ShouldContain("\"enabled\"");
        json.ShouldContain("\"forcePathStyle\"");
        json.ShouldContain("\"fileType\"");
        json.ShouldContain("\"JSONL\"");
        json.ShouldContain("\"exportMode\"");
        json.ShouldContain("\"FROM_CUSTOM_DATE\"");
        json.ShouldContain("\"exportStartDate\"");
    }

    [Fact]
    public void Should_Serialize_CreateBlobStorageIntegrationRequest_With_Null_Fields()
    {
        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = "project-123",
            Type = BlobStorageIntegrationType.S3,
            BucketName = "my-bucket",
            Endpoint = null,
            Region = "us-east-1",
            AccessKeyId = null,
            SecretAccessKey = null,
            Prefix = null,
            ExportFrequency = BlobStorageExportFrequency.Hourly,
            Enabled = false,
            ForcePathStyle = false,
            FileType = BlobStorageIntegrationFileType.Json,
            ExportMode = BlobStorageExportMode.FullHistory,
            ExportStartDate = null
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"projectId\":\"project-123\"");
        json.ShouldContain("\"type\":\"S3\"");
        json.ShouldContain("\"bucketName\":\"my-bucket\"");
        json.ShouldContain("\"endpoint\":null");
        json.ShouldContain("\"accessKeyId\":null");
        json.ShouldContain("\"secretAccessKey\":null");
        json.ShouldContain("\"prefix\":null");
        json.ShouldContain("\"exportStartDate\":null");
        json.ShouldContain("\"enabled\":false");
        json.ShouldContain("\"forcePathStyle\":false");
    }

    [Fact]
    public void Should_Deserialize_BlobStorageIntegrationResponse()
    {
        var json = @"{
            ""id"": ""integration-123"",
            ""projectId"": ""project-456"",
            ""type"": ""S3"",
            ""bucketName"": ""test-bucket"",
            ""endpoint"": null,
            ""region"": ""eu-central-1"",
            ""accessKeyId"": ""AKIATEST"",
            ""prefix"": ""data/"",
            ""exportFrequency"": ""weekly"",
            ""enabled"": true,
            ""forcePathStyle"": false,
            ""fileType"": ""CSV"",
            ""exportMode"": ""FROM_TODAY"",
            ""compressed"": true,
            ""exportSource"": ""OBSERVATIONS_V2"",
            ""exportStartDate"": null,
            ""nextSyncAt"": ""2024-08-10T12:00:00Z"",
            ""lastSyncAt"": ""2024-08-03T12:00:00Z"",
            ""createdAt"": ""2024-01-01T10:00:00Z"",
            ""updatedAt"": ""2024-08-01T15:30:00Z""
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationResponse>(json);

        response.ShouldNotBeNull();
        response.Id.ShouldBe("integration-123");
        response.ProjectId.ShouldBe("project-456");
        response.Type.ShouldBe(BlobStorageIntegrationType.S3);
        response.BucketName.ShouldBe("test-bucket");
        response.Endpoint.ShouldBeNull();
        response.Region.ShouldBe("eu-central-1");
        response.AccessKeyId.ShouldBe("AKIATEST");
        response.Prefix.ShouldBe("data/");
        response.ExportFrequency.ShouldBe(BlobStorageExportFrequency.Weekly);
        response.Enabled.ShouldBeTrue();
        response.ForcePathStyle.ShouldBeFalse();
        response.FileType.ShouldBe(BlobStorageIntegrationFileType.Csv);
        response.ExportMode.ShouldBe(BlobStorageExportMode.FromToday);
        response.ExportStartDate.ShouldBeNull();
        response.NextSyncAt.ShouldBe(new DateTime(2024, 8, 10, 12, 0, 0, DateTimeKind.Utc));
        response.LastSyncAt.ShouldBe(new DateTime(2024, 8, 3, 12, 0, 0, DateTimeKind.Utc));
        response.CreatedAt.ShouldBe(new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        response.UpdatedAt.ShouldBe(new DateTime(2024, 8, 1, 15, 30, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Should_Deserialize_BlobStorageIntegrationsResponse_With_Multiple_Items()
    {
        var json = @"{
            ""data"": [
                {
                    ""id"": ""int-1"",
                    ""projectId"": ""proj-1"",
                    ""type"": ""S3"",
                    ""bucketName"": ""bucket-1"",
                    ""region"": ""us-east-1"",
                    ""prefix"": ""exports/"",
                    ""exportFrequency"": ""daily"",
                    ""enabled"": true,
                    ""forcePathStyle"": false,
                    ""fileType"": ""JSON"",
                    ""exportMode"": ""FULL_HISTORY"",
                    ""compressed"": true,
                    ""exportSource"": ""OBSERVATIONS_V2"",
                    ""createdAt"": ""2024-01-01T00:00:00Z"",
                    ""updatedAt"": ""2024-01-01T00:00:00Z""
                },
                {
                    ""id"": ""int-2"",
                    ""projectId"": ""proj-1"",
                    ""type"": ""AZURE_BLOB_STORAGE"",
                    ""bucketName"": ""bucket-2"",
                    ""region"": ""eastus"",
                    ""prefix"": """",
                    ""exportFrequency"": ""hourly"",
                    ""enabled"": false,
                    ""forcePathStyle"": true,
                    ""fileType"": ""JSONL"",
                    ""exportMode"": ""FROM_TODAY"",
                    ""compressed"": false,
                    ""exportSource"": ""LEGACY_TRACES_OBSERVATIONS"",
                    ""createdAt"": ""2024-02-01T00:00:00Z"",
                    ""updatedAt"": ""2024-02-01T00:00:00Z""
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationsResponse>(json);

        response.ShouldNotBeNull();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBe(2);

        response.Data[0].Id.ShouldBe("int-1");
        response.Data[0].Type.ShouldBe(BlobStorageIntegrationType.S3);
        response.Data[0].ExportFrequency.ShouldBe(BlobStorageExportFrequency.Daily);
        response.Data[0].Enabled.ShouldBeTrue();

        response.Data[1].Id.ShouldBe("int-2");
        response.Data[1].Type.ShouldBe(BlobStorageIntegrationType.AzureBlobStorage);
        response.Data[1].ExportFrequency.ShouldBe(BlobStorageExportFrequency.Hourly);
        response.Data[1].Enabled.ShouldBeFalse();
    }

    [Fact]
    public void Should_Deserialize_BlobStorageIntegrationDeletionResponse()
    {
        var json = @"{""message"": ""Blob storage integration deleted successfully""}";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationDeletionResponse>(json);

        response.ShouldNotBeNull();
        response.Message.ShouldBe("Blob storage integration deleted successfully");
    }

    [Fact]
    public void Should_Handle_Null_Optional_Fields_In_Response()
    {
        var json = @"{
            ""id"": ""int-1"",
            ""projectId"": ""proj-1"",
            ""type"": ""S3"",
            ""bucketName"": ""bucket"",
            ""endpoint"": null,
            ""region"": ""us-east-1"",
            ""accessKeyId"": null,
            ""prefix"": null,
            ""exportFrequency"": ""daily"",
            ""enabled"": true,
            ""forcePathStyle"": false,
            ""fileType"": ""JSON"",
            ""exportMode"": ""FULL_HISTORY"",
            ""compressed"": false,
            ""exportSource"": ""LEGACY_TRACES_OBSERVATIONS"",
            ""exportStartDate"": null,
            ""nextSyncAt"": null,
            ""lastSyncAt"": null,
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationResponse>(json);

        response.ShouldNotBeNull();
        response.Endpoint.ShouldBeNull();
        response.AccessKeyId.ShouldBeNull();
        response.Prefix.ShouldBeNull();
        response.ExportStartDate.ShouldBeNull();
        response.NextSyncAt.ShouldBeNull();
        response.LastSyncAt.ShouldBeNull();
    }

    [Fact]
    public void Should_Serialize_BlobStorageExportSource_To_SnakeCaseUpper()
    {
        JsonSerializer.Serialize(BlobStorageExportSource.LegacyTracesObservations)
            .ShouldBe("\"LEGACY_TRACES_OBSERVATIONS\"");
        JsonSerializer.Serialize(BlobStorageExportSource.ObservationsV2)
            .ShouldBe("\"OBSERVATIONS_V2\"");
        JsonSerializer.Serialize(BlobStorageExportSource.LegacyTracesAndEnrichedObservations)
            .ShouldBe("\"LEGACY_TRACES_AND_ENRICHED_OBSERVATIONS\"");
    }

    [Fact]
    public void Should_Deserialize_BlobStorageExportSource_From_SnakeCaseUpper()
    {
        JsonSerializer.Deserialize<BlobStorageExportSource>("\"LEGACY_TRACES_OBSERVATIONS\"")
            .ShouldBe(BlobStorageExportSource.LegacyTracesObservations);
        JsonSerializer.Deserialize<BlobStorageExportSource>("\"OBSERVATIONS_V2\"")
            .ShouldBe(BlobStorageExportSource.ObservationsV2);
        JsonSerializer.Deserialize<BlobStorageExportSource>("\"LEGACY_TRACES_AND_ENRICHED_OBSERVATIONS\"")
            .ShouldBe(BlobStorageExportSource.LegacyTracesAndEnrichedObservations);
    }

    [Fact]
    public void Should_Serialize_BlobStorageExportFieldGroup_To_Lowercase()
    {
        JsonSerializer.Serialize(BlobStorageExportFieldGroup.Core).ShouldBe("\"core\"");
        JsonSerializer.Serialize(BlobStorageExportFieldGroup.Io).ShouldBe("\"io\"");
        JsonSerializer.Serialize(BlobStorageExportFieldGroup.Trace_Context).ShouldBe("\"trace_context\"");
    }

    [Fact]
    public void Should_Deserialize_BlobStorageExportFieldGroup_From_Lowercase()
    {
        JsonSerializer.Deserialize<BlobStorageExportFieldGroup>("\"core\"")
            .ShouldBe(BlobStorageExportFieldGroup.Core);
        JsonSerializer.Deserialize<BlobStorageExportFieldGroup>("\"io\"")
            .ShouldBe(BlobStorageExportFieldGroup.Io);
        JsonSerializer.Deserialize<BlobStorageExportFieldGroup>("\"trace_context\"")
            .ShouldBe(BlobStorageExportFieldGroup.Trace_Context);
    }

    [Fact]
    public void Should_Serialize_CreateRequest_With_ExportSource_And_FieldGroups()
    {
        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = "p1",
            Type = BlobStorageIntegrationType.S3,
            BucketName = "b",
            Region = "us-east-1",
            ExportFrequency = BlobStorageExportFrequency.Daily,
            Enabled = true,
            ForcePathStyle = false,
            FileType = BlobStorageIntegrationFileType.Json,
            ExportMode = BlobStorageExportMode.FullHistory,
            Compressed = true,
            ExportSource = BlobStorageExportSource.ObservationsV2,
            ExportFieldGroups = new[] { BlobStorageExportFieldGroup.Core, BlobStorageExportFieldGroup.Io }
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"compressed\":true");
        json.ShouldContain("\"exportSource\":\"OBSERVATIONS_V2\"");
        json.ShouldContain("\"exportFieldGroups\":[\"core\",\"io\"]");
    }

    [Fact]
    public void Should_Omit_Null_ExportSource_And_FieldGroups_On_CreateRequest()
    {
        var request = new CreateBlobStorageIntegrationRequest
        {
            ProjectId = "p1",
            Type = BlobStorageIntegrationType.S3,
            BucketName = "b",
            Region = "us-east-1",
            ExportFrequency = BlobStorageExportFrequency.Daily,
            Enabled = true,
            ForcePathStyle = false,
            FileType = BlobStorageIntegrationFileType.Json,
            ExportMode = BlobStorageExportMode.FullHistory
        };

        // Client serializer ignores nulls; mirror that here so omit-to-preserve semantics hold.
        var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        json.ShouldNotContain("exportSource");
        json.ShouldNotContain("exportFieldGroups");
        json.ShouldNotContain("compressed");
    }

    [Fact]
    public void Should_Deserialize_Response_With_New_Fields()
    {
        var json = @"{
            ""id"": ""int-1"",
            ""projectId"": ""proj-1"",
            ""type"": ""S3"",
            ""bucketName"": ""bucket"",
            ""region"": ""us-east-1"",
            ""prefix"": """",
            ""exportFrequency"": ""daily"",
            ""enabled"": true,
            ""forcePathStyle"": false,
            ""fileType"": ""JSON"",
            ""exportMode"": ""FULL_HISTORY"",
            ""compressed"": true,
            ""exportSource"": ""OBSERVATIONS_V2"",
            ""exportFieldGroups"": [""core"", ""trace_context""],
            ""lastError"": ""boom"",
            ""lastErrorAt"": ""2024-01-02T00:00:00Z"",
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationResponse>(json);

        response.ShouldNotBeNull();
        response.Compressed.ShouldBeTrue();
        response.ExportSource.ShouldBe(BlobStorageExportSource.ObservationsV2);
        response.ExportFieldGroups.ShouldNotBeNull();
        response.ExportFieldGroups.ShouldBe(new[]
        {
            BlobStorageExportFieldGroup.Core, BlobStorageExportFieldGroup.Trace_Context
        });
        response.LastError.ShouldBe("boom");
        response.LastErrorAt.ShouldBe(new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Should_Deserialize_Response_With_Null_ExportFieldGroups()
    {
        var json = @"{
            ""id"": ""int-1"",
            ""projectId"": ""proj-1"",
            ""type"": ""S3"",
            ""bucketName"": ""bucket"",
            ""region"": ""us-east-1"",
            ""prefix"": """",
            ""exportFrequency"": ""daily"",
            ""enabled"": true,
            ""forcePathStyle"": false,
            ""fileType"": ""JSON"",
            ""exportMode"": ""FULL_HISTORY"",
            ""compressed"": false,
            ""exportSource"": ""LEGACY_TRACES_OBSERVATIONS"",
            ""exportFieldGroups"": null,
            ""lastError"": null,
            ""lastErrorAt"": null,
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationResponse>(json);

        response.ShouldNotBeNull();
        response.Compressed.ShouldBeFalse();
        response.ExportSource.ShouldBe(BlobStorageExportSource.LegacyTracesObservations);
        response.ExportFieldGroups.ShouldBeNull();
        response.LastError.ShouldBeNull();
        response.LastErrorAt.ShouldBeNull();
    }

    private class TestTypeWrapper
    {
        public BlobStorageIntegrationType Type { get; set; }
    }

    private class TestFrequencyWrapper
    {
        public BlobStorageExportFrequency Frequency { get; set; }
    }
}