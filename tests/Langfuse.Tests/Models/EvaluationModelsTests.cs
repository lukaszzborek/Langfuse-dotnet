using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Tests.Models;

public class EvaluationModelsTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [Fact]
    public void Should_RoundTrip_PromptVariableMappingSource()
    {
        JsonSerializer.Serialize(PromptVariableMappingSource.Input).ShouldBe("\"input\"");
        JsonSerializer.Serialize(PromptVariableMappingSource.Output).ShouldBe("\"output\"");
        JsonSerializer.Serialize(PromptVariableMappingSource.Metadata).ShouldBe("\"metadata\"");
        JsonSerializer.Serialize(PromptVariableMappingSource.Tool_Calls).ShouldBe("\"tool_calls\"");
        JsonSerializer.Serialize(PromptVariableMappingSource.Expected_Output).ShouldBe("\"expected_output\"");
        JsonSerializer.Serialize(PromptVariableMappingSource.Experiment_Item_Metadata)
            .ShouldBe("\"experiment_item_metadata\"");
        JsonSerializer.Deserialize<PromptVariableMappingSource>("\"tool_calls\"")
            .ShouldBe(PromptVariableMappingSource.Tool_Calls);
    }

    [Fact]
    public void Should_RoundTrip_Enums()
    {
        JsonSerializer.Serialize(EvaluatorType.Llm_As_Judge).ShouldBe("\"llm_as_judge\"");
        JsonSerializer.Deserialize<EvaluatorType>("\"code\"").ShouldBe(EvaluatorType.Code);

        JsonSerializer.Serialize(EvaluatorStatus.Paused).ShouldBe("\"paused\"");
        JsonSerializer.Deserialize<EvaluatorStatus>("\"active\"").ShouldBe(EvaluatorStatus.Active);

        JsonSerializer.Serialize(EvaluatorChatMessageRole.System).ShouldBe("\"system\"");
        JsonSerializer.Deserialize<EvaluatorChatMessageRole>("\"assistant\"")
            .ShouldBe(EvaluatorChatMessageRole.Assistant);

        JsonSerializer.Serialize(CodeEvaluatorSourceCodeLanguage.Typescript).ShouldBe("\"TYPESCRIPT\"");
        JsonSerializer.Deserialize<CodeEvaluatorSourceCodeLanguage>("\"PYTHON\"")
            .ShouldBe(CodeEvaluatorSourceCodeLanguage.Python);

        JsonSerializer.Serialize(EvaluatorOutputScoreType.Categorical).ShouldBe("\"CATEGORICAL\"");
        JsonSerializer.Deserialize<EvaluatorOutputScoreType>("\"BOOLEAN\"").ShouldBe(EvaluatorOutputScoreType.Boolean);

        JsonSerializer.Serialize(LegacyEvaluationObject.Dataset_Item).ShouldBe("\"dataset_item\"");
        JsonSerializer.Deserialize<LegacyEvaluationObject>("\"generation\"")
            .ShouldBe(LegacyEvaluationObject.Generation);
    }

    [Fact]
    public void Should_Serialize_Prompt_Input_As_String_Or_Messages()
    {
        var textJson = JsonSerializer.Serialize(new CreateLlmAsJudgeEvaluatorRequest
        {
            Name = "helpfulness",
            Prompt = "Rate {{input}}",
            OutputDefinition = new NumericEvaluatorOutputDefinition()
        }, Options);

        textJson.ShouldContain("\"type\":\"llm_as_judge\"");
        textJson.ShouldContain("\"prompt\":\"Rate {{input}}\"");
        textJson.ShouldContain("\"outputDefinition\":{\"dataType\":\"NUMERIC\"}");
        textJson.ShouldNotContain("modelConfig");
        textJson.ShouldNotContain("variableMapping");

        var messagesJson = JsonSerializer.Serialize(new CreateLlmAsJudgeEvaluatorRequest
        {
            Name = "helpfulness",
            Prompt = new[]
            {
                new EvaluatorChatMessage { Role = EvaluatorChatMessageRole.System, Content = "You judge." },
                new EvaluatorChatMessage { Role = EvaluatorChatMessageRole.User, Content = "Rate {{input}}" }
            },
            OutputDefinition = new CategoricalEvaluatorOutputDefinition
            {
                Categories = new[] { "good", "bad" },
                ShouldAllowMultipleMatches = false
            }
        }, Options);

        messagesJson.ShouldContain(
            "\"prompt\":[{\"role\":\"system\",\"content\":\"You judge.\"},{\"role\":\"user\",\"content\":\"Rate {{input}}\"}]");
        messagesJson.ShouldContain("\"categories\":[\"good\",\"bad\"]");
        messagesJson.ShouldContain("\"shouldAllowMultipleMatches\":false");
    }

    [Fact]
    public void Should_Deserialize_Prompt_Input_From_Either_Shape()
    {
        var fromText = JsonSerializer.Deserialize<EvaluatorChatPromptInput>("\"Rate {{input}}\"");
        fromText.ShouldNotBeNull();
        fromText.Text.ShouldBe("Rate {{input}}");
        fromText.Messages.ShouldBeNull();

        var fromMessages = JsonSerializer.Deserialize<EvaluatorChatPromptInput>(
            "[{\"role\":\"user\",\"content\":\"hi\"}]");
        fromMessages.ShouldNotBeNull();
        fromMessages.Text.ShouldBeNull();
        fromMessages.Messages.ShouldNotBeNull();
        fromMessages.Messages.Single().Role.ShouldBe(EvaluatorChatMessageRole.User);
    }

    [Fact]
    public void Should_Serialize_CreateCodeEvaluatorRequest_With_Type_Discriminator()
    {
        var codeJson = JsonSerializer.Serialize(new CreateCodeEvaluatorRequest
        {
            Name = "length-check",
            Description = "counts chars",
            SourceCode = "export default () => 1",
            SourceCodeLanguage = CodeEvaluatorSourceCodeLanguage.Typescript
        }, Options);

        codeJson.ShouldContain("\"type\":\"code\"");
        codeJson.ShouldContain("\"description\":\"counts chars\"");
        codeJson.ShouldContain("\"sourceCodeLanguage\":\"TYPESCRIPT\"");
    }

    [Fact]
    public void Should_Serialize_UpdateEvaluatorRequests()
    {
        var metadataJson = JsonSerializer.Serialize<object>(
            new UpdateEvaluatorMetadataRequest { Name = "renamed" }, Options);
        metadataJson.ShouldBe("{\"name\":\"renamed\"}");

        var codeJson = JsonSerializer.Serialize<object>(new UpdateCodeEvaluatorRequest
        {
            SourceCode = "def evaluate(**kwargs): return 1",
            SourceCodeLanguage = CodeEvaluatorSourceCodeLanguage.Python
        }, Options);
        codeJson.ShouldContain("\"type\":\"code\"");
        codeJson.ShouldContain("\"sourceCode\":\"def evaluate(**kwargs): return 1\"");
        codeJson.ShouldNotContain("name");

        var llmJson = JsonSerializer.Serialize<object>(new UpdateLlmAsJudgeEvaluatorRequest
        {
            Prompt = "Rate {{input}}",
            ModelConfig = new EvaluatorModelConfig { Provider = "openai", Model = "gpt-4.1-mini" },
            VariableMapping = new[]
            {
                new PromptVariableMappingInput
                {
                    Variable = "input", Source = PromptVariableMappingSource.Metadata, JsonPath = "$.question"
                }
            },
            OutputDefinition = new NumericEvaluatorOutputDefinition { MinValue = 0, MaxValue = 1 }
        }, Options);
        llmJson.ShouldContain("\"type\":\"llm_as_judge\"");
        llmJson.ShouldContain("\"modelConfig\":{\"provider\":\"openai\",\"model\":\"gpt-4.1-mini\"}");
        llmJson.ShouldContain(
            "\"variableMapping\":[{\"variable\":\"input\",\"source\":\"metadata\",\"jsonPath\":\"$.question\"}]");
        llmJson.ShouldContain("\"minValue\":0");
        llmJson.ShouldContain("\"maxValue\":1");
    }

    [Fact]
    public void Should_Deserialize_LlmAsJudgeEvaluator_When_Type_Is_Not_First_Property()
    {
        // "type" is intentionally the last property: the converter must not rely on discriminator order
        var json = @"{
            ""id"": ""ev-1"",
            ""name"": ""tone"",
            ""description"": null,
            ""createdBy"": { ""id"": ""user-1"", ""name"": null },
            ""status"": ""paused"",
            ""pausedAt"": ""2024-01-03T00:00:00Z"",
            ""pausedReason"": ""model_not_found"",
            ""pausedMessage"": ""Model is not available"",
            ""evaluationRuleAssignments"": [
                { ""evaluationRuleId"": ""rule-1"" },
                { ""evaluationRuleId"": ""rule-legacy"", ""variableMappingOverride"": [
                    { ""mappingType"": ""legacy"", ""variable"": ""input"", ""langfuseObject"": ""trace"", ""objectName"": null, ""source"": ""input"" }
                ] }
            ],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-02T00:00:00Z"",
            ""versionId"": ""ver-2"",
            ""version"": 2,
            ""versionCreatedAt"": ""2024-01-02T00:00:00Z"",
            ""versionCreatedBy"": null,
            ""prompt"": [{ ""role"": ""user"", ""content"": ""Classify {{input}}"" }],
            ""variables"": [""input""],
            ""variableMapping"": [{ ""variable"": ""input"", ""source"": ""input"", ""jsonPath"": null }],
            ""modelConfig"": null,
            ""outputDefinition"": {
                ""dataType"": ""CATEGORICAL"",
                ""categories"": [""positive"", ""negative""],
                ""shouldAllowMultipleMatches"": false
            },
            ""type"": ""llm_as_judge""
        }";

        var evaluator = JsonSerializer.Deserialize<Evaluator>(json);

        var llmEvaluator = evaluator.ShouldBeOfType<LlmAsJudgeEvaluator>();
        llmEvaluator.Version.ShouldBe(2);
        llmEvaluator.VersionId.ShouldBe("ver-2");
        llmEvaluator.Status.ShouldBe(EvaluatorStatus.Paused);
        llmEvaluator.PausedReason.ShouldBe("model_not_found");
        llmEvaluator.CreatedBy.ShouldNotBeNull();
        llmEvaluator.CreatedBy.Id.ShouldBe("user-1");
        llmEvaluator.CreatedBy.Name.ShouldBeNull();
        llmEvaluator.Type.ShouldBe(EvaluatorType.Llm_As_Judge);
        llmEvaluator.Prompt.Single().Content.ShouldBe("Classify {{input}}");
        llmEvaluator.VariableMapping.ShouldNotBeNull();
        llmEvaluator.VariableMapping.Single().Source.ShouldBe("input");
        llmEvaluator.VariableMapping.Single().IsLegacy.ShouldBeFalse();
        llmEvaluator.ModelConfig.ShouldBeNull();
        var llmOutput = llmEvaluator.OutputDefinition.ShouldBeOfType<CategoricalEvaluatorOutputDefinition>();
        llmOutput.DataType.ShouldBe(EvaluatorOutputScoreType.Categorical);
        llmOutput.Categories.ShouldBe(new[] { "positive", "negative" });
        llmOutput.ShouldAllowMultipleMatches.ShouldBe(false);

        llmEvaluator.EvaluationRuleAssignments.Length.ShouldBe(2);
        llmEvaluator.EvaluationRuleAssignments[0].VariableMappingOverride.ShouldBeNull();
        var legacy = llmEvaluator.EvaluationRuleAssignments[1].VariableMappingOverride.ShouldNotBeNull().Single();
        legacy.IsLegacy.ShouldBeTrue();
        legacy.LangfuseObject.ShouldBe(LegacyEvaluationObject.Trace);
        legacy.ObjectName.ShouldBeNull();
    }

    [Fact]
    public void Should_Deserialize_CodeEvaluator_And_Versions()
    {
        var json = @"{
            ""id"": ""ev-2"",
            ""name"": ""length-check"",
            ""description"": ""d"",
            ""createdBy"": null,
            ""status"": ""active"",
            ""pausedAt"": null,
            ""pausedReason"": null,
            ""pausedMessage"": null,
            ""evaluationRuleAssignments"": [],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z"",
            ""versionId"": ""ver-1"",
            ""version"": 1,
            ""versionCreatedAt"": ""2024-01-01T00:00:00Z"",
            ""versionCreatedBy"": null,
            ""sourceCode"": ""def evaluate(**kwargs): return 1"",
            ""sourceCodeLanguage"": ""PYTHON"",
            ""type"": ""code""
        }";

        var codeEvaluator = JsonSerializer.Deserialize<Evaluator>(json).ShouldBeOfType<CodeEvaluator>();
        codeEvaluator.Type.ShouldBe(EvaluatorType.Code);
        codeEvaluator.SourceCode.ShouldBe("def evaluate(**kwargs): return 1");
        codeEvaluator.SourceCodeLanguage.ShouldBe(CodeEvaluatorSourceCodeLanguage.Python);

        var versionsJson = @"{ ""data"": [
            { ""id"": ""ver-2"", ""version"": 2, ""createdAt"": ""2024-01-02T00:00:00Z"", ""createdBy"": null,
              ""type"": ""code"", ""sourceCode"": ""v2"", ""sourceCodeLanguage"": ""TYPESCRIPT"" },
            { ""id"": ""ver-1"", ""version"": 1, ""createdAt"": ""2024-01-01T00:00:00Z"", ""createdBy"": null,
              ""type"": ""llm_as_judge"", ""prompt"": [], ""variables"": [], ""variableMapping"": null,
              ""modelConfig"": { ""provider"": ""openai"", ""model"": ""gpt-4.1-mini"" },
              ""outputDefinition"": { ""dataType"": ""BOOLEAN"" } }
        ], ""meta"": { ""cursor"": null } }";

        var page = JsonSerializer.Deserialize<EvaluatorVersionsPage>(versionsJson);
        page.ShouldNotBeNull();
        page.Meta.Cursor.ShouldBeNull();
        page.Data[0].ShouldBeOfType<CodeEvaluatorVersion>().SourceCodeLanguage
            .ShouldBe(CodeEvaluatorSourceCodeLanguage.Typescript);
        var llmVersion = page.Data[1].ShouldBeOfType<LlmAsJudgeEvaluatorVersion>();
        llmVersion.ModelConfig.ShouldNotBeNull();
        llmVersion.ModelConfig.Provider.ShouldBe("openai");
        llmVersion.OutputDefinition.DataType.ShouldBe(EvaluatorOutputScoreType.Boolean);
    }

    [Fact]
    public void Should_Throw_When_Evaluator_Type_Is_Missing_Or_Unknown()
    {
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Evaluator>(@"{ ""id"": ""ev-1"", ""name"": ""x"" }"));

        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Evaluator>(@"{ ""id"": ""ev-1"", ""type"": ""human"" }"));

        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<EvaluatorVersion>(@"{ ""id"": ""ver-1"", ""type"": ""human"" }"));
    }

    [Fact]
    public void Should_Serialize_Evaluator_Declared_As_Base_Type_With_Derived_Properties()
    {
        Evaluator evaluator = new CodeEvaluator
        {
            Id = "ev-2",
            Name = "length-check",
            Status = EvaluatorStatus.Active,
            EvaluationRuleAssignments = Array.Empty<EvaluationRuleAssignment>(),
            SourceCode = "def evaluate(**kwargs): return 1",
            SourceCodeLanguage = CodeEvaluatorSourceCodeLanguage.Python,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            VersionId = "ver-1",
            Version = 1,
            VersionCreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var json = JsonSerializer.Serialize(evaluator);

        json.ShouldContain("\"type\":\"code\"");
        json.ShouldContain("\"sourceCode\":\"def evaluate(**kwargs): return 1\"");

        JsonSerializer.Deserialize<Evaluator>(json).ShouldBeOfType<CodeEvaluator>();
    }

    [Fact]
    public void Should_Serialize_CreateEvaluationRuleRequest_And_Omit_Optionals()
    {
        var request = new CreateEvaluationRuleRequest
        {
            Name = "rule",
            Enabled = true,
            EvaluatorAssignments = new[]
            {
                new EvaluationRuleEvaluatorAssignmentInput { EvaluatorId = "ev-1" },
                new EvaluationRuleEvaluatorAssignmentInput
                {
                    EvaluatorId = "ev-2",
                    VariableMapping = new[]
                    {
                        new PromptVariableMappingInput
                        {
                            Variable = "expected", Source = PromptVariableMappingSource.Expected_Output
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(request, Options);

        json.ShouldContain("\"name\":\"rule\"");
        json.ShouldContain("\"enabled\":true");
        json.ShouldContain("{\"evaluatorId\":\"ev-1\"}");
        json.ShouldContain("\"source\":\"expected_output\"");
        json.ShouldNotContain("sampling");
        json.ShouldNotContain("filter");
    }

    [Fact]
    public void Should_Serialize_EvaluationRule_Filters()
    {
        var request = new UpdateEvaluationRuleRequest
        {
            Filter = new EvaluationRuleFilter[]
            {
                new StringOptionsEvaluationRuleFilter
                {
                    Column = "type",
                    Operator = EvaluationRuleOptionsFilterOperator.AnyOf,
                    Value = new[] { "GENERATION" }
                },
                new StringObjectEvaluationRuleFilter
                {
                    Column = "metadata",
                    Key = "env",
                    Operator = EvaluationRuleStringFilterOperator.Equals,
                    Value = "prod"
                },
                new NullEvaluationRuleFilter
                {
                    Column = "parentObservationId", Operator = EvaluationRuleNullFilterOperator.IsNull
                },
                new BooleanEvaluationRuleFilter
                {
                    Column = "isExperimentItemRootSpan",
                    Operator = EvaluationRuleBooleanFilterOperator.Equals,
                    Value = true
                }
            }
        };

        var json = JsonSerializer.Serialize(request, Options);

        json.ShouldBe("{\"filter\":[" +
                      "{\"type\":\"stringOptions\",\"column\":\"type\",\"operator\":\"any of\",\"value\":[\"GENERATION\"]}," +
                      "{\"type\":\"stringObject\",\"column\":\"metadata\",\"key\":\"env\",\"operator\":\"=\",\"value\":\"prod\"}," +
                      "{\"type\":\"null\",\"column\":\"parentObservationId\",\"operator\":\"is null\",\"value\":\"\"}," +
                      "{\"type\":\"boolean\",\"column\":\"isExperimentItemRootSpan\",\"operator\":\"=\",\"value\":true}" +
                      "]}");
    }

    [Fact]
    public void Should_Deserialize_EvaluationRule()
    {
        var json = @"{
            ""id"": ""rule-1"",
            ""name"": ""My rule"",
            ""createdBy"": null,
            ""enabled"": true,
            ""sampling"": 0.5,
            ""filter"": [
                { ""type"": ""stringObject"", ""column"": ""metadata"", ""key"": ""env"", ""operator"": ""="", ""value"": ""prod"" },
                { ""type"": ""arrayOptions"", ""column"": ""tags"", ""operator"": ""any of"", ""value"": [""a"", ""b""] }
            ],
            ""evaluatorAssignments"": [
                { ""evaluatorId"": ""ev-1"", ""variableMapping"": null },
                { ""evaluatorId"": ""ev-2"", ""variableMapping"": [{ ""variable"": ""input"", ""source"": null }] }
            ],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-02T00:00:00Z""
        }";

        var rule = JsonSerializer.Deserialize<EvaluationRule>(json);

        rule.ShouldNotBeNull();
        rule.Sampling.ShouldBe(0.5);
        rule.Filter.Length.ShouldBe(2);
        rule.Filter[0].Key.ShouldBe("env");
        rule.Filter[0].Value.ShouldBeOfType<JsonElement>().GetString().ShouldBe("prod");
        rule.Filter[1].Key.ShouldBeNull();
        rule.Filter[1].Value.ShouldBeOfType<JsonElement>().GetArrayLength().ShouldBe(2);
        rule.EvaluatorAssignments[0].VariableMapping.ShouldBeNull();
        rule.EvaluatorAssignments[1].VariableMapping.ShouldNotBeNull().Single().Source.ShouldBeNull();
    }

    [Fact]
    public void Should_RoundTrip_DateTimeEvaluationRuleFilter()
    {
        var filter = new DateTimeEvaluationRuleFilter
        {
            Column = "createdAt",
            Operator = EvaluationRuleNumberFilterOperator.GreaterThanOrEqual,
            Value = "2024-01-01T00:00:00Z"
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe(
            "{\"type\":\"datetime\",\"column\":\"createdAt\",\"operator\":\"\\u003E=\",\"value\":\"2024-01-01T00:00:00Z\"}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<DateTimeEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("createdAt");
        deserialized.Operator.ShouldBe(EvaluationRuleNumberFilterOperator.GreaterThanOrEqual);
        deserialized.Value.ShouldBe("2024-01-01T00:00:00Z");
    }

    [Fact]
    public void Should_RoundTrip_StringEvaluationRuleFilter()
    {
        var filter = new StringEvaluationRuleFilter
        {
            Column = "userId",
            Operator = EvaluationRuleStringFilterOperator.StartsWith,
            Value = "user-"
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe("{\"type\":\"string\",\"column\":\"userId\",\"operator\":\"starts with\",\"value\":\"user-\"}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<StringEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("userId");
        deserialized.Operator.ShouldBe(EvaluationRuleStringFilterOperator.StartsWith);
        deserialized.Value.ShouldBe("user-");
    }

    [Fact]
    public void Should_RoundTrip_NumberEvaluationRuleFilter()
    {
        var filter = new NumberEvaluationRuleFilter
        {
            Column = "toolCalls",
            Operator = EvaluationRuleNumberFilterOperator.GreaterThan,
            Value = 3
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe("{\"type\":\"number\",\"column\":\"toolCalls\",\"operator\":\"\\u003E\",\"value\":3}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<NumberEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("toolCalls");
        deserialized.Operator.ShouldBe(EvaluationRuleNumberFilterOperator.GreaterThan);
        deserialized.Value.ShouldBe(3);
    }

    [Fact]
    public void Should_RoundTrip_StringOptionsEvaluationRuleFilter()
    {
        var filter = new StringOptionsEvaluationRuleFilter
        {
            Column = "type",
            Operator = EvaluationRuleOptionsFilterOperator.NoneOf,
            Value = new[] { "SPAN", "EVENT" }
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe(
            "{\"type\":\"stringOptions\",\"column\":\"type\",\"operator\":\"none of\",\"value\":[\"SPAN\",\"EVENT\"]}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<StringOptionsEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("type");
        deserialized.Operator.ShouldBe(EvaluationRuleOptionsFilterOperator.NoneOf);
        deserialized.Value.ShouldBe(new[] { "SPAN", "EVENT" });
    }

    [Fact]
    public void Should_RoundTrip_CategoryOptionsEvaluationRuleFilter()
    {
        var filter = new CategoryOptionsEvaluationRuleFilter
        {
            Column = "metadata",
            Key = "region",
            Operator = EvaluationRuleOptionsFilterOperator.AnyOf,
            Value = new[] { "eu", "us" }
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe(
            "{\"type\":\"categoryOptions\",\"column\":\"metadata\",\"key\":\"region\",\"operator\":\"any of\",\"value\":[\"eu\",\"us\"]}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<CategoryOptionsEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("metadata");
        deserialized.Key.ShouldBe("region");
        deserialized.Operator.ShouldBe(EvaluationRuleOptionsFilterOperator.AnyOf);
        deserialized.Value.ShouldBe(new[] { "eu", "us" });
    }

    [Fact]
    public void Should_RoundTrip_ArrayOptionsEvaluationRuleFilter()
    {
        var filter = new ArrayOptionsEvaluationRuleFilter
        {
            Column = "tags",
            Operator = EvaluationRuleArrayOptionsFilterOperator.AllOf,
            Value = new[] { "prod", "critical" }
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe(
            "{\"type\":\"arrayOptions\",\"column\":\"tags\",\"operator\":\"all of\",\"value\":[\"prod\",\"critical\"]}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<ArrayOptionsEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("tags");
        deserialized.Operator.ShouldBe(EvaluationRuleArrayOptionsFilterOperator.AllOf);
        deserialized.Value.ShouldBe(new[] { "prod", "critical" });
    }

    [Fact]
    public void Should_RoundTrip_StringObjectEvaluationRuleFilter()
    {
        var filter = new StringObjectEvaluationRuleFilter
        {
            Column = "metadata",
            Key = "env",
            Operator = EvaluationRuleStringFilterOperator.Equals,
            Value = "prod"
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe(
            "{\"type\":\"stringObject\",\"column\":\"metadata\",\"key\":\"env\",\"operator\":\"=\",\"value\":\"prod\"}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<StringObjectEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("metadata");
        deserialized.Key.ShouldBe("env");
        deserialized.Operator.ShouldBe(EvaluationRuleStringFilterOperator.Equals);
        deserialized.Value.ShouldBe("prod");
    }

    [Fact]
    public void Should_RoundTrip_NumberObjectEvaluationRuleFilter()
    {
        var filter = new NumberObjectEvaluationRuleFilter
        {
            Column = "metadata",
            Key = "score",
            Operator = EvaluationRuleNumberFilterOperator.LessThanOrEqual,
            Value = 0.5
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe(
            "{\"type\":\"numberObject\",\"column\":\"metadata\",\"key\":\"score\",\"operator\":\"\\u003C=\",\"value\":0.5}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<NumberObjectEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("metadata");
        deserialized.Key.ShouldBe("score");
        deserialized.Operator.ShouldBe(EvaluationRuleNumberFilterOperator.LessThanOrEqual);
        deserialized.Value.ShouldBe(0.5);
    }

    [Fact]
    public void Should_RoundTrip_BooleanEvaluationRuleFilter()
    {
        var filter = new BooleanEvaluationRuleFilter
        {
            Column = "isRootObservation",
            Operator = EvaluationRuleBooleanFilterOperator.NotEquals,
            Value = false
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe(
            "{\"type\":\"boolean\",\"column\":\"isRootObservation\",\"operator\":\"\\u003C\\u003E\",\"value\":false}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<BooleanEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("isRootObservation");
        deserialized.Operator.ShouldBe(EvaluationRuleBooleanFilterOperator.NotEquals);
        deserialized.Value.ShouldBeFalse();
    }

    [Fact]
    public void Should_RoundTrip_NullEvaluationRuleFilter()
    {
        var filter = new NullEvaluationRuleFilter
        {
            Column = "parentObservationId",
            Operator = EvaluationRuleNullFilterOperator.IsNotNull
        };

        var json = JsonSerializer.Serialize<EvaluationRuleFilter>(filter, Options);
        json.ShouldBe(
            "{\"type\":\"null\",\"column\":\"parentObservationId\",\"operator\":\"is not null\",\"value\":\"\"}");

        var deserialized = JsonSerializer.Deserialize<EvaluationRuleFilter>(json)
            .ShouldBeOfType<NullEvaluationRuleFilter>();
        deserialized.Column.ShouldBe("parentObservationId");
        deserialized.Operator.ShouldBe(EvaluationRuleNullFilterOperator.IsNotNull);
        deserialized.Value.ShouldBe("");
    }

    [Fact]
    public void Should_Deserialize_EvaluationRuleReadFilter()
    {
        var json =
            @"{ ""type"": ""stringObject"", ""column"": ""metadata"", ""key"": ""env"", ""operator"": ""="", ""value"": ""prod"" }";

        var filter = JsonSerializer.Deserialize<EvaluationRuleReadFilter>(json);

        filter.ShouldNotBeNull();
        filter.Type.ShouldBe("stringObject");
        filter.Column.ShouldBe("metadata");
        filter.Key.ShouldBe("env");
        filter.Operator.ShouldBe("=");
        filter.Value.ShouldBeOfType<JsonElement>().GetString().ShouldBe("prod");
    }
}