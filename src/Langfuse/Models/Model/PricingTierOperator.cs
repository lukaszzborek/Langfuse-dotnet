using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Comparison operators for pricing tier conditions.
///     Used to compare usage metrics against threshold values when determining which pricing tier applies.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<PricingTierOperator>))]
public enum PricingTierOperator
{
    /// <summary>
    ///     Greater than comparison. Matches when the sum of matched usage values exceeds the threshold.
    /// </summary>
    Gt,

    /// <summary>
    ///     Greater than or equal comparison. Matches when the sum of matched usage values is at least the threshold.
    /// </summary>
    Gte,

    /// <summary>
    ///     Less than comparison. Matches when the sum of matched usage values is below the threshold.
    /// </summary>
    Lt,

    /// <summary>
    ///     Less than or equal comparison. Matches when the sum of matched usage values is at most the threshold.
    /// </summary>
    Lte,

    /// <summary>
    ///     Equal comparison. Matches when the sum of matched usage values equals the threshold exactly.
    /// </summary>
    Eq,

    /// <summary>
    ///     Not equal comparison. Matches when the sum of matched usage values differs from the threshold.
    /// </summary>
    Neq
}
