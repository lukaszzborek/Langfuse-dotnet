using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     A dimension (group-by field) of a dashboard widget.
/// </summary>
public class DashboardWidgetDimension
{
    /// <summary>
    ///     Field name to group by, e.g. "name" or "environment".
    /// </summary>
    [JsonPropertyName("field")]
    public required string Field { get; init; }
}