using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using zborek.Langfuse.Models.Observation;

namespace zborek.Langfuse.Tests.Models;

public class ObservationOtelMetadataTests
{
    private const string SampleMetadataJson = """
                                              {
                                                  "attributes": {
                                                      "langfuse.user.id": "user-baggage-b284c82f519c4676a6adf90f4a1c3f76",
                                                      "langfuse.session.id": "session-baggage-e412466d1f464d889988ca29c57f3171",
                                                      "langfuse.trace.tags": "[\"baggage-tag1\",\"baggage-tag2\"]",
                                                      "langfuse.observation.type": "generation",
                                                      "gen_ai.operation.name": "chat",
                                                      "gen_ai.provider.name": "openai",
                                                      "gen_ai.request.model": "gpt-4",
                                                      "langfuse.observation.metadata.test": "test-value",
                                                      "langfuse.observation.metadata.custom_key": "custom_value",
                                                      "gen_ai.response.model": "gpt-4",
                                                      "gen_ai.usage.input_tokens": "50",
                                                      "gen_ai.usage.output_tokens": "25"
                                                  },
                                                  "resourceAttributes": {
                                                      "service.name": "unknown_service:dotnet",
                                                      "service.version": "1.0.0"
                                                  },
                                                  "scope": {
                                                      "name": "Langfuse",
                                                      "attributes": {}
                                                  }
                                              }
                                              """;

