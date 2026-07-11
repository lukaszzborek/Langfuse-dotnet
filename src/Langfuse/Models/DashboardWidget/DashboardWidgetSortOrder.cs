using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     Sort direction for a dashboard widget default sort.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<DashboardWidgetSortOrder>))]
public enum DashboardWidgetSortOrder
{
    /// <summary>
    ///     Ascending order.
    /// </summary>
    Asc,

    /// <summary>
    ///     Descending order.
    /// </summary>
    Desc
}