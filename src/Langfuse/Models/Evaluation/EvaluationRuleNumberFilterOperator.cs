using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Comparison operators for numeric and date-time evaluation rule filters
///     (<see cref="NumberEvaluationRuleFilter" />, <see cref="NumberObjectEvaluationRuleFilter" />,
///     <see cref="DateTimeEvaluationRuleFilter" />).
/// </summary>
[JsonConverter(typeof(EnumWireValueConverter<EvaluationRuleNumberFilterOperator>))]
public enum EvaluationRuleNumberFilterOperator
{
    /// <summary>Equal comparison.</summary>
    [EnumMember(Value = "=")] Equals,

    /// <summary>Greater-than comparison.</summary>
    [EnumMember(Value = ">")] GreaterThan,

    /// <summary>Less-than comparison.</summary>
    [EnumMember(Value = "<")] LessThan,

    /// <summary>Greater-than-or-equal comparison.</summary>
    [EnumMember(Value = ">=")] GreaterThanOrEqual,

    /// <summary>Less-than-or-equal comparison.</summary>
    [EnumMember(Value = "<=")] LessThanOrEqual
}