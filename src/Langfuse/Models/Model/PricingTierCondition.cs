using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Condition for matching a pricing tier based on usage details.
///     Used to implement tiered pricing models where costs vary based on usage thresholds.
/// </summary>
/// <remarks>
///     <para>How it works:</para>
///     <list type="number">
///         <item>The regex pattern matches against usage detail keys (e.g., "input_tokens", "input_cached")</item>
///         <item>Values of all matching keys are summed together</item>
///         <item>The sum is compared against the threshold value using the specified operator</item>
///         <item>All conditions in a tier must be met (AND logic) for the tier to match</item>
///     </list>
/// </remarks>
public class PricingTierCondition
{
    /// <summary>
    ///     Regex pattern to match against usage detail keys.
    ///     Values from all matching keys are summed for comparison.
    ///     The pattern is case-insensitive by default.
    /// </summary>
    /// <example>
    ///     "^input" matches "input", "input_tokens", "input_cached", etc.
    /// </example>
    [JsonPropertyName("usageDetailPattern")]
    public string UsageDetailPattern { get; set; } = string.Empty;

    /// <summary>
    ///     Comparison operator for evaluating the condition.
    /// </summary>
    [JsonPropertyName("operator")]
    public PricingTierOperator Operator { get; set; }

    /// <summary>
    ///     Threshold value to compare against the sum of matched usage values.
    /// </summary>
    [JsonPropertyName("value")]
    public double Value { get; set; }

    /// <summary>
    ///     Whether the regex pattern matching is case-sensitive.
    ///     Default is false (case-insensitive matching).
    /// </summary>
    [JsonPropertyName("caseSensitive")]
    public bool CaseSensitive { get; set; }
}
