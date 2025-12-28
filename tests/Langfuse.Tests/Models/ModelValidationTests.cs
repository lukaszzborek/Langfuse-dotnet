using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using zborek.Langfuse.Models.AnnotationQueue;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Dataset;
using zborek.Langfuse.Models.Model;
using zborek.Langfuse.Models.Observation;
using zborek.Langfuse.Models.Organization;
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
        var result = JsonSerializer.Deserialize<zborek.Langfuse.Models.Model.Model>(json, JsonOptions);

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
            Unit = ModelUsageUnit.Tokens,
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
        result.Unit.ShouldBe(ModelUsageUnit.Tokens);
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

    #region Test Classes

    private class TestAnnotationObject
    {
        public AnnotationObjectType ObjectType { get; set; }
    }

    #endregion
}
