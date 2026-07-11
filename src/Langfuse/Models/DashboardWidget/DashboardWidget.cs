using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     A reusable dashboard widget.
/// </summary>
/// <remarks>
///     This is an unstable API surface and may evolve while dashboard/widget APIs are being finalized.
/// </remarks>
public class DashboardWidget
{
    /// <summary>
    ///     Widget identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Timestamp when the widget was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTime CreatedAt { get; init; }

    /// <summary>
    ///     Timestamp when the widget was last updated.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public required DateTime UpdatedAt { get; init; }

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
    ///     Data view the widget is built on.
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
    ///     Chart-specific configuration.
    /// </summary>
    [JsonPropertyName("chartConfig")]
    public required DashboardWidgetChartConfig ChartConfig { get; init; }

    /// <summary>
    ///     Minimum widget version.
    /// </summary>
    [JsonPropertyName("minVersion")]
    public required int MinVersion { get; init; }
}