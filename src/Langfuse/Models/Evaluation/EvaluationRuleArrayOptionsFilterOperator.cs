using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Membership operators for array-valued evaluation rule filters (<see cref="ArrayOptionsEvaluationRuleFilter" />).
/// </summary>
[JsonConverter(typeof(EnumWireValueConverter<EvaluationRuleArrayOptionsFilterOperator>))]
public enum EvaluationRuleArrayOptionsFilterOperator
{
    /// <summary>Matches when the array column contains any of the given values.</summary>
    [EnumMember(Value = "any of")] AnyOf,

    /// <summary>Matches when the array column contains none of the given values.</summary>
    [EnumMember(Value = "none of")] NoneOf,

    /// <summary>Matches when the array column contains all of the given values.</summary>
    [EnumMember(Value = "all of")] AllOf
}