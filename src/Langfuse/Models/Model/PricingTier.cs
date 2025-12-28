using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Pricing tier definition with conditional pricing based on usage thresholds.
/// </summary>
/// <remarks>
///     <para>
///         Pricing tiers enable accurate cost tracking for LLM providers that charge different rates
///         based on usage patterns. For example, some providers charge higher rates when context size
///         exceeds certain thresholds.
///     </para>
///     <para>How tier matching works:</para>
///     <list type="number">
///         <item>Tiers are evaluated in ascending priority order (priority 1 before priority 2, etc.)</item>
///         <item>For each tier, all conditions must be met (AND logic)</item>
///         <item>The first matching tier's prices are used for cost calculation</item>
///         <item>If no conditional tiers match, the default tier (isDefault=true) is used</item>
///     </list>
/// </remarks>
public class PricingTier
{
    /// <summary>
    ///     Unique identifier for the pricing tier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the pricing tier for display and identification purposes.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Whether this is the default tier.
    ///     Exactly one tier per model must be marked as default.
    ///     The default tier acts as a fallback when no conditional tiers match.
    /// </summary>
    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }

    /// <summary>
    ///     Priority for tier matching evaluation.
    ///     Lower numbers = higher priority (evaluated first).
    ///     The default tier must always have priority 0.
    ///     Conditional tiers should have priority 1, 2, 3, etc.
    /// </summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>
    ///     Array of conditions that must ALL be met for this tier to match (AND logic).
    ///     The default tier must have an empty conditions array.
    ///     Conditional tiers should have one or more conditions that define when this tier's pricing applies.
    /// </summary>
    [JsonPropertyName("conditions")]
    public List<PricingTierCondition> Conditions { get; set; } = [];

    /// <summary>
    ///     Prices (USD) by usage type for this tier.
    ///     Common usage types: "input", "output", "total", "request", "image".
    ///     Prices are specified in USD per unit (e.g., per token, per request, per second).
    /// </summary>
    /// <example>
    ///     {"input": 0.000003, "output": 0.000015} means $3 per million input tokens and $15 per million output tokens.
    /// </example>
    [JsonPropertyName("prices")]
    public Dictionary<string, double> Prices { get; set; } = new();
}
