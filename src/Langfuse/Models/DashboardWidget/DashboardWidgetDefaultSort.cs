using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     Default sort configuration for pivot table dashboard widgets.
/// </summary>
public class DashboardWidgetDefaultSort
{
    /// <summary>
    ///     Column to sort by.
    /// </summary>
    [JsonPropertyName("column")]
    public required string Column { get; init; }

    /// <summary>
    ///     Sort direction.
    /// </summary>
    [JsonPropertyName("order")]
    public required DashboardWidgetSortOrder Order { get; init; }
}