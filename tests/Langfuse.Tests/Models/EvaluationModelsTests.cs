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
    public void Should_Serialize_EvaluatorType_To_Lowercase_With_Underscores()
    {
        JsonSerializer.Serialize(EvaluatorType.Llm_As_Judge).ShouldBe("\"llm_as_judge\"");
        JsonSerializer.Deserialize<EvaluatorType>("\"llm_as_judge\"").ShouldBe(EvaluatorType.Llm_As_Judge);
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
    public void Should_Serialize_CreateEvaluationRuleRequest_And_Omit_Optionals()
    {
        var request = new CreateEvaluationRuleRequest
        {
            Name = "rule",
            Evaluator = new EvaluationRuleEvaluatorReference { Name = "helpfulness", Scope = EvaluatorScope.Managed },
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
        json.ShouldContain("\"target\":\"experiment\"");
        json.ShouldContain("\"source\":\"expected_output\"");
        // sampling and filter are optional and omitted when null
        json.ShouldNotContain("sampling");
        json.ShouldNotContain("filter");
    }

    [Fact]
    public void Should_Deserialize_Evaluator_With_Categorical_Output()
    {
        var json = @"{
            ""id"": ""ev-1"",
            ""name"": ""tone"",
            ""version"": 2,
            ""scope"": ""managed"",
            ""type"": ""llm_as_judge"",
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
            ""updatedAt"": ""2024-01-02T00:00:00Z""
        }";

        var evaluator = JsonSerializer.Deserialize<Evaluator>(json);

        evaluator.ShouldNotBeNull();
        evaluator.Version.ShouldBe(2);
        evaluator.Scope.ShouldBe(EvaluatorScope.Managed);
        evaluator.Type.ShouldBe(EvaluatorType.Llm_As_Judge);
        evaluator.ModelConfig.ShouldBeNull();
        evaluator.OutputDefinition.DataType.ShouldBe(EvaluatorOutputDataType.Categorical);
        evaluator.OutputDefinition.Score.Categories.ShouldBe(new[] { "positive", "negative" });
        evaluator.OutputDefinition.Score.ShouldAllowMultipleMatches.ShouldBe(false);
        evaluator.EvaluationRuleCount.ShouldBe(3);
    }
}
