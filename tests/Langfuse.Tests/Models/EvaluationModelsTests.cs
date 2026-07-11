using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Tests.Models;

public class EvaluationModelsTests
{
    [Fact]
    public void Should_Serialize_EvaluationRuleMappingSource_To_Lowercase()
    {
        JsonSerializer.Serialize(EvaluationRuleMappingSource.Input).ShouldBe("\"input\"");
        JsonSerializer.Serialize(EvaluationRuleMappingSource.Output).ShouldBe("\"output\"");
        JsonSerializer.Serialize(EvaluationRuleMappingSource.Metadata).ShouldBe("\"metadata\"");
        JsonSerializer.Serialize(EvaluationRuleMappingSource.Expected_Output).ShouldBe("\"expected_output\"");
        JsonSerializer.Serialize(EvaluationRuleMappingSource.Experiment_Item_Metadata)
            .ShouldBe("\"experiment_item_metadata\"");
    }

    [Fact]
    public void Should_Deserialize_EvaluationRuleMappingSource_From_Lowercase()
    {
        JsonSerializer.Deserialize<EvaluationRuleMappingSource>("\"input\"")
            .ShouldBe(EvaluationRuleMappingSource.Input);
        JsonSerializer.Deserialize<EvaluationRuleMappingSource>("\"expected_output\"")
            .ShouldBe(EvaluationRuleMappingSource.Expected_Output);
        JsonSerializer.Deserialize<EvaluationRuleMappingSource>("\"experiment_item_metadata\"")
            .ShouldBe(EvaluationRuleMappingSource.Experiment_Item_Metadata);
    }

    [Fact]
    public void Should_RoundTrip_EvaluationRuleTarget()
    {
        JsonSerializer.Serialize(EvaluationRuleTarget.Observation).ShouldBe("\"observation\"");
        JsonSerializer.Serialize(EvaluationRuleTarget.Experiment).ShouldBe("\"experiment\"");
        JsonSerializer.Deserialize<EvaluationRuleTarget>("\"experiment\"").ShouldBe(EvaluationRuleTarget.Experiment);
    }

    [Fact]
    public void Should_RoundTrip_EvaluationRuleStatus()
    {
        JsonSerializer.Serialize(EvaluationRuleStatus.Active).ShouldBe("\"active\"");
        JsonSerializer.Serialize(EvaluationRuleStatus.Inactive).ShouldBe("\"inactive\"");
        JsonSerializer.Serialize(EvaluationRuleStatus.Paused).ShouldBe("\"paused\"");
        JsonSerializer.Deserialize<EvaluationRuleStatus>("\"paused\"").ShouldBe(EvaluationRuleStatus.Paused);
    }

    [Fact]
    public void Should_RoundTrip_EvaluatorScope()
    {
        JsonSerializer.Serialize(EvaluatorScope.Project).ShouldBe("\"project\"");
        JsonSerializer.Serialize(EvaluatorScope.Managed).ShouldBe("\"managed\"");
        JsonSerializer.Deserialize<EvaluatorScope>("\"managed\"").ShouldBe(EvaluatorScope.Managed);
    }

    [Fact]
    public void Should_RoundTrip_EvaluatorType()
    {
        JsonSerializer.Serialize(EvaluatorType.Llm_As_Judge).ShouldBe("\"llm_as_judge\"");
        JsonSerializer.Serialize(EvaluatorType.Code).ShouldBe("\"code\"");
        JsonSerializer.Deserialize<EvaluatorType>("\"llm_as_judge\"").ShouldBe(EvaluatorType.Llm_As_Judge);
        JsonSerializer.Deserialize<EvaluatorType>("\"code\"").ShouldBe(EvaluatorType.Code);
    }

    [Fact]
    public void Should_RoundTrip_CodeEvaluatorSourceCodeLanguage()
    {
        JsonSerializer.Serialize(CodeEvaluatorSourceCodeLanguage.Python).ShouldBe("\"PYTHON\"");
        JsonSerializer.Serialize(CodeEvaluatorSourceCodeLanguage.Typescript).ShouldBe("\"TYPESCRIPT\"");
        JsonSerializer.Deserialize<CodeEvaluatorSourceCodeLanguage>("\"PYTHON\"")
            .ShouldBe(CodeEvaluatorSourceCodeLanguage.Python);
        JsonSerializer.Deserialize<CodeEvaluatorSourceCodeLanguage>("\"TYPESCRIPT\"")
            .ShouldBe(CodeEvaluatorSourceCodeLanguage.Typescript);
    }

