using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     One filter condition of a stored <see cref="EvaluationRule" />, returned verbatim by the API.
///     Unlike the write-side <see cref="EvaluationRuleFilter" /> union, this shape is intentionally not broken
///     down by type: <see cref="Operator" /> and <see cref="Value" /> are read back as plain strings/elements.
/// </summary>
/// <remarks>
///     <para>Supported columns and their filter type:</para>
///     <list type="bullet">
///         <item>
///             <c>type</c> (stringOptions: SPAN, EVENT, GENERATION, AGENT, TOOL, CHAIN, RETRIEVER, EVALUATOR, EMBEDDING,
///             GUARDRAIL)
///         </item>
///         <item>
///             <c>name</c>, <c>environment</c>, <c>traceName</c>, <c>experimentId</c>, <c>datasetId</c> (stringOptions;
///             dataset ids, not names)
///         </item>
///         <item><c>level</c> (stringOptions: DEBUG, DEFAULT, WARNING, ERROR)</item>
///         <item><c>version</c>, <c>userId</c>, <c>sessionId</c> (string)</item>
///         <item><c>tags</c>, <c>calledToolNames</c> (arrayOptions)</item>
///         <item><c>metadata</c> (stringObject, requires <see cref="Key" />)</item>
///         <item><c>isRootObservation</c>, <c>isExperimentItemRootSpan</c> (boolean)</item>
///         <item><c>parentObservationId</c> (null, with <see cref="Value" /> = "")</item>
///         <item><c>toolCalls</c> (number of tool calls on the observation)</item>
///     </list>
///     <para>
///         Operators by type: string/stringObject: "=", "contains", "does not contain", "starts with", "ends with";
///         number/datetime: "=", "&gt;", "&lt;", "&gt;=", "&lt;="; stringOptions/categoryOptions: "any of", "none of";
///         arrayOptions: "any of", "none of", "all of"; boolean: "=", "&lt;&gt;"; null: "is null", "is not null".
///     </para>
///     <para>
///         Experiment scope is expressed with filters: <c>isExperimentItemRootSpan = true</c> limits execution to
///         experiment item roots and <c>datasetId</c> limits it to experiments for the selected datasets.
///     </para>
/// </remarks>
public class EvaluationRuleReadFilter
{
    /// <summary>
    ///     Filter type: "datetime", "string", "number", "stringOptions", "categoryOptions", "arrayOptions",
    ///     "stringObject", "numberObject", "boolean", or "null". Determines the required fields and value shape.
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    ///     Column to filter on.
    /// </summary>
    [JsonPropertyName("column")]
    public required string Column { get; init; }

    /// <summary>
    ///     Top-level key inside the object-valued column. Only present for object filters such as "metadata".
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }

    /// <summary>
    ///     Comparison operator. Valid values depend on the filter Type
    ///     (for example "=", "contains", "any of", "is null").
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value to compare against. The shape depends on the filter Type (string, number, ISO datetime string,
    ///     boolean, or array of strings). Read back from the API as a <see cref="System.Text.Json.JsonElement" />.
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; init; }
}