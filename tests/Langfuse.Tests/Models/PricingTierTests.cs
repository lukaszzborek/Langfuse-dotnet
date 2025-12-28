using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using zborek.Langfuse.Models.Model;

namespace zborek.Langfuse.Tests.Models;

public class PricingTierTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    #region PricingTierCondition Tests

    [Fact]
    public void PricingTierCondition_Should_Serialize_Correctly()
    {
        // Arrange
        var condition = new PricingTierCondition
        {
            UsageDetailPattern = "^input",
            Operator = PricingTierOperator.Gte,
            Value = 128000,
            CaseSensitive = false
        };

        // Act
        var json = JsonSerializer.Serialize(condition, JsonOptions);

        // Assert
        json.ShouldContain("\"usageDetailPattern\":\"^input\"");
        json.ShouldContain("\"operator\":\"gte\"");
        json.ShouldContain("\"value\":128000");
        json.ShouldContain("\"caseSensitive\":false");
    }

    [Fact]
    public void PricingTierCondition_Should_Deserialize_Correctly()
    {
        // Arrange
        var json = """
                   {
                     "usageDetailPattern": "^output",
                     "operator": "gt",
                     "value": 50000,
                     "caseSensitive": true
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<PricingTierCondition>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.UsageDetailPattern.ShouldBe("^output");
        result.Operator.ShouldBe(PricingTierOperator.Gt);
        result.Value.ShouldBe(50000);
        result.CaseSensitive.ShouldBeTrue();
    }

    #endregion

    #region PricingTier Tests

    [Fact]
    public void PricingTier_DefaultTier_Should_Serialize_Correctly()
    {
        // Arrange
        var tier = new PricingTier
        {
            Id = "tier-default-123",
            Name = "Standard",
            IsDefault = true,
            Priority = 0,
            Conditions = [],
            Prices = new Dictionary<string, double>
            {
                ["input"] = 0.000003,
                ["output"] = 0.000015
            }
        };

        // Act
        var json = JsonSerializer.Serialize(tier, JsonOptions);

        // Assert
        json.ShouldContain("\"id\":\"tier-default-123\"");
        json.ShouldContain("\"name\":\"Standard\"");
        json.ShouldContain("\"isDefault\":true");
        json.ShouldContain("\"priority\":0");
        json.ShouldContain("\"conditions\":[]");
        // Decimal values may serialize with or without trailing zeros
        json.ShouldContain("\"input\":");
        json.ShouldContain("\"output\":");
    }

    [Fact]
    public void PricingTier_ConditionalTier_Should_Serialize_Correctly()
    {
        // Arrange
        var tier = new PricingTier
        {
            Id = "tier-extended-456",
            Name = "Extended Context",
            IsDefault = false,
            Priority = 1,
            Conditions =
            [
                new PricingTierCondition
                {
                    UsageDetailPattern = "^input",
                    Operator = PricingTierOperator.Gte,
                    Value = 128000,
                    CaseSensitive = false
                }
            ],
            Prices = new Dictionary<string, double>
            {
                ["input"] = 0.00001,
                ["output"] = 0.00003
            }
        };

        // Act
        var json = JsonSerializer.Serialize(tier, JsonOptions);

        // Assert
        json.ShouldContain("\"isDefault\":false");
        json.ShouldContain("\"priority\":1");
        json.ShouldContain("\"usageDetailPattern\":\"^input\"");
        json.ShouldContain("\"operator\":\"gte\"");
        json.ShouldContain("\"value\":128000");
        json.ShouldContain("\"caseSensitive\":false");
    }

    [Fact]
    public void PricingTier_Should_Deserialize_From_Api_Response()
    {
        // Arrange
        var json = """
                   {
                     "id": "tier-xyz-789",
                     "name": "High Volume",
                     "isDefault": false,
                     "priority": 2,
                     "conditions": [
                       {
                         "usageDetailPattern": "^input_tokens$",
                         "operator": "gt",
                         "value": 200000,
                         "caseSensitive": false
                       },
                       {
                         "usageDetailPattern": "^output_tokens$",
                         "operator": "gt",
                         "value": 50000,
                         "caseSensitive": false
                       }
                     ],
                     "prices": {
                       "input": 0.000005,
                       "output": 0.00002,
                       "total": 0.00001
                     }
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<PricingTier>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("tier-xyz-789");
        result.Name.ShouldBe("High Volume");
        result.IsDefault.ShouldBeFalse();
        result.Priority.ShouldBe(2);
        result.Conditions.Count.ShouldBe(2);
        result.Conditions[0].UsageDetailPattern.ShouldBe("^input_tokens$");
        result.Conditions[0].Operator.ShouldBe(PricingTierOperator.Gt);
        result.Conditions[0].CaseSensitive.ShouldBeFalse();
        result.Conditions[1].UsageDetailPattern.ShouldBe("^output_tokens$");
        result.Prices.Count.ShouldBe(3);
        result.Prices["input"].ShouldBe(0.000005);
        result.Prices["output"].ShouldBe(0.00002);
        result.Prices["total"].ShouldBe(0.00001);
    }

    #endregion

    #region PricingTierInput Tests

    [Fact]
    public void PricingTierInput_Should_Serialize_Correctly()
    {
        // Arrange
        var input = new PricingTierInput
        {
            Name = "New Tier",
            IsDefault = false,
            Priority = 1,
            Conditions =
            [
                new PricingTierCondition
                {
                    UsageDetailPattern = "^input",
                    Operator = PricingTierOperator.Gte,
                    Value = 100000,
                    CaseSensitive = false
                }
            ],
            Prices = new Dictionary<string, double>
            {
                ["input"] = 0.000008,
                ["output"] = 0.000024
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input, JsonOptions);

        // Assert
        json.ShouldContain("\"name\":\"New Tier\"");
        json.ShouldContain("\"isDefault\":false");
        json.ShouldContain("\"priority\":1");
        json.ShouldNotContain("\"id\""); // ID should not be in input
    }

    [Fact]
    public void PricingTierInput_DefaultTier_Should_Have_Empty_Conditions()
    {
        // Arrange
        var input = new PricingTierInput
        {
            Name = "Default",
            IsDefault = true,
            Priority = 0,
            Conditions = [],
            Prices = new Dictionary<string, double>
            {
                ["input"] = 0.000003,
                ["output"] = 0.000015
            }
        };

        // Act
        var json = JsonSerializer.Serialize(input, JsonOptions);

        // Assert
        json.ShouldContain("\"conditions\":[]");
        json.ShouldContain("\"isDefault\":true");
        json.ShouldContain("\"priority\":0");
    }

    #endregion

    #region Model with PricingTiers Tests

    [Fact]
    public void Model_Should_Deserialize_With_PricingTiers()
    {
        // Arrange
        var json = """
                   {
                     "id": "model-123",
                     "modelName": "gpt-4-turbo",
                     "matchPattern": "gpt-4-turbo.*",
                     "unit": "TOKENS",
                     "isLangfuseManaged": true,
                     "pricingTiers": [
                       {
                         "id": "tier-1",
                         "name": "Standard",
                         "isDefault": true,
                         "priority": 0,
                         "conditions": [],
                         "prices": {
                           "input": 0.00001,
                           "output": 0.00003
                         }
                       },
                       {
                         "id": "tier-2",
                         "name": "Extended",
                         "isDefault": false,
                         "priority": 1,
                         "conditions": [
                           {
                             "usageDetailPattern": "^input",
                             "operator": "gte",
                             "value": 128000,
                             "caseSensitive": false
                           }
                         ],
                         "prices": {
                           "input": 0.00002,
                           "output": 0.00006
                         }
                       }
                     ]
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<Model>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.PricingTiers.ShouldNotBeNull();
        result.PricingTiers.Count.ShouldBe(2);

        var standardTier = result.PricingTiers.First(t => t.IsDefault);
        standardTier.Name.ShouldBe("Standard");
        standardTier.Priority.ShouldBe(0);
        standardTier.Conditions.ShouldBeEmpty();

        var extendedTier = result.PricingTiers.First(t => !t.IsDefault);
        extendedTier.Name.ShouldBe("Extended");
        extendedTier.Priority.ShouldBe(1);
        extendedTier.Conditions.Count.ShouldBe(1);
        extendedTier.Conditions[0].Operator.ShouldBe(PricingTierOperator.Gte);
    }

    #endregion
}