    [Fact]
    public void Should_Serialize_EvaluatorOutputDataType_To_Uppercase()
    {
        JsonSerializer.Serialize(EvaluatorOutputDataType.Numeric).ShouldBe("\"NUMERIC\"");
        JsonSerializer.Serialize(EvaluatorOutputDataType.Boolean).ShouldBe("\"BOOLEAN\"");
        JsonSerializer.Serialize(EvaluatorOutputDataType.Categorical).ShouldBe("\"CATEGORICAL\"");
        JsonSerializer.Deserialize<EvaluatorOutputDataType>("\"CATEGORICAL\"")
            .ShouldBe(EvaluatorOutputDataType.Categorical);
    }

    [Fact]
    public void Should_Serialize_CreateLlmAsJudgeEvaluationRuleRequest_And_Omit_Optionals()
    {
        var request = new CreateLlmAsJudgeEvaluationRuleRequest
        {
            Name = "rule",
            Evaluator = new LlmAsJudgeEvaluationRuleEvaluatorReference
            {
                Name = "helpfulness", Scope = EvaluatorScope.Managed
            },
            Target = EvaluationRuleTarget.Experiment,
            Enabled = true,
            Mapping = new[]
            {
                new EvaluationRuleMapping
                {
                    Variable = "expected",
                    Source = EvaluationRuleMappingSource.Expected_Output
                }
            }
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"name\":\"rule\"");
        json.ShouldContain("\"scope\":\"managed\"");
        json.ShouldContain("\"type\":\"llm_as_judge\"");
        json.ShouldContain("\"target\":\"experiment\"");
        json.ShouldContain("\"source\":\"expected_output\"");
        // sampling and filter are optional and omitted when null
        json.ShouldNotContain("sampling");
        json.ShouldNotContain("filter");
    }

    [Fact]
    public void Should_Serialize_CreateCodeEvaluationRuleRequest_Without_Mapping()
    {
        var request = new CreateCodeEvaluationRuleRequest
        {
            Name = "code rule",
            Evaluator = new CodeEvaluationRuleEvaluatorReference
            {
                Name = "length-check", Scope = EvaluatorScope.Project
            },
            Target = EvaluationRuleTarget.Observation,
            Enabled = false
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"name\":\"code rule\"");
        json.ShouldContain("\"type\":\"code\"");
        json.ShouldContain("\"target\":\"observation\"");
        json.ShouldNotContain("mapping");
    }

    [Fact]
    public void Should_Serialize_CreateEvaluatorRequests_With_Type_Discriminator()
    {
        var llmJson = JsonSerializer.Serialize(new CreateLlmAsJudgeEvaluatorRequest
        {
            Name = "helpfulness",
            Prompt = "Rate {{input}}",
            OutputDefinition = new EvaluatorOutputDefinition
            {
                DataType = EvaluatorOutputDataType.Numeric,
                Reasoning = new EvaluatorOutputFieldDefinition { Description = "why" },
                Score = new EvaluatorOutputScoreDefinition { Description = "0..1" }
            }
        });

        llmJson.ShouldContain("\"type\":\"llm_as_judge\"");
        llmJson.ShouldContain("\"prompt\":\"Rate {{input}}\"");

        var codeJson = JsonSerializer.Serialize(new CreateCodeEvaluatorRequest
        {
            Name = "length-check",
            SourceCode = "export default () => 1",
            SourceCodeLanguage = CodeEvaluatorSourceCodeLanguage.Typescript
        });

        codeJson.ShouldContain("\"type\":\"code\"");
        codeJson.ShouldContain("\"sourceCodeLanguage\":\"TYPESCRIPT\"");
    }

    [Fact]
    public void Should_Deserialize_LlmAsJudgeEvaluator_When_Type_Is_Not_First_Property()
    {
        // "type" is intentionally the last property: the converter must not rely on discriminator order
        var json = @"{
            ""id"": ""ev-1"",
            ""name"": ""tone"",
            ""version"": 2,
            ""scope"": ""managed"",
            ""prompt"": ""Classify {{input}}"",
            ""variables"": [""input""],
            ""outputDefinition"": {
                ""dataType"": ""CATEGORICAL"",
                ""reasoning"": { ""description"": ""why"" },
                ""score"": {
                    ""description"": ""label"",
                    ""categories"": [""positive"", ""negative""],
                    ""shouldAllowMultipleMatches"": false
                }
            },
            ""modelConfig"": null,
            ""evaluationRuleCount"": 3,
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-02T00:00:00Z"",
            ""type"": ""llm_as_judge""
        }";

