using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.DashboardWidget;

/// <summary>
///     Aggregation applied to a dashboard widget metric measure.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<DashboardWidgetMetricAggregation>))]
public enum DashboardWidgetMetricAggregation
{
    /// <summary>
    ///     Sum of values.
    /// </summary>
    Sum,

    /// <summary>
    ///     Average of values.
    /// </summary>
    Avg,

    /// <summary>
    ///     Count of items.
    /// </summary>
    Count,

    /// <summary>
    ///     Maximum value.
    /// </summary>
    Max,

    /// <summary>
    ///     Minimum value.
    /// </summary>
    Min,

    /// <summary>
    ///     50th percentile (median).
    /// </summary>
    P50,

    /// <summary>
    ///     75th percentile.
    /// </summary>
    P75,

    /// <summary>
    ///     90th percentile.
    /// </summary>
    P90,

    /// <summary>
    ///     95th percentile.
    /// </summary>
    P95,

    /// <summary>
    ///     99th percentile.
    /// </summary>
    P99,

    /// <summary>
    ///     Histogram distribution of values.
    /// </summary>
    Histogram,

    /// <summary>
    ///     Count of unique values.
    /// </summary>
    Uniq
}