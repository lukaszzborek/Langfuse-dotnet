using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Null-check operators for null-valued evaluation rule filters (<see cref="NullEvaluationRuleFilter" />).
/// </summary>
[JsonConverter(typeof(EnumWireValueConverter<EvaluationRuleNullFilterOperator>))]
public enum EvaluationRuleNullFilterOperator
{
    /// <summary>Matches when the column is null.</summary>
    [EnumMember(Value = "is null")] IsNull,

    /// <summary>Matches when the column is not null.</summary>
    [EnumMember(Value = "is not null")] IsNotNull
}