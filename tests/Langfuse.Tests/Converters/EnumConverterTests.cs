using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.Model;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Tests.Converters;

public class EnumConverterTests
{
    #region PricingTierOperator Tests

    [Theory]
    [InlineData(PricingTierOperator.Gt, "gt")]
    [InlineData(PricingTierOperator.Gte, "gte")]
    [InlineData(PricingTierOperator.Lt, "lt")]
    [InlineData(PricingTierOperator.Lte, "lte")]
    [InlineData(PricingTierOperator.Eq, "eq")]
    [InlineData(PricingTierOperator.Neq, "neq")]
    public void PricingTierOperator_Should_Serialize_To_Lowercase(PricingTierOperator value, string expected)
    {
        var testObject = new { Operator = value };
        var json = JsonSerializer.Serialize(testObject);
        json.ShouldContain($"\"{expected}\"");
    }

    [Theory]
    [InlineData("gt", PricingTierOperator.Gt)]
    [InlineData("gte", PricingTierOperator.Gte)]
    [InlineData("lt", PricingTierOperator.Lt)]
    [InlineData("lte", PricingTierOperator.Lte)]
    [InlineData("eq", PricingTierOperator.Eq)]
    [InlineData("neq", PricingTierOperator.Neq)]
    [InlineData("GT", PricingTierOperator.Gt)]
    [InlineData("GTE", PricingTierOperator.Gte)]
    [InlineData("Gt", PricingTierOperator.Gt)]
    public void PricingTierOperator_Should_Deserialize_From_Various_Cases(string jsonValue,
        PricingTierOperator expected)
    {
        var json = $"{{\"Operator\":\"{jsonValue}\"}}";
        var result = JsonSerializer.Deserialize<TestPricingTierOperator>(json);
        result.ShouldNotBeNull();
        result.Operator.ShouldBe(expected);
    }

    #endregion

    #region ScoreConfigDataType Tests

    [Theory]
    [InlineData(ScoreConfigDataType.Numeric, "NUMERIC")]
    [InlineData(ScoreConfigDataType.Boolean, "BOOLEAN")]
    [InlineData(ScoreConfigDataType.Categorical, "CATEGORICAL")]
    public void ScoreConfigDataType_Should_Serialize_To_Uppercase(ScoreConfigDataType value, string expected)
    {
        var testObject = new { DataType = value };
        var json = JsonSerializer.Serialize(testObject);
        json.ShouldContain($"\"{expected}\"");
    }

    [Theory]
    [InlineData("NUMERIC", ScoreConfigDataType.Numeric)]
    [InlineData("BOOLEAN", ScoreConfigDataType.Boolean)]
    [InlineData("CATEGORICAL", ScoreConfigDataType.Categorical)]
    [InlineData("numeric", ScoreConfigDataType.Numeric)]
    [InlineData("Numeric", ScoreConfigDataType.Numeric)]
    public void ScoreConfigDataType_Should_Deserialize_From_Various_Cases(string jsonValue,
        ScoreConfigDataType expected)
    {
        var json = $"{{\"DataType\":\"{jsonValue}\"}}";
        var result = JsonSerializer.Deserialize<TestScoreConfigDataType>(json);
        result.ShouldNotBeNull();
        result.DataType.ShouldBe(expected);
    }

    #endregion

    #region ScoreDataType Tests (including new Correction value)

    [Theory]
    [InlineData(ScoreDataType.Numeric, "NUMERIC")]
    [InlineData(ScoreDataType.Boolean, "BOOLEAN")]
    [InlineData(ScoreDataType.Categorical, "CATEGORICAL")]
    [InlineData(ScoreDataType.Correction, "CORRECTION")]
    public void ScoreDataType_Should_Serialize_To_Uppercase(ScoreDataType value, string expected)
    {
        var testObject = new { DataType = value };
        var json = JsonSerializer.Serialize(testObject);
        json.ShouldContain($"\"{expected}\"");
    }

    [Theory]
    [InlineData("CORRECTION", ScoreDataType.Correction)]
    [InlineData("correction", ScoreDataType.Correction)]
    [InlineData("Correction", ScoreDataType.Correction)]
    public void ScoreDataType_Correction_Should_Deserialize_From_Various_Cases(string jsonValue,
        ScoreDataType expected)
    {
        var json = $"{{\"DataType\":\"{jsonValue}\"}}";
        var result = JsonSerializer.Deserialize<TestScoreDataType>(json);
        result.ShouldNotBeNull();
        result.DataType.ShouldBe(expected);
    }

    #endregion

    #region Test Classes

    private class TestPricingTierOperator
    {
        public PricingTierOperator Operator { get; set; }
    }

    private class TestScoreConfigDataType
    {
        public ScoreConfigDataType DataType { get; set; }
    }

    private class TestScoreDataType
    {
        public ScoreDataType DataType { get; set; }
    }

    #endregion
}
