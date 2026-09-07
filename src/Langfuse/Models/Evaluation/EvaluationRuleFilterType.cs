using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Discriminator for the <see cref="EvaluationRuleFilter" /> write union. Determines the required fields
///     and the shape of the filter's value.
/// </summary>
[JsonConverter(typeof(EnumWireValueConverter<EvaluationRuleFilterType>))]
public enum EvaluationRuleFilterType
{
    /// <summary>ISO date-time comparison. See <see cref="DateTimeEvaluationRuleFilter" />.</summary>
    [EnumMember(Value = "datetime")] DateTime,

    /// <summary>String comparison. See <see cref="StringEvaluationRuleFilter" />.</summary>
    [EnumMember(Value = "string")] String,

    /// <summary>Numeric comparison. See <see cref="NumberEvaluationRuleFilter" />.</summary>
    [EnumMember(Value = "number")] Number,

    /// <summary>Membership against a fixed set of string options. See <see cref="StringOptionsEvaluationRuleFilter" />.</summary>
    [EnumMember(Value = "stringOptions")] StringOptions,

    /// <summary>
    ///     Membership against a fixed set of string options within a keyed category. See
    ///     <see cref="CategoryOptionsEvaluationRuleFilter" />.
    /// </summary>
    [EnumMember(Value = "categoryOptions")]
    CategoryOptions,

    /// <summary>Membership comparison against an array-valued column. See <see cref="ArrayOptionsEvaluationRuleFilter" />.</summary>
    [EnumMember(Value = "arrayOptions")] ArrayOptions,

    /// <summary>
    ///     String comparison against a keyed entry inside an object-valued column. See
    ///     <see cref="StringObjectEvaluationRuleFilter" />.
    /// </summary>
    [EnumMember(Value = "stringObject")] StringObject,

    /// <summary>
    ///     Numeric comparison against a keyed entry inside an object-valued column. See
    ///     <see cref="NumberObjectEvaluationRuleFilter" />.
    /// </summary>
    [EnumMember(Value = "numberObject")] NumberObject,

    /// <summary>Boolean comparison. See <see cref="BooleanEvaluationRuleFilter" />.</summary>
    [EnumMember(Value = "boolean")] Boolean,

    /// <summary>Null/not-null check. See <see cref="NullEvaluationRuleFilter" />.</summary>
    [EnumMember(Value = "null")] Null
}