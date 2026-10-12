using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Comparison operators for <see cref="StringObjectEvaluationRuleFilter" />.
/// </summary>
[JsonConverter(typeof(EnumWireValueConverter<EvaluationRuleStringObjectFilterOperator>))]
public enum EvaluationRuleStringObjectFilterOperator
{
    /// <summary>Exact match.</summary>
    [EnumMember(Value = "=")] Equals,

    /// <summary>Substring match.</summary>
    [EnumMember(Value = "contains")] Contains,

    /// <summary>Negated substring match.</summary>
    [EnumMember(Value = "does not contain")]
    DoesNotContain,

    /// <summary>Prefix match.</summary>
    [EnumMember(Value = "starts with")] StartsWith,

    /// <summary>Suffix match.</summary>
    [EnumMember(Value = "ends with")] EndsWith,

    /// <summary>Key is present.</summary>
    [EnumMember(Value = "is set")] IsSet,

    /// <summary>Key is absent.</summary>
    [EnumMember(Value = "is not set")] IsNotSet
}
