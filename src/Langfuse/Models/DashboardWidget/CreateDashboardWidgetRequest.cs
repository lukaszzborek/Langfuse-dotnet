using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     Request body for creating a reusable dashboard widget.
/// </summary>
/// <remarks>
///     Creating a widget does not place it on a dashboard grid; that has to be done in the UI.
///     This is an unstable API surface and may evolve while dashboard/widget APIs are being finalized.
/// </remarks>
public class CreateDashboardWidgetRequest
{
    /// <summary>
    ///     Widget name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Widget description.
    /// </summary>
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    /// <summary>
    ///     Data view the widget is built on. The legacy traces view is not supported by this unstable API.
    /// </summary>
    [JsonPropertyName("view")]
    public required DashboardWidgetView View { get; init; }

    /// <summary>
    ///     Dimensions (group-by fields) of the widget.
    /// </summary>
    [JsonPropertyName("dimensions")]
    public required DashboardWidgetDimension[] Dimensions { get; init; }

    /// <summary>
    ///     Metrics displayed by the widget.
    /// </summary>
    [JsonPropertyName("metrics")]
    public required DashboardWidgetMetric[] Metrics { get; init; }

    /// <summary>
    ///     Filters applied to the widget data.
    /// </summary>
    [JsonPropertyName("filters")]
    public required DashboardWidgetFilter[] Filters { get; init; }

    /// <summary>
    ///     Chart type used to render the widget.
    /// </summary>
    [JsonPropertyName("chartType")]
    public required DashboardWidgetChartType ChartType { get; init; }

    /// <summary>
    ///     Chart-specific configuration. Its type must match <see cref="ChartType" />.
    /// </summary>
    [JsonPropertyName("chartConfig")]
    public required DashboardWidgetChartConfig ChartConfig { get; init; }

    /// <summary>
    ///     Minimum widget version. Defaults to 2; values below 2 are rejected.
    /// </summary>
    [JsonPropertyName("minVersion")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? MinVersion { get; init; }
}