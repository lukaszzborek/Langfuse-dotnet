using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using zborek.Langfuse.Models.AnnotationQueue;
using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Models.Model;
using zborek.Langfuse.Models.ObservationV2;
using zborek.Langfuse.Models.Organization;
using zborek.Langfuse.Models.Project;
using zborek.Langfuse.Models.Prompt;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Tests.Models;

/// <summary>
///     Tests validating model serialization/deserialization matches API specifications.
///     These tests verify fixes made during the model validation phase.
/// </summary>
public class ModelValidationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    #region Model TokenizerConfig Default Tests

    [Fact]
    public void Model_DefaultValues_TokenizerConfigIsNotNull()
    {
        var model = new Model();

        model.TokenizerConfig.ShouldNotBeNull();
    }

    #endregion

    #region Test Classes

    private class TestAnnotationObject
    {
        public AnnotationObjectType ObjectType { get; set; }
    }

    #endregion

    #region CreateApiKeyRequest Tests

    [Fact]
    public void CreateApiKeyRequest_Should_Serialize_With_NewProperties()
    {
        // Arrange
        var request = new CreateApiKeyRequest
        {
            Name = "Test API Key",
            ExpiresAt = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PublicKey = "pk_test_123",
            SecretKey = "sk_test_456"
        };

        // Act
        var json = JsonSerializer.Serialize(request, JsonOptions);

        // Assert - verify new property names
        json.ShouldContain("\"name\":\"Test API Key\"");
        json.ShouldContain("\"expiresAt\":\"2030-01-01T00:00:00Z\"");
        json.ShouldContain("\"publicKey\":\"pk_test_123\"");
        json.ShouldContain("\"secretKey\":\"sk_test_456\"");
    }

    [Fact]
    public void CreateApiKeyRequest_Should_Deserialize_With_NewProperties()
    {
        // Arrange
        var json = """
                   {
                     "name": "My API Key Name",
                     "publicKey": "pk-abc",
                     "secretKey": "sk-xyz"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<CreateApiKeyRequest>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("My API Key Name");
        result.PublicKey.ShouldBe("pk-abc");
        result.SecretKey.ShouldBe("sk-xyz");
    }

    #endregion

    #region Model Tests

    [Fact]
    public void Model_Should_Deserialize_With_CreatedAt_And_Prices()
    {
        // Arrange
        var json = """
                   {
                     "id": "model-123",
                     "modelName": "gpt-4",
                     "matchPattern": "gpt-4.*",
                     "unit": "TOKENS",
                     "createdAt": "2024-06-15T10:00:00Z",
                     "prices": {
                       "input": {"price": 0.03, "usageType": "input"},
                       "output": {"price": 0.06, "usageType": "output"}
                     },
                     "pricingTiers": []
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<Model>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("model-123");
        result.CreatedAt.ShouldBe(new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc));
        result.Prices.ShouldNotBeNull();
        result.Prices.ContainsKey("input").ShouldBeTrue();
        result.Prices["input"].Price.ShouldBe(0.03);
    }

    [Fact]
    public void CreateModelRequest_Should_Serialize_PricingTiers()
    {
        // Arrange
        var request = new CreateModelRequest
        {
            ModelName = "custom-model",
            MatchPattern = "custom-.*",
            Unit = ModelUsageUnit.Tokens,
            InputPrice = 0.01,
            OutputPrice = 0.02,
            PricingTiers =
            [
                new PricingTierInput
                {
                    Name = "Standard",
                    IsDefault = true,
                    Priority = 0,
                    Conditions = [],
                    Prices = new Dictionary<string, double> { ["input"] = 0.01, ["output"] = 0.02 }
                }
            ]
        };

        // Act
        var json = JsonSerializer.Serialize(request, JsonOptions);

        // Assert
        json.ShouldContain("\"pricingTiers\":");
        json.ShouldContain("\"name\":\"Standard\"");
        json.ShouldContain("\"isDefault\":true");
        json.ShouldContain("\"prices\":");
    }

    #endregion

    #region Score Tests

    [Fact]
    public void ScoreConfig_Should_Use_ScoreConfigDataType()
    {
        // Arrange
        var json = """
                   {
                     "id": "config-123",
                     "name": "Test Config",
                     "dataType": "NUMERIC",
                     "createdAt": "2024-01-15T10:00:00Z",
                     "updatedAt": "2024-01-15T10:00:00Z",
                     "projectId": "proj-123"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<ScoreConfig>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.DataType.ShouldBe(ScoreConfigDataType.Numeric);
    }

    [Fact]
    public void ScoreCreateRequest_Should_Serialize_Timestamp()
    {
        var request = new ScoreCreateRequest
        {
            Name = "accuracy",
            Value = 0.9,
            TraceId = "trace-1",
            Timestamp = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.FromHours(2))
        };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldContain("\"timestamp\":\"2024-06-15T10:00:00+02:00\"");
    }

    [Fact]
    public void ScoreCreateRequest_NullTimestamp_OmittedInSerialization()
    {
        var request = new ScoreCreateRequest { Name = "accuracy", Value = 0.9, TraceId = "trace-1" };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldNotContain("\"timestamp\"");
    }

    #endregion

    #region Dataset Tests

    [Fact]
    public void Dataset_Should_Deserialize_With_SchemaProperties()
    {
        // Arrange
        var json = """
                   {
                     "id": "dataset-123",
                     "name": "Test Dataset",
                     "projectId": "proj-456",
                     "inputSchema": {"type": "object", "properties": {"text": {"type": "string"}}},
                     "expectedOutputSchema": {"type": "object", "properties": {"result": {"type": "number"}}},
                     "createdAt": "2024-01-15T10:00:00Z",
                     "updatedAt": "2024-01-15T10:00:00Z"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<DatasetModel>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.InputSchema.ShouldNotBeNull();
        result.ExpectedOutputSchema.ShouldNotBeNull();
    }

    [Fact]
    public void CreateDatasetRequest_Should_Serialize_With_SchemaProperties()
    {
        // Arrange
        var request = new CreateDatasetRequest
        {
            Name = "New Dataset",
            InputSchema = new { type = "object" },
            ExpectedOutputSchema = new { type = "string" }
        };

        // Act
        var json = JsonSerializer.Serialize(request, JsonOptions);

        // Assert
        json.ShouldContain("\"inputSchema\":");
        json.ShouldContain("\"expectedOutputSchema\":");
    }

    #endregion

    #region AnnotationObjectType Tests

    [Fact]
    public void AnnotationObjectType_Should_Include_Session()
    {
        // Assert - Session enum value should exist
        var sessionValue = AnnotationObjectType.Session;
        sessionValue.ShouldBe(AnnotationObjectType.Session);
    }

    [Fact]
    public void AnnotationObjectType_Should_Serialize_Session()
    {
        // Arrange
        var item = new { ObjectType = AnnotationObjectType.Session };

        // Act
        var json = JsonSerializer.Serialize(item);

        // Assert
        json.ShouldContain("\"SESSION\"");
    }

    [Fact]
    public void AnnotationObjectType_Should_Deserialize_Session()
    {
        // Arrange
        var json = """{"ObjectType":"SESSION"}""";

        // Act
        var result = JsonSerializer.Deserialize<TestAnnotationObject>(json);

        // Assert
        result.ShouldNotBeNull();
        result.ObjectType.ShouldBe(AnnotationObjectType.Session);
    }

    #endregion

    #region PromptMeta Tests

    [Fact]
    public void PromptMeta_Should_Deserialize_With_Type()
    {
        // Arrange
        var json = """
                   {
                     "name": "My Prompt",
                     "type": "chat",
                     "versions": [1, 2, 3],
                     "labels": ["production"],
                     "tags": ["v2"],
                     "lastUpdatedAt": "2024-01-15T10:00:00Z"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<PromptMeta>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Type.ShouldBe(PromptType.Chat);
    }

    [Fact]
    public void PromptMeta_Should_Serialize_With_Type()
    {
        // Arrange
        var meta = new PromptMeta
        {
            Name = "Test Prompt",
            Type = PromptType.Text,
            Versions = [1],
            Labels = ["dev"],
            Tags = ["test"],
            LastUpdatedAt = DateTime.UtcNow
        };

        // Act
        var json = JsonSerializer.Serialize(meta, JsonOptions);

        // Assert
        json.ShouldContain("\"type\":\"text\"");
    }

    #endregion

    #region UpdateProjectRequest Retention Nullable Tests

    [Fact]
    public void UpdateProjectRequest_Retention_IsNullable()
    {
        var request = new UpdateProjectRequest { Name = "test", Retention = null };

        request.Retention.ShouldBeNull();
    }

    [Fact]
    public void UpdateProjectRequest_NullRetention_OmittedInSerialization()
    {
        var request = new UpdateProjectRequest { Name = "test", Retention = null };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldNotContain("\"retention\"");
    }

    [Fact]
    public void UpdateProjectRequest_Should_Serialize_Retention_WhenSet()
    {
        var request = new UpdateProjectRequest { Name = "test", Retention = 30 };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldContain("\"retention\":30");
    }

    [Fact]
    public void UpdateProjectRequest_Should_Deserialize_NullRetention()
    {
        var json = """{"name": "test"}""";

        var result = JsonSerializer.Deserialize<UpdateProjectRequest>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.Retention.ShouldBeNull();
    }

    #endregion

    #region DatasetItemListRequest Version Tests

    [Fact]
    public void DatasetItemListRequest_Should_Serialize_Version()
    {
        var date = new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc);
        var request = new DatasetItemListRequest { Version = date };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldContain("\"version\":");
        json.ShouldContain("2024-06-15");
    }

    [Fact]
    public void DatasetItemListRequest_NullVersion_OmittedInSerialization()
    {
        var request = new DatasetItemListRequest { DatasetName = "test" };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldNotContain("\"version\"");
    }

    #endregion

    #region ObservationsV2Request Tests

    [Fact]
    public void ObservationsV2Request_Should_Serialize_ExpandMetadata()
    {
        var request = new ObservationsV2Request { ExpandMetadata = "otel" };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldContain("\"expandMetadata\":\"otel\"");
    }

    [Fact]
    public void ObservationsV2Request_ParseIoAsJson_HasObsoleteAttribute()
    {
        var property = typeof(ObservationsV2Request).GetProperty("ParseIoAsJson");

        property.ShouldNotBeNull();
        var obsoleteAttr = property.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        obsoleteAttr.ShouldNotBeEmpty();
    }

    #endregion
}
