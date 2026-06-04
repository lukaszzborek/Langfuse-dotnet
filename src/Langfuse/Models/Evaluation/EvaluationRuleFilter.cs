using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     One filter condition used to decide whether a live-ingested target should be evaluated.
///     The valid Type, Column, Operator, and Value shape depend on the evaluation rule target;
///     see the Langfuse API documentation for the supported combinations.
/// </summary>
public class EvaluationRuleFilter
{
    /// <summary>
    ///     Filter type, for example "string", "number", "datetime", "stringOptions", "arrayOptions",
    ///     "stringObject", "boolean", or "null".
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    ///     Column to filter on.
    /// </summary>
    [JsonPropertyName("column")]
    public required string Column { get; init; }

    /// <summary>
    ///     Top-level key inside the object-valued column. Only used for object filters such as "metadata".
    /// </summary>
    [JsonPropertyName("key")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Key { get; init; }

    /// <summary>
    ///     Comparison operator. Valid values depend on the filter Type
    ///     (for example "=", "contains", "any of", "is null").
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value to compare against. The shape depends on the filter Type
    ///     (string, number, boolean, or array of strings).
    /// </summary>
    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Value { get; init; }
}