    [Fact]
    public void Should_Deserialize_OtelMetadata_From_Json()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata.ShouldNotBeNull();
        metadata.Attributes.ShouldNotBeNull();
        metadata.ResourceAttributes.ShouldNotBeNull();
        metadata.Scope.ShouldNotBeNull();
    }

    [Fact]
    public void Should_Get_UserId_From_Attributes()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.UserId.ShouldBe("user-baggage-b284c82f519c4676a6adf90f4a1c3f76");
    }

    [Fact]
    public void Should_Get_SessionId_From_Attributes()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.SessionId.ShouldBe("session-baggage-e412466d1f464d889988ca29c57f3171");
    }

    [Fact]
    public void Should_Parse_Tags_From_Json_Array_String()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.Tags.ShouldNotBeNull();
        metadata.Tags.Count.ShouldBe(2);
        metadata.Tags.ShouldContain("baggage-tag1");
        metadata.Tags.ShouldContain("baggage-tag2");
    }

    [Fact]
    public void Should_Extract_Custom_Metadata()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.CustomMetadata.ShouldNotBeNull();
        metadata.CustomMetadata.Count.ShouldBe(2);
        metadata.CustomMetadata["test"].ShouldBe("test-value");
        metadata.CustomMetadata["custom_key"].ShouldBe("custom_value");
    }

    [Fact]
    public void Should_Extract_GenAi_Attributes()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.GenAiAttributes.ShouldNotBeNull();
        metadata.GenAiAttributes.Count.ShouldBe(6);
        metadata.GenAiAttributes["operation.name"].ShouldBe("chat");
        metadata.GenAiAttributes["provider.name"].ShouldBe("openai");
        metadata.GenAiAttributes["request.model"].ShouldBe("gpt-4");
        metadata.GenAiAttributes["response.model"].ShouldBe("gpt-4");
        metadata.GenAiAttributes["usage.input_tokens"].ShouldBe("50");
        metadata.GenAiAttributes["usage.output_tokens"].ShouldBe("25");
    }

    [Fact]
    public void Should_Get_Resource_Attribute()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.GetResourceAttribute("service.name").ShouldBe("unknown_service:dotnet");
        metadata.GetResourceAttribute("service.version").ShouldBe("1.0.0");
    }

    [Fact]
    public void Should_Get_Scope_Name()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.Scope!.Name.ShouldBe("Langfuse");
    }

    [Fact]
    public void Should_Return_Null_For_Missing_Attribute()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.GetAttribute("non.existent.key").ShouldBeNull();
    }

    [Fact]
    public void Should_Return_Null_For_Missing_Resource_Attribute()
    {
        var metadata = JsonSerializer.Deserialize<ObservationOtelMetadata>(SampleMetadataJson);

        metadata!.GetResourceAttribute("non.existent.key").ShouldBeNull();
    }

    [Fact]
    public void Should_Return_Null_UserId_When_Not_Present()
    {
        var metadata = new ObservationOtelMetadata
        {
            Attributes = new Dictionary<string, string>()
        };

        metadata.UserId.ShouldBeNull();
    }

    [Fact]
    public void Should_Return_Null_SessionId_When_Not_Present()
    {
        var metadata = new ObservationOtelMetadata
        {
            Attributes = new Dictionary<string, string>()
        };

        metadata.SessionId.ShouldBeNull();
    }

    [Fact]
    public void Should_Return_Null_Tags_When_Not_Present()
    {
        var metadata = new ObservationOtelMetadata
        {
            Attributes = new Dictionary<string, string>()
        };

        metadata.Tags.ShouldBeNull();
    }

    [Fact]
    public void Should_Return_Null_Tags_For_Invalid_Json()
    {
        var metadata = new ObservationOtelMetadata
        {
            Attributes = new Dictionary<string, string>
            {
                ["langfuse.trace.tags"] = "not-valid-json"
            }
        };

        metadata.Tags.ShouldBeNull();
    }

    [Fact]
    public void Should_Return_Empty_CustomMetadata_When_None_Present()
    {
        var metadata = new ObservationOtelMetadata
        {
            Attributes = new Dictionary<string, string>
            {
                ["some.other.key"] = "value"
            }
        };

        metadata.CustomMetadata.ShouldNotBeNull();
        metadata.CustomMetadata.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Return_Empty_GenAiAttributes_When_None_Present()
    {
        var metadata = new ObservationOtelMetadata
        {
            Attributes = new Dictionary<string, string>
            {
                ["some.other.key"] = "value"
            }
        };

        metadata.GenAiAttributes.ShouldNotBeNull();
        metadata.GenAiAttributes.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Handle_Null_Attributes()
    {
        var metadata = new ObservationOtelMetadata();

        metadata.UserId.ShouldBeNull();
        metadata.SessionId.ShouldBeNull();
        metadata.Tags.ShouldBeNull();
        metadata.CustomMetadata.ShouldBeEmpty();
        metadata.GenAiAttributes.ShouldBeEmpty();
        metadata.GetAttribute("any.key").ShouldBeNull();
    }

    [Fact]
    public void Should_Handle_Null_ResourceAttributes()
    {
        var metadata = new ObservationOtelMetadata();

        metadata.GetResourceAttribute("any.key").ShouldBeNull();
    }
}

public class ObservationModelMetadataTests
{
    private const string SampleMetadataJson = """
                                              {
                                                  "attributes": {
                                                      "langfuse.user.id": "user-123",
                                                      "gen_ai.provider.name": "openai"
                                                  },
                                                  "resourceAttributes": {
                                                      "service.name": "test-service"
                                                  },
                                                  "scope": {
                                                      "name": "Langfuse"
                                                  }
                                              }
                                              """;

    [Fact]
    public void Should_Parse_Metadata_From_JsonElement()
    {
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(SampleMetadataJson);
        var observation = new ObservationModel
        {
            Id = "test-id",
            Type = "GENERATION",
            Metadata = jsonElement
        };

        var otelMetadata = observation.GetOtelMetadata();

        otelMetadata.ShouldNotBeNull();
        otelMetadata.UserId.ShouldBe("user-123");
        otelMetadata.GenAiAttributes["provider.name"].ShouldBe("openai");
        otelMetadata.GetResourceAttribute("service.name").ShouldBe("test-service");
    }

    [Fact]
    public void Should_Return_Null_When_Metadata_Is_Null()
    {
        var observation = new ObservationModel
        {
            Id = "test-id",
            Type = "GENERATION",
            Metadata = null
        };

        var otelMetadata = observation.GetOtelMetadata();

        otelMetadata.ShouldBeNull();
    }

    [Fact]
    public void Should_Return_Null_For_Invalid_Metadata_Structure()
    {
        var jsonElement = JsonSerializer.Deserialize<JsonElement>("\"just-a-string\"");
        var observation = new ObservationModel
        {
            Id = "test-id",
            Type = "GENERATION",
            Metadata = jsonElement
        };

        var otelMetadata = observation.GetOtelMetadata();

        otelMetadata.ShouldBeNull();
    }

    [Fact]
    public void Should_Return_Already_Typed_Object()
    {
        var expectedMetadata = new ObservationOtelMetadata
        {
            Attributes = new Dictionary<string, string>
            {
                ["langfuse.user.id"] = "already-typed-user"
            }
        };
        var observation = new ObservationModel
        {
            Id = "test-id",
            Type = "GENERATION",
            Metadata = expectedMetadata
        };

        var otelMetadata = observation.GetOtelMetadata();

        otelMetadata.ShouldBeSameAs(expectedMetadata);
        otelMetadata!.UserId.ShouldBe("already-typed-user");
    }

    [Fact]
    public void Should_Parse_Custom_Type_With_GetMetadataAs()
    {
        var json = """{"CustomField": "custom-value", "Number": 42}""";
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
        var observation = new ObservationModel
        {
            Id = "test-id",
            Type = "GENERATION",
            Metadata = jsonElement
        };

        var customMetadata = observation.GetMetadataAs<CustomMetadata>();

        customMetadata.ShouldNotBeNull();
        customMetadata.CustomField.ShouldBe("custom-value");
        customMetadata.Number.ShouldBe(42);
    }

    [Fact]
    public void Should_Return_Null_For_Invalid_Custom_Type()
    {
        var json = """{"wrongField": "value"}""";
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
        var observation = new ObservationModel
        {
            Id = "test-id",
            Type = "GENERATION",
            Metadata = jsonElement
        };

        // This should still succeed but with null/default values for missing properties
        var customMetadata = observation.GetMetadataAs<CustomMetadata>();

        customMetadata.ShouldNotBeNull();
        customMetadata.CustomField.ShouldBeNull();
        customMetadata.Number.ShouldBe(0);
    }

    [Fact]
    public void Should_Handle_Object_Metadata_By_Serializing_And_Deserializing()
    {
        var anonymousMetadata = new
        {
            attributes = new Dictionary<string, string>
            {
                ["langfuse.user.id"] = "anonymous-user"
            }
        };
        var observation = new ObservationModel
        {
            Id = "test-id",
            Type = "GENERATION",
            Metadata = anonymousMetadata
        };

        var otelMetadata = observation.GetOtelMetadata();

        otelMetadata.ShouldNotBeNull();
        otelMetadata.UserId.ShouldBe("anonymous-user");
    }

    [Fact]
    public void Should_Deserialize_Full_Observation_With_Metadata()
    {
        var observationJson = """
                              {
                                  "id": "obs-123",
                                  "type": "GENERATION",
                                  "name": "test-generation",
                                  "metadata": {
                                      "attributes": {
                                          "langfuse.user.id": "user-from-json",
                                          "langfuse.session.id": "session-from-json"
                                      },
                                      "resourceAttributes": {
                                          "service.name": "my-service"
                                      },
                                      "scope": {
                                          "name": "Langfuse"
                                      }
                                  }
                              }
                              """;

        var observation = JsonSerializer.Deserialize<ObservationModel>(observationJson);

        observation.ShouldNotBeNull();
        observation.Id.ShouldBe("obs-123");

        var otelMetadata = observation.GetOtelMetadata();
        otelMetadata.ShouldNotBeNull();
        otelMetadata.UserId.ShouldBe("user-from-json");
        otelMetadata.SessionId.ShouldBe("session-from-json");
        otelMetadata.GetResourceAttribute("service.name").ShouldBe("my-service");
        otelMetadata.Scope!.Name.ShouldBe("Langfuse");
    }
    
    [Fact]
    public void Should_Deserialize_Full_Observation_With_Custom_Metadata()
    {
        var observationJson = """
                              {
                                  "id": "obs-123",
                                  "type": "GENERATION",
                                  "name": "test-generation",
                                  "metadata": {
                                      "customField": "customValue",
                                      "number": 42,
                                      "attributes": {
                                          "langfuse.user.id": "user-from-json",
                                          "langfuse.session.id": "session-from-json"
                                      },
                                      "resourceAttributes": {
                                          "service.name": "my-service"
                                      },
                                      "scope": {
                                          "name": "Langfuse"
                                      }
                                  }
                              }
                              """;

        var observation = JsonSerializer.Deserialize<ObservationModel>(observationJson);

        observation.ShouldNotBeNull();
        observation.Id.ShouldBe("obs-123");

        var otelMetadata = observation.GetMetadataAs<CustomFields>();
        otelMetadata.ShouldNotBeNull();
        otelMetadata.UserId.ShouldBe("user-from-json");
        otelMetadata.SessionId.ShouldBe("session-from-json");
        otelMetadata.GetResourceAttribute("service.name").ShouldBe("my-service");
        otelMetadata.Scope!.Name.ShouldBe("Langfuse");
        otelMetadata.CustomField.ShouldBe("customValue");
        otelMetadata.Number.ShouldBe(42);
    }

    private class CustomMetadata
    {
        public string? CustomField { get; set; }
        public int Number { get; set; }
    }

    private class CustomFields : ObservationOtelMetadata
    {
        [JsonPropertyName("customField")]
        public string? CustomField { get; set; }
        
        [JsonPropertyName("number")]
        public int Number { get; set; }
    }
}