using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     One filter condition used to decide whether a live-ingested observation should be evaluated, for
///     creating or updating an evaluation rule. All filters of a rule must be satisfied. Each column accepts
///     exactly one filter type. When rules are read back, filters come as the untyped
///     <see cref="EvaluationRuleReadFilter" />.
/// </summary>
/// <remarks>
///     <para>Supported columns and their filter type:</para>
///     <list type="bullet">
///         <item>
///             <c>type</c> (<see cref="StringOptionsEvaluationRuleFilter" />: SPAN, EVENT, GENERATION, AGENT, TOOL,
///             CHAIN, RETRIEVER, EVALUATOR, EMBEDDING, GUARDRAIL)
///         </item>
///         <item>
///             <c>name</c>, <c>environment</c>, <c>traceName</c>, <c>experimentId</c>, <c>datasetId</c>
///             (<see cref="StringOptionsEvaluationRuleFilter" />; dataset ids, not names)
///         </item>
///         <item><c>level</c> (<see cref="StringOptionsEvaluationRuleFilter" />: DEBUG, DEFAULT, WARNING, ERROR)</item>
///         <item><c>version</c>, <c>userId</c>, <c>sessionId</c> (<see cref="StringEvaluationRuleFilter" />)</item>
///         <item><c>tags</c>, <c>calledToolNames</c> (<see cref="ArrayOptionsEvaluationRuleFilter" />)</item>
///         <item><c>metadata</c> (<see cref="StringObjectEvaluationRuleFilter" />, requires <c>Key</c>)</item>
///         <item><c>isRootObservation</c>, <c>isExperimentItemRootSpan</c> (<see cref="BooleanEvaluationRuleFilter" />)</item>
///         <item><c>parentObservationId</c> (<see cref="NullEvaluationRuleFilter" />)</item>
///         <item><c>toolCalls</c> (<see cref="NumberEvaluationRuleFilter" />: number of tool calls on the observation)</item>
///     </list>
///     <para>
///         Experiment scope is expressed with filters: <c>isExperimentItemRootSpan = true</c> limits execution to
///         experiment item roots and <c>datasetId</c> limits it to experiments for the selected datasets.
///     </para>
/// </remarks>
[JsonConverter(typeof(EvaluationRuleFilterConverter))]
public abstract class EvaluationRuleFilter
{
    /// <summary>
    ///     Filter type discriminator. Determines the required fields and the shape of <c>Value</c>.
    ///     Set by the concrete filter class.
    /// </summary>
    [JsonPropertyName("type")]
    [JsonPropertyOrder(0)]
    public abstract EvaluationRuleFilterType Type { get; }

    /// <summary>
    ///     Column to filter on.
    /// </summary>
    [JsonPropertyName("column")]
    [JsonPropertyOrder(1)]
    public required string Column { get; init; }
}

/// <summary>
///     Filters an ISO date-time column using a numeric comparison operator.
/// </summary>
public class DateTimeEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.DateTime;

    /// <summary>
    ///     Comparison operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleNumberFilterOperator Operator { get; init; }

    /// <summary>
    ///     ISO 8601 date-time value to compare against.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

/// <summary>
///     Filters a string-valued column.
/// </summary>
public class StringEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.String;

    /// <summary>
    ///     Comparison operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleStringFilterOperator Operator { get; init; }

    /// <summary>
    ///     String value to compare against.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

/// <summary>
///     Filters a numeric column.
/// </summary>
public class NumberEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.Number;

    /// <summary>
    ///     Comparison operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleNumberFilterOperator Operator { get; init; }

    /// <summary>
    ///     Numeric value to compare against.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required double Value { get; init; }
}

/// <summary>
///     Filters a column against a fixed set of string options (e.g. observation <c>type</c>, <c>tags</c> presence
///     as a single value, <c>level</c>).
/// </summary>
public class StringOptionsEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.StringOptions;

    /// <summary>
    ///     Membership operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleOptionsFilterOperator Operator { get; init; }

    /// <summary>
    ///     Accepted string options.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required string[] Value { get; init; }
}

/// <summary>
///     Filters a keyed category inside an object-valued column against a fixed set of string options.
/// </summary>
public class CategoryOptionsEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.CategoryOptions;

    /// <summary>
    ///     Top-level key inside the object-valued column.
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("key")]
    public required string Key { get; init; }

    /// <summary>
    ///     Membership operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleOptionsFilterOperator Operator { get; init; }

    /// <summary>
    ///     Accepted string options.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required string[] Value { get; init; }
}

/// <summary>
///     Filters an array-valued column (e.g. <c>tags</c>, <c>calledToolNames</c>) against a set of string values.
/// </summary>
public class ArrayOptionsEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.ArrayOptions;

    /// <summary>
    ///     Membership operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleArrayOptionsFilterOperator Operator { get; init; }

    /// <summary>
    ///     String values to compare against the array column.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required string[] Value { get; init; }
}

/// <summary>
///     Filters a keyed entry inside an object-valued column (e.g. <c>metadata</c>) using a string comparison.
/// </summary>
public class StringObjectEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.StringObject;

    /// <summary>
    ///     Top-level key inside the object-valued column.
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("key")]
    public required string Key { get; init; }

    /// <summary>
    ///     Comparison operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleStringFilterOperator Operator { get; init; }

    /// <summary>
    ///     String value to compare against.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

/// <summary>
///     Filters a keyed entry inside an object-valued column using a numeric comparison.
/// </summary>
public class NumberObjectEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.NumberObject;

    /// <summary>
    ///     Top-level key inside the object-valued column.
    /// </summary>
    [JsonPropertyOrder(2)]
    [JsonPropertyName("key")]
    public required string Key { get; init; }

    /// <summary>
    ///     Comparison operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleNumberFilterOperator Operator { get; init; }

    /// <summary>
    ///     Numeric value to compare against.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required double Value { get; init; }
}

/// <summary>
///     Filters a boolean-valued column (e.g. <c>isRootObservation</c>, <c>isExperimentItemRootSpan</c>).
/// </summary>
public class BooleanEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.Boolean;

    /// <summary>
    ///     Comparison operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleBooleanFilterOperator Operator { get; init; }

    /// <summary>
    ///     Boolean value to compare against.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public required bool Value { get; init; }
}

/// <summary>
///     Filters a column for null/not-null (e.g. <c>parentObservationId</c>).
/// </summary>
public class NullEvaluationRuleFilter : EvaluationRuleFilter
{
    /// <inheritdoc />
    [JsonPropertyOrder(0)]
    [JsonPropertyName("type")]
    public override EvaluationRuleFilterType Type => EvaluationRuleFilterType.Null;

    /// <summary>
    ///     Null-check operator.
    /// </summary>
    [JsonPropertyOrder(3)]
    [JsonPropertyName("operator")]
    public required EvaluationRuleNullFilterOperator Operator { get; init; }

    /// <summary>
    ///     Placeholder value, always the empty string. The API requires this field but ignores its content.
    /// </summary>
    [JsonPropertyOrder(4)]
    [JsonPropertyName("value")]
    public string Value => "";
}