using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using zborek.Langfuse.Models.AnnotationQueue;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Models.Model;
using zborek.Langfuse.Models.Observation;
using zborek.Langfuse.Models.ObservationV2;
using zborek.Langfuse.Models.Organization;
using zborek.Langfuse.Models.Project;
using zborek.Langfuse.Models.Prompt;
using zborek.Langfuse.Models.Score;
using zborek.Langfuse.Models.Session;
using zborek.Langfuse.Models.Trace;

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

    #region CreateGenerationEventBody Tests

    [Fact]
    public void CreateGenerationEventBody_Should_Serialize_With_UsageDetails()
    {
        // Arrange
        var body = new CreateGenerationEventBody
        {
            Id = "gen-123",
            TraceId = "trace-123",
            Name = "Generation",
            Model = "gpt-4",
            Environment = "production",
            UsageDetails = new Dictionary<string, int>
            {
                ["input_tokens"] = 100,
                ["output_tokens"] = 50
            }
        };

        // Act
        var json = JsonSerializer.Serialize(body, JsonOptions);

        // Assert
        json.ShouldContain("\"environment\":\"production\"");
        json.ShouldContain("\"usageDetails\":");
        json.ShouldContain("\"input_tokens\":100");
        json.ShouldContain("\"output_tokens\":50");
    }

    #endregion

    #region ObservationModel Tests

    [Fact]
    public void ObservationModel_Should_Deserialize_With_AllNewProperties()
    {
        // Arrange
        var json = """
                   {
                     "id": "obs-123",
                     "type": "GENERATION",
                     "startTime": "2024-01-15T10:00:00Z",
                     "level": "DEFAULT",
                     "traceId": "trace-456",
                     "completionStartTime": "2024-01-15T10:00:01Z",
                     "usageDetails": {"input_tokens": 100, "output_tokens": 50},
                     "costDetails": {"input": 0.01, "output": 0.02},
                     "promptId": "prompt-abc",
                     "promptName": "My Prompt",
                     "promptVersion": 3,
                     "modelId": "model-xyz",
                     "inputPrice": 0.001,
                     "outputPrice": 0.002,
                     "totalPrice": 0.003,
                     "latency": 1.5,
                     "timeToFirstToken": 0.2,
                     "environment": "production",
                     "createdAt": "2024-01-15T10:00:00Z",
                     "updatedAt": "2024-01-15T10:00:02Z"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<ObservationModel>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("obs-123");
        result.TraceId.ShouldBe("trace-456");
        result.CompletionStartTime.ShouldNotBeNull();
        result.UsageDetails.ShouldNotBeNull();
        result.UsageDetails!["input_tokens"].ShouldBe(100);
        result.CostDetails.ShouldNotBeNull();
        result.CostDetails!["input"].ShouldBe(0.01);
        result.PromptId.ShouldBe("prompt-abc");
        result.PromptName.ShouldBe("My Prompt");
        result.PromptVersion.ShouldBe(3);
        result.ModelId.ShouldBe("model-xyz");
        result.InputPrice.ShouldBe(0.001);
        result.OutputPrice.ShouldBe(0.002);
        result.TotalPrice.ShouldBe(0.003);
        result.Latency.ShouldBe(1.5);
        result.TimeToFirstToken.ShouldBe(0.2);
        result.Environment.ShouldBe("production");
    }

    #endregion

    #region IIngestionEvent Metadata Tests

    [Fact]
    public void CreateScoreEvent_Should_Have_Metadata_Property()
    {
        // Arrange
        var body = new CreateScoreEventBody
        {
            Id = "score-id",
            TraceId = "trace-id",
            Name = "test",
            Value = 1.0
        };
        var evt = new CreateScoreEvent(body, "2024-01-15T10:00:00Z")
        {
            Metadata = new { debug = true }
        };

        // Act
        var json = JsonSerializer.Serialize(evt, JsonOptions);

        // Assert
        json.ShouldContain("\"metadata\":");
        json.ShouldContain("\"debug\":true");
    }

    #endregion

    #region SessionModel Default Values Tests

    [Fact]
    public void SessionModel_DefaultValues_EnvironmentIsEmptyString()
    {
        var session = new SessionModel();

        session.Environment.ShouldNotBeNull();
        session.Environment.ShouldBe(string.Empty);
    }

    #endregion

    #region ScoreModel Default Values Tests

    [Fact]
    public void ScoreModel_DefaultValues_EnvironmentIsEmptyString()
    {
        var score = new ScoreModel();

        score.Environment.ShouldNotBeNull();
        score.Environment.ShouldBe(string.Empty);
    }

    #endregion

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
            Note = "Test API Key",
            PublicKey = "pk_test_123",
            SecretKey = "sk_test_456"
        };

        // Act
        var json = JsonSerializer.Serialize(request, JsonOptions);

        // Assert - verify new property names
        json.ShouldContain("\"note\":\"Test API Key\"");
        json.ShouldContain("\"publicKey\":\"pk_test_123\"");
        json.ShouldContain("\"secretKey\":\"sk_test_456\"");
    }

    [Fact]
    public void CreateApiKeyRequest_Should_Deserialize_With_NewProperties()
    {
        // Arrange
        var json = """
                   {
                     "note": "My API Key Note",
                     "publicKey": "pk-abc",
                     "secretKey": "sk-xyz"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<CreateApiKeyRequest>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Note.ShouldBe("My API Key Note");
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

    #region CreateTraceBody Tests

    [Fact]
    public void CreateTraceBody_Should_Serialize_With_Correct_Casing()
    {
        // Arrange
        var body = new CreateTraceBody
        {
            Id = "trace-123",
            Name = "Test Trace",
            Version = "1.0.0",
            Public = true,
            Environment = "production"
        };

        // Act
        var json = JsonSerializer.Serialize(body, JsonOptions);

        // Assert - verify camelCase properties
        json.ShouldContain("\"version\":\"1.0.0\"");
        json.ShouldContain("\"public\":true");
        json.ShouldContain("\"environment\":\"production\"");
    }

    [Fact]
    public void CreateTraceBody_Should_Deserialize_With_Correct_Casing()
    {
        // Arrange
        var json = """
                   {
                     "id": "trace-456",
                     "name": "Deserialization Test",
                     "version": "2.0.0",
                     "public": false,
                     "environment": "staging"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<CreateTraceBody>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Version.ShouldBe("2.0.0");
        result.Public.ShouldBe(false);
        result.Environment.ShouldBe("staging");
    }

    #endregion

    #region Usage Tests

    [Fact]
    public void Usage_Should_Serialize_With_Correct_PropertyNames()
    {
        // Arrange
        var usage = new Usage
        {
            Input = 100,
            Output = 50,
            Total = 150,
            Unit = "TOKENS",
            InputCost = 0.01,
            OutputCost = 0.02,
            TotalCost = 0.03
        };

        // Act
        var json = JsonSerializer.Serialize(usage, JsonOptions);

        // Assert - verify renamed properties
        json.ShouldContain("\"input\":100");
        json.ShouldContain("\"output\":50");
        json.ShouldContain("\"total\":150");
        json.ShouldContain("\"unit\":");
        json.ShouldContain("\"inputCost\":0.01");
        json.ShouldContain("\"outputCost\":0.02");
        json.ShouldContain("\"totalCost\":0.03");
    }

    [Fact]
    public void Usage_Should_Deserialize_With_Correct_PropertyNames()
    {
        // Arrange
        var json = """
                   {
                     "input": 200,
                     "output": 100,
                     "total": 300,
                     "unit": "TOKENS",
                     "inputCost": 0.05,
                     "outputCost": 0.10,
                     "totalCost": 0.15
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<Usage>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Input.ShouldBe(200);
        result.Output.ShouldBe(100);
        result.Total.ShouldBe(300);
        result.Unit.ShouldBe("TOKENS");
        result.InputCost.ShouldBe(0.05);
        result.OutputCost.ShouldBe(0.10);
        result.TotalCost.ShouldBe(0.15);
    }

    #endregion

    #region Score Tests

    [Fact]
    public void ScoreModel_Should_Deserialize_With_NewProperties()
    {
        // Arrange
        var json = """
                   {
                     "id": "score-123",
                     "name": "accuracy",
                     "value": 0.95,
                     "traceId": "trace-456",
                     "datasetRunId": "run-789",
                     "queueId": "queue-abc",
                     "environment": "production",
                     "source": "API"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<ScoreModel>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.DatasetRunId.ShouldBe("run-789");
        result.QueueId.ShouldBe("queue-abc");
        result.Environment.ShouldBe("production");
    }

    [Fact]
    public void CreateScoreEventBody_Should_Serialize_With_NewProperties()
    {
        // Arrange
        var body = new CreateScoreEventBody
        {
            Id = "score-123",
            TraceId = "trace-456",
            Name = "test-score",
            Value = 0.8,
            SessionId = "session-abc",
            DatasetRunId = "run-xyz",
            Environment = "staging",
            QueueId = "queue-123",
            Metadata = new { key = "value" }
        };

        // Act
        var json = JsonSerializer.Serialize(body, JsonOptions);

        // Assert
        json.ShouldContain("\"sessionId\":\"session-abc\"");
        json.ShouldContain("\"datasetRunId\":\"run-xyz\"");
        json.ShouldContain("\"environment\":\"staging\"");
        json.ShouldContain("\"queueId\":\"queue-123\"");
        json.ShouldContain("\"metadata\":");
    }

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

    #region TraceModel Default Values Tests

    [Fact]
    public void TraceModel_DefaultValues_TagsIsEmptyArray()
    {
        var trace = new TraceModel();

        trace.Tags.ShouldNotBeNull();
        trace.Tags.ShouldBeEmpty();
    }

    [Fact]
    public void TraceModel_DefaultValues_PublicIsFalse()
    {
        var trace = new TraceModel();

        trace.Public.ShouldBeFalse();
    }

    [Fact]
    public void TraceModel_DefaultValues_EnvironmentIsEmptyString()
    {
        var trace = new TraceModel();

        trace.Environment.ShouldNotBeNull();
        trace.Environment.ShouldBe(string.Empty);
    }

    [Fact]
    public void TraceModel_Should_Deserialize_NonNullableFields()
    {
        var json = """
                   {
                     "id": "trace-123",
                     "tags": ["tag1", "tag2"],
                     "public": true,
                     "environment": "production"
                   }
                   """;

        var result = JsonSerializer.Deserialize<TraceModel>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.Tags.Length.ShouldBe(2);
        result.Tags.ShouldContain("tag1");
        result.Public.ShouldBeTrue();
        result.Environment.ShouldBe("production");
    }

    #endregion

    #region TraceWithDetails Latency/TotalCost Tests

    [Fact]
    public void TraceWithDetails_Should_Deserialize_Latency_And_TotalCost()
    {
        var json = """
                   {
                     "id": "trace-123",
                     "tags": [],
                     "public": false,
                     "environment": "default",
                     "latency": 1.234,
                     "totalCost": 0.0056
                   }
                   """;

        var result = JsonSerializer.Deserialize<TraceWithDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.Latency.ShouldBe(1.234);
        result.TotalCost.ShouldBe(0.0056);
    }

    [Fact]
    public void TraceWithDetails_Should_Deserialize_NullLatency_And_TotalCost()
    {
        var json = """
                   {
                     "id": "trace-123",
                     "tags": [],
                     "public": false,
                     "environment": "default",
                     "latency": null,
                     "totalCost": null
                   }
                   """;

        var result = JsonSerializer.Deserialize<TraceWithDetails>(json, JsonOptions);

        result.ShouldNotBeNull();
        result.Latency.ShouldBeNull();
        result.TotalCost.ShouldBeNull();
    }

    [Fact]
    public void TraceWithDetails_Should_Serialize_Latency_And_TotalCost()
    {
        var trace = new TraceWithDetails
        {
            Id = "trace-123",
            Latency = 2.5,
            TotalCost = 0.01
        };

        var json = JsonSerializer.Serialize(trace, JsonOptions);

        json.ShouldContain("\"latency\":2.5");
        json.ShouldContain("\"totalCost\":0.01");
    }

    #endregion

    #region ObservationModel Default Values Tests

    [Fact]
    public void ObservationModel_DefaultValues_UsageDetailsIsEmptyDictionary()
    {
        var observation = new ObservationModel();

        observation.UsageDetails.ShouldNotBeNull();
        observation.UsageDetails.ShouldBeEmpty();
    }

    [Fact]
    public void ObservationModel_DefaultValues_CostDetailsIsEmptyDictionary()
    {
        var observation = new ObservationModel();

        observation.CostDetails.ShouldNotBeNull();
        observation.CostDetails.ShouldBeEmpty();
    }

    [Fact]
    public void ObservationModel_DefaultValues_EnvironmentIsEmptyString()
    {
        var observation = new ObservationModel();

        observation.Environment.ShouldNotBeNull();
        observation.Environment.ShouldBe(string.Empty);
    }

    #endregion

    #region Usage Non-Nullable Tests

    [Fact]
    public void Usage_DefaultValues_InputOutputTotalAreZero()
    {
        var usage = new Usage();

        usage.Input.ShouldBe(0);
        usage.Output.ShouldBe(0);
        usage.Total.ShouldBe(0);
    }

    [Fact]
    public void Usage_Unit_IsStringType()
    {
        var usage = new Usage { Unit = "TOKENS" };

        usage.Unit.ShouldBe("TOKENS");
    }

    [Fact]
    public void Usage_Unit_CanBeNull()
    {
        var usage = new Usage { Unit = null };

        usage.Unit.ShouldBeNull();
    }

    [Fact]
    public void Usage_NullUnit_OmittedInSerialization()
    {
        var usage = new Usage
        {
            Input = 10,
            Output = 5,
            Total = 15,
            Unit = null
        };

        var json = JsonSerializer.Serialize(usage, JsonOptions);

        json.ShouldNotContain("\"unit\"");
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

    #region CreateDatasetRunItemRequest DatasetVersion Tests

    [Fact]
    public void CreateDatasetRunItemRequest_Should_Serialize_DatasetVersion()
    {
        var date = new DateTime(2024, 6, 15, 10, 0, 0, DateTimeKind.Utc);
        var request = new CreateDatasetRunItemRequest
        {
            DatasetItemId = "item-1",
            DatasetVersion = date
        };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldContain("\"datasetVersion\":");
        json.ShouldContain("2024-06-15");
    }

    [Fact]
    public void CreateDatasetRunItemRequest_NullDatasetVersion_OmittedInSerialization()
    {
        var request = new CreateDatasetRunItemRequest { DatasetItemId = "item-1" };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldNotContain("\"datasetVersion\"");
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
        var property = typeof(ObservationsV2Request).GetProperty(nameof(ObservationsV2Request.ParseIoAsJson));

        property.ShouldNotBeNull();
        var obsoleteAttr = property.GetCustomAttributes(typeof(ObsoleteAttribute), false);
        obsoleteAttr.ShouldNotBeEmpty();
    }

    #endregion

    #region ScoreListRequest New Parameters Tests

    [Fact]
    public void ScoreListRequest_Should_Serialize_ObservationId()
    {
        var request = new ScoreListRequest { ObservationId = "obs-1,obs-2" };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldContain("\"observationId\":\"obs-1,obs-2\"");
    }

    [Fact]
    public void ScoreListRequest_Should_Serialize_Fields()
    {
        var request = new ScoreListRequest { Fields = "score,trace" };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldContain("\"fields\":\"score,trace\"");
    }

    [Fact]
    public void ScoreListRequest_Should_Serialize_Filter()
    {
        var request = new ScoreListRequest { Filter = "[{\"type\":\"string\",\"column\":\"name\"}]" };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldContain("\"filter\":");
    }

    [Fact]
    public void ScoreListRequest_NullNewParams_OmittedInSerialization()
    {
        var request = new ScoreListRequest { Page = 1 };

        var json = JsonSerializer.Serialize(request, JsonOptions);

        json.ShouldNotContain("\"observationId\"");
        json.ShouldNotContain("\"fields\"");
        json.ShouldNotContain("\"filter\"");
    }

    #endregion
}