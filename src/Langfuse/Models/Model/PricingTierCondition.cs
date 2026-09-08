using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Condition for matching a pricing tier against usage details or observation attributes.
///     All conditions in a tier must be met (AND logic) for the tier to match.
/// </summary>
/// <remarks>
///     <para>Two kinds of conditions exist:</para>
///     <list type="bullet">
///         <item>
///             <see cref="PricingTierUsageCondition" /> treats <c>usageDetailPattern</c> as a regex, sums all matching
///             usage values and compares the sum to a numeric threshold.
///         </item>
///         <item>
///             <see cref="PricingTierAttributeCondition" /> matches an exact top-level model parameter or metadata key
///             against one or more string values.
///         </item>
///     </list>
/// </remarks>
[JsonConverter(typeof(PricingTierConditionConverter))]
public abstract class PricingTierCondition
{
}

/// <summary>
///     Condition that sums usage details whose keys match a regex and compares the sum to a numeric threshold.
/// </summary>
/// <remarks>
///     <para>How it works:</para>
///     <list type="number">
///         <item>The regex pattern matches against usage detail keys (e.g., "input_tokens", "input_cached")</item>
///         <item>Values of all matching keys are summed together</item>
///         <item>The sum is compared against the threshold value using the specified operator</item>
///     </list>
/// </remarks>
public class PricingTierUsageCondition : PricingTierCondition
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
    ///     Optional on input, where it defaults to false (case-insensitive matching) when omitted.
    ///     Always present on conditions read back from the API, so responses never leave this null.
    /// </summary>
    [JsonPropertyName("caseSensitive")]
    public bool? CaseSensitive { get; set; }
}

/// <summary>
///     Condition that matches any configured value for a top-level observation attribute
///     (model parameter or metadata), e.g. a provider service tier.
/// </summary>
public class PricingTierAttributeCondition : PricingTierCondition
{
    /// <summary>
    ///     Observation attribute object evaluated by this condition.
    /// </summary>
    [JsonPropertyName("source")]
    public PricingTierAttributeSource Source { get; set; }

    /// <summary>
    ///     Exact top-level attribute key.
    /// </summary>
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    ///     Membership operator. Always <c>in</c>.
    /// </summary>
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = "in";

    /// <summary>
    ///     Accepted string attribute values. At least one value is required.
    /// </summary>
    [JsonPropertyName("values")]
    public List<string> Values { get; set; } = [];
}

/// <summary>
///     Observation attribute object evaluated by a <see cref="PricingTierAttributeCondition" />.
/// </summary>
[JsonConverter(typeof(SnakeCaseLowerEnumConverter<PricingTierAttributeSource>))]
public enum PricingTierAttributeSource
{
    /// <summary>
    ///     Top-level keys of the observation's model parameters.
    /// </summary>
    ModelParameters,

    /// <summary>
    ///     Top-level keys of the observation's metadata.
    /// </summary>
    Metadata
}