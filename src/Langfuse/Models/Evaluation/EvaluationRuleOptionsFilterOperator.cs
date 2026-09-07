using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Membership operators for fixed-option evaluation rule filters
///     (<see cref="StringOptionsEvaluationRuleFilter" />, <see cref="CategoryOptionsEvaluationRuleFilter" />).
/// </summary>
[JsonConverter(typeof(EnumWireValueConverter<EvaluationRuleOptionsFilterOperator>))]
public enum EvaluationRuleOptionsFilterOperator
{
    /// <summary>Matches when the column value is any of the given options.</summary>
    [EnumMember(Value = "any of")] AnyOf,

    /// <summary>Matches when the column value is none of the given options.</summary>
    [EnumMember(Value = "none of")] NoneOf
}