        var evaluator = JsonSerializer.Deserialize<Evaluator>(json);

        var llmEvaluator = evaluator.ShouldBeOfType<LlmAsJudgeEvaluator>();
        llmEvaluator.Version.ShouldBe(2);
        llmEvaluator.Scope.ShouldBe(EvaluatorScope.Managed);
        llmEvaluator.Type.ShouldBe(EvaluatorType.Llm_As_Judge);
        llmEvaluator.Prompt.ShouldBe("Classify {{input}}");
        llmEvaluator.ModelConfig.ShouldBeNull();
        llmEvaluator.OutputDefinition.DataType.ShouldBe(EvaluatorOutputDataType.Categorical);
        llmEvaluator.OutputDefinition.Score.Categories.ShouldBe(new[] { "positive", "negative" });
        llmEvaluator.OutputDefinition.Score.ShouldAllowMultipleMatches.ShouldBe(false);
        llmEvaluator.EvaluationRuleCount.ShouldBe(3);
    }

    [Fact]
    public void Should_Deserialize_CodeEvaluator_When_Type_Is_Not_First_Property()
    {
        var json = @"{
            ""id"": ""ev-2"",
            ""name"": ""length-check"",
            ""version"": 1,
            ""scope"": ""project"",
            ""variables"": [""input"", ""output""],
            ""sourceCode"": ""def evaluate(**kwargs): return 1"",
            ""sourceCodeLanguage"": ""PYTHON"",
            ""evaluationRuleCount"": 0,
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z"",
            ""type"": ""code""
        }";

        var evaluator = JsonSerializer.Deserialize<Evaluator>(json);

        var codeEvaluator = evaluator.ShouldBeOfType<CodeEvaluator>();
        codeEvaluator.Type.ShouldBe(EvaluatorType.Code);
        codeEvaluator.SourceCode.ShouldBe("def evaluate(**kwargs): return 1");
        codeEvaluator.SourceCodeLanguage.ShouldBe(CodeEvaluatorSourceCodeLanguage.Python);
        codeEvaluator.Variables.ShouldBe(new[] { "input", "output" });
    }

    [Fact]
    public void Should_Throw_When_Evaluator_Type_Is_Missing_Or_Unknown()
    {
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Evaluator>(@"{ ""id"": ""ev-1"", ""name"": ""x"" }"));

        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<Evaluator>(@"{ ""id"": ""ev-1"", ""type"": ""human"" }"));
    }

    [Fact]
    public void Should_Serialize_Evaluator_Declared_As_Base_Type_With_Derived_Properties()
    {
        Evaluator evaluator = new CodeEvaluator
        {
            Id = "ev-2",
            Name = "length-check",
            Version = 1,
            Scope = EvaluatorScope.Project,
            Variables = new[] { "input" },
            SourceCode = "def evaluate(**kwargs): return 1",
            SourceCodeLanguage = CodeEvaluatorSourceCodeLanguage.Python,
            EvaluationRuleCount = 0,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var json = JsonSerializer.Serialize(evaluator);

        json.ShouldContain("\"type\":\"code\"");
        json.ShouldContain("\"sourceCode\":\"def evaluate(**kwargs): return 1\"");

        var roundTripped = JsonSerializer.Deserialize<Evaluator>(json);
        roundTripped.ShouldBeOfType<CodeEvaluator>();
    }

    [Fact]
    public void Should_RoundTrip_EvaluationRuleEvaluator_Type()
    {
        var evaluator = new EvaluationRuleEvaluator
        {
            Id = "ev-1",
            Name = "helpfulness",
            Scope = EvaluatorScope.Project,
            Type = EvaluatorType.Code
        };

        var json = JsonSerializer.Serialize(evaluator);
        json.ShouldContain("\"type\":\"code\"");

        var roundTripped = JsonSerializer.Deserialize<EvaluationRuleEvaluator>(json);
        roundTripped.ShouldNotBeNull();
        roundTripped.Type.ShouldBe(EvaluatorType.Code);
    }
}