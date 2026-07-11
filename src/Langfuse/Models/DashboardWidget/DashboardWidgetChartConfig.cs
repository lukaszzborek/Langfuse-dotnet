using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     Chart-specific dashboard widget configuration.
/// </summary>
/// <remarks>
///     <see cref="Type" /> must match the widget's top-level chart type.
///     <see cref="RowLimit" /> applies to total-value charts and pivot tables,
///     <see cref="Bins" /> applies to histograms, and
///     <see cref="DefaultSort" /> applies to pivot tables.
/// </remarks>
public class DashboardWidgetChartConfig
{
    /// <summary>
    ///     Chart type. Must match the widget's top-level chart type.
    /// </summary>
    [JsonPropertyName("type")]
    public required DashboardWidgetChartType Type { get; init; }

    /// <summary>
    ///     Maximum number of rows for total-value charts and pivot tables.
    /// </summary>
    [JsonPropertyName("row_limit")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? RowLimit { get; init; }

    /// <summary>
    ///     Whether to show value labels on the chart.
    /// </summary>
    [JsonPropertyName("show_value_labels")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? ShowValueLabels { get; init; }

    /// <summary>
    ///     Number of bins for histograms.
    /// </summary>
    [JsonPropertyName("bins")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Bins { get; init; }

    /// <summary>
    ///     Default sort for pivot tables.
    /// </summary>
    [JsonPropertyName("defaultSort")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DashboardWidgetDefaultSort? DefaultSort { get; init; }
}