using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Comparison operators for boolean-valued evaluation rule filters (<see cref="BooleanEvaluationRuleFilter" />).
/// </summary>
[JsonConverter(typeof(EnumWireValueConverter<EvaluationRuleBooleanFilterOperator>))]
public enum EvaluationRuleBooleanFilterOperator
{
    /// <summary>Equal comparison.</summary>
    [EnumMember(Value = "=")] Equals,

    /// <summary>Not-equal comparison.</summary>
    [EnumMember(Value = "<>")] NotEquals
}