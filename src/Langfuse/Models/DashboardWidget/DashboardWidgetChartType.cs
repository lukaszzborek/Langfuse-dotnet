using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     Chart type used to render a dashboard widget.
/// </summary>
[JsonConverter(typeof(SnakeCaseUpperEnumConverter<DashboardWidgetChartType>))]
public enum DashboardWidgetChartType
{
    /// <summary>
    ///     Line chart over time.
    /// </summary>
    LineTimeSeries,

    /// <summary>
    ///     Area chart over time.
    /// </summary>
    AreaTimeSeries,

    /// <summary>
    ///     Bar chart over time.
    /// </summary>
    BarTimeSeries,

    /// <summary>
    ///     Horizontal bar chart of total values.
    /// </summary>
    HorizontalBar,

    /// <summary>
    ///     Vertical bar chart of total values.
    /// </summary>
    VerticalBar,

    /// <summary>
    ///     Pie chart of total values.
    /// </summary>
    Pie,

    /// <summary>
    ///     Single number display.
    /// </summary>
    Number,

    /// <summary>
    ///     Histogram of value distribution.
    /// </summary>
    Histogram,

    /// <summary>
    ///     Pivot table of total values.
    /// </summary>
    PivotTable
}