using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Tests.Models;

public class EvaluatorOutputDefinitionTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    [Fact]
    public void Should_RoundTrip_Numeric()
    {
        var definition = new NumericEvaluatorOutputDefinition
        {
            ScoreReasoningInstructions = "Explain",
            ScoreValueInstructions = "Score it",
            MinValue = 0,
            MaxValue = 1
        };

        var json = JsonSerializer.Serialize<EvaluatorOutputDefinition>(definition, Options);

        json.ShouldBe(
            "{\"dataType\":\"NUMERIC\",\"minValue\":0,\"maxValue\":1,\"scoreReasoningInstructions\":\"Explain\",\"scoreValueInstructions\":\"Score it\"}");

        var deserialized = JsonSerializer.Deserialize<EvaluatorOutputDefinition>(json);

        var numeric = deserialized.ShouldBeOfType<NumericEvaluatorOutputDefinition>();
        numeric.DataType.ShouldBe(EvaluatorOutputScoreType.Numeric);
        numeric.ScoreReasoningInstructions.ShouldBe("Explain");
        numeric.ScoreValueInstructions.ShouldBe("Score it");
        numeric.MinValue.ShouldBe(0);
        numeric.MaxValue.ShouldBe(1);
    }

    [Fact]
    public void Should_RoundTrip_Numeric_With_Optionals_Omitted()
    {
        var definition = new NumericEvaluatorOutputDefinition();

        var json = JsonSerializer.Serialize<EvaluatorOutputDefinition>(definition, Options);

        json.ShouldBe("{\"dataType\":\"NUMERIC\"}");

        var deserialized = JsonSerializer.Deserialize<EvaluatorOutputDefinition>(json);
        deserialized.ShouldBeOfType<NumericEvaluatorOutputDefinition>().MinValue.ShouldBeNull();
    }

    [Fact]
    public void Should_RoundTrip_Boolean()
    {
        var definition = new BooleanEvaluatorOutputDefinition
        {
            ScoreReasoningInstructions = "Explain"
        };

        var json = JsonSerializer.Serialize<EvaluatorOutputDefinition>(definition, Options);

        json.ShouldBe("{\"dataType\":\"BOOLEAN\",\"scoreReasoningInstructions\":\"Explain\"}");

        var deserialized = JsonSerializer.Deserialize<EvaluatorOutputDefinition>(json);

        var boolean = deserialized.ShouldBeOfType<BooleanEvaluatorOutputDefinition>();
        boolean.DataType.ShouldBe(EvaluatorOutputScoreType.Boolean);
        boolean.ScoreReasoningInstructions.ShouldBe("Explain");
    }

    [Fact]
    public void Should_RoundTrip_Categorical()
    {
        var definition = new CategoricalEvaluatorOutputDefinition
        {
            Categories = new[] { "good", "bad" },
            ShouldAllowMultipleMatches = false
        };

        var json = JsonSerializer.Serialize<EvaluatorOutputDefinition>(definition, Options);

        json.ShouldBe(
            "{\"dataType\":\"CATEGORICAL\",\"categories\":[\"good\",\"bad\"],\"shouldAllowMultipleMatches\":false}");

        var deserialized = JsonSerializer.Deserialize<EvaluatorOutputDefinition>(json);

        var categorical = deserialized.ShouldBeOfType<CategoricalEvaluatorOutputDefinition>();
        categorical.DataType.ShouldBe(EvaluatorOutputScoreType.Categorical);
        categorical.Categories.ShouldBe(new[] { "good", "bad" });
        categorical.ShouldAllowMultipleMatches.ShouldBeFalse();
    }

    [Fact]
    public void Should_Throw_When_DataType_Missing_Or_Unknown()
    {
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<EvaluatorOutputDefinition>("{}"));

        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<EvaluatorOutputDefinition>("{\"dataType\":\"UNKNOWN\"}"));
    }
}