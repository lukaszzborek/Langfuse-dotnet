using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     A metric displayed by a dashboard widget: a measure combined with an aggregation.
/// </summary>
public class DashboardWidgetMetric
{
    /// <summary>
    ///     Measure to aggregate, e.g. "count" or "latency".
    /// </summary>
    [JsonPropertyName("measure")]
    public required string Measure { get; init; }

    /// <summary>
    ///     Aggregation applied to the measure.
    /// </summary>
    [JsonPropertyName("agg")]
    public required DashboardWidgetMetricAggregation Agg { get; init; }
}