using System.Text.Json;
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

        Assert.Contains("\"S3\"", json);
        Assert.Contains("\"S3_COMPATIBLE\"", json);
        Assert.Contains("\"AZURE_BLOB_STORAGE\"", json);
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

        Assert.Equal(BlobStorageIntegrationType.S3, result1!.Type);
        Assert.Equal(BlobStorageIntegrationType.S3Compatible, result2!.Type);
        Assert.Equal(BlobStorageIntegrationType.AzureBlobStorage, result3!.Type);
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

        Assert.Contains("\"hourly\"", json);
        Assert.Contains("\"daily\"", json);
        Assert.Contains("\"weekly\"", json);
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

        Assert.Equal(BlobStorageExportFrequency.Hourly, result1!.Frequency);
        Assert.Equal(BlobStorageExportFrequency.Daily, result2!.Frequency);
        Assert.Equal(BlobStorageExportFrequency.Weekly, result3!.Frequency);
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

        Assert.Contains("\"FULL_HISTORY\"", json);
        Assert.Contains("\"FROM_TODAY\"", json);
        Assert.Contains("\"FROM_CUSTOM_DATE\"", json);
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

        Assert.Contains("\"JSON\"", json);
        Assert.Contains("\"CSV\"", json);
        Assert.Contains("\"JSONL\"", json);
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

        Assert.Contains("\"projectId\"", json);
        Assert.Contains("\"project-123\"", json);
        Assert.Contains("\"type\"", json);
        Assert.Contains("\"S3_COMPATIBLE\"", json);
        Assert.Contains("\"bucketName\"", json);
        Assert.Contains("\"my-bucket\"", json);
        Assert.Contains("\"endpoint\"", json);
        Assert.Contains("\"https://s3.example.com\"", json);
        Assert.Contains("\"region\"", json);
        Assert.Contains("\"us-east-1\"", json);
        Assert.Contains("\"accessKeyId\"", json);
        Assert.Contains("\"secretAccessKey\"", json);
        Assert.Contains("\"prefix\"", json);
        Assert.Contains("\"exports/\"", json);
        Assert.Contains("\"exportFrequency\"", json);
        Assert.Contains("\"daily\"", json);
        Assert.Contains("\"enabled\"", json);
        Assert.Contains("\"forcePathStyle\"", json);
        Assert.Contains("\"fileType\"", json);
        Assert.Contains("\"JSONL\"", json);
        Assert.Contains("\"exportMode\"", json);
        Assert.Contains("\"FROM_CUSTOM_DATE\"", json);
        Assert.Contains("\"exportStartDate\"", json);
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

        Assert.Contains("\"projectId\":\"project-123\"", json);
        Assert.Contains("\"type\":\"S3\"", json);
        Assert.Contains("\"bucketName\":\"my-bucket\"", json);
        Assert.Contains("\"endpoint\":null", json);
        Assert.Contains("\"accessKeyId\":null", json);
        Assert.Contains("\"secretAccessKey\":null", json);
        Assert.Contains("\"prefix\":null", json);
        Assert.Contains("\"exportStartDate\":null", json);
        Assert.Contains("\"enabled\":false", json);
        Assert.Contains("\"forcePathStyle\":false", json);
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
            ""exportStartDate"": null,
            ""nextSyncAt"": ""2024-08-10T12:00:00Z"",
            ""lastSyncAt"": ""2024-08-03T12:00:00Z"",
            ""createdAt"": ""2024-01-01T10:00:00Z"",
            ""updatedAt"": ""2024-08-01T15:30:00Z""
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationResponse>(json);

        Assert.NotNull(response);
        Assert.Equal("integration-123", response.Id);
        Assert.Equal("project-456", response.ProjectId);
        Assert.Equal(BlobStorageIntegrationType.S3, response.Type);
        Assert.Equal("test-bucket", response.BucketName);
        Assert.Null(response.Endpoint);
        Assert.Equal("eu-central-1", response.Region);
        Assert.Equal("AKIATEST", response.AccessKeyId);
        Assert.Equal("data/", response.Prefix);
        Assert.Equal(BlobStorageExportFrequency.Weekly, response.ExportFrequency);
        Assert.True(response.Enabled);
        Assert.False(response.ForcePathStyle);
        Assert.Equal(BlobStorageIntegrationFileType.Csv, response.FileType);
        Assert.Equal(BlobStorageExportMode.FromToday, response.ExportMode);
        Assert.Null(response.ExportStartDate);
        Assert.Equal(new DateTime(2024, 8, 10, 12, 0, 0, DateTimeKind.Utc), response.NextSyncAt);
        Assert.Equal(new DateTime(2024, 8, 3, 12, 0, 0, DateTimeKind.Utc), response.LastSyncAt);
        Assert.Equal(new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc), response.CreatedAt);
        Assert.Equal(new DateTime(2024, 8, 1, 15, 30, 0, DateTimeKind.Utc), response.UpdatedAt);
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
                    ""exportFrequency"": ""daily"",
                    ""enabled"": true,
                    ""forcePathStyle"": false,
                    ""fileType"": ""JSON"",
                    ""exportMode"": ""FULL_HISTORY"",
                    ""createdAt"": ""2024-01-01T00:00:00Z"",
                    ""updatedAt"": ""2024-01-01T00:00:00Z""
                },
                {
                    ""id"": ""int-2"",
                    ""projectId"": ""proj-1"",
                    ""type"": ""AZURE_BLOB_STORAGE"",
                    ""bucketName"": ""bucket-2"",
                    ""region"": ""eastus"",
                    ""exportFrequency"": ""hourly"",
                    ""enabled"": false,
                    ""forcePathStyle"": true,
                    ""fileType"": ""JSONL"",
                    ""exportMode"": ""FROM_TODAY"",
                    ""createdAt"": ""2024-02-01T00:00:00Z"",
                    ""updatedAt"": ""2024-02-01T00:00:00Z""
                }
            ]
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationsResponse>(json);

        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.Equal(2, response.Data.Length);

        Assert.Equal("int-1", response.Data[0].Id);
        Assert.Equal(BlobStorageIntegrationType.S3, response.Data[0].Type);
        Assert.Equal(BlobStorageExportFrequency.Daily, response.Data[0].ExportFrequency);
        Assert.True(response.Data[0].Enabled);

        Assert.Equal("int-2", response.Data[1].Id);
        Assert.Equal(BlobStorageIntegrationType.AzureBlobStorage, response.Data[1].Type);
        Assert.Equal(BlobStorageExportFrequency.Hourly, response.Data[1].ExportFrequency);
        Assert.False(response.Data[1].Enabled);
    }

    [Fact]
    public void Should_Deserialize_BlobStorageIntegrationDeletionResponse()
    {
        var json = @"{""message"": ""Blob storage integration deleted successfully""}";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationDeletionResponse>(json);

        Assert.NotNull(response);
        Assert.Equal("Blob storage integration deleted successfully", response.Message);
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
            ""exportStartDate"": null,
            ""nextSyncAt"": null,
            ""lastSyncAt"": null,
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<BlobStorageIntegrationResponse>(json);

        Assert.NotNull(response);
        Assert.Null(response.Endpoint);
        Assert.Null(response.AccessKeyId);
        Assert.Null(response.Prefix);
        Assert.Null(response.ExportStartDate);
        Assert.Null(response.NextSyncAt);
        Assert.Null(response.LastSyncAt);
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