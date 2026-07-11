using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     A dashboard widget filter in Langfuse filter-state shape.
/// </summary>
/// <remarks>
///     Filter shapes depend on <see cref="Type" />: string filters use a string value,
///     option filters use a list of strings, and object filters include <see cref="Key" />.
/// </remarks>
public class DashboardWidgetFilter
{
    /// <summary>
    ///     Column the filter applies to.
    /// </summary>
    [JsonPropertyName("column")]
    public required string Column { get; init; }

    /// <summary>
    ///     Filter operator, e.g. "=", "contains", or "any of".
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Filter type, e.g. "string", "number", or "stringOptions".
    /// </summary>
    [JsonPropertyName("type")]
    public required string Type { get; init; }

    /// <summary>
    ///     Filter value. The expected shape depends on <see cref="Type" />.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Value { get; init; }

    /// <summary>
    ///     Object key for object filters such as "metadata".
    /// </summary>
    [JsonPropertyName("key")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Key { get; init; }
}