using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Input schema for creating a pricing tier.
///     The tier ID will be automatically generated server-side.
/// </summary>
/// <remarks>
///     <para>When creating a model with pricing tiers:</para>
///     <list type="bullet">
///         <item>Exactly one tier must have IsDefault=true (the fallback tier)</item>
///         <item>The default tier must have Priority=0 and empty Conditions</item>
///         <item>All tier names and priorities must be unique within the model</item>
///         <item>Each tier must define at least one price</item>
///     </list>
/// </remarks>
public class PricingTierInput
{
    /// <summary>
    ///     Name of the pricing tier for display and identification purposes.
    ///     Must be unique within the model.
    /// </summary>
    /// <example>
    ///     "Standard", "High Volume Tier", "Extended Context"
    /// </example>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Whether this is the default tier.
    ///     Exactly one tier per model must be marked as default.
    /// </summary>
    /// <remarks>
    ///     Requirements for default tier:
    ///     <list type="bullet">
    ///         <item>Must have IsDefault=true</item>
    ///         <item>Must have Priority=0</item>
    ///         <item>Must have empty Conditions array</item>
    ///     </list>
    ///     The default tier acts as a fallback when no conditional tiers match.
    /// </remarks>
    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }

    /// <summary>
    ///     Priority for tier matching evaluation.
    ///     Lower numbers = higher priority (evaluated first).
    ///     Must be unique within the model. The default tier must have Priority=0.
    ///     Conditional tiers should use priority 1, 2, 3, etc. based on their specificity.
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>
    ///     Array of conditions that must ALL be met for this tier to match (AND logic).
    ///     The default tier must have an empty array.
    ///     Conditional tiers should define one or more conditions that specify when this tier's pricing applies.
    /// </summary>
    [JsonPropertyName("conditions")]
    public List<PricingTierCondition> Conditions { get; set; } = [];

    /// <summary>
    ///     Prices (USD) by usage type for this tier.
    ///     At least one price must be defined.
    ///     Common usage types: "input", "output", "total", "request", "image".
    ///     Prices are in USD per unit (e.g., per token).
    /// </summary>
    /// <example>
    ///     {"input": 0.000003, "output": 0.000015} represents $3 per million input tokens and $15 per million output tokens.
    /// </example>
    [JsonPropertyName("prices")]
    public Dictionary<string, double> Prices { get; set; } = new();
}
