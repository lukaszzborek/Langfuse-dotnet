using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Metrics;

/// <summary>
///     Request parameters for metrics query
/// </summary>
public class MetricsRequest
{
    /// <summary>
    ///     JSON string containing the query parameters
    /// </summary>
    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;
}

/// <summary>
///     Strongly-typed metrics query structure
/// </summary>
public class MetricsQuery
{
    /// <summary>
    ///     Required. One of "traces", "observations", "scores-numeric", "scores-categorical"
    /// </summary>
    [JsonPropertyName("view")]
    public string View { get; set; } = string.Empty;

    /// <summary>
    ///     Optional. Default: []
    /// </summary>
    [JsonPropertyName("dimensions")]
    public MetricsDimension[] Dimensions { get; set; } = [];

    /// <summary>
    ///     Required. At least one metric must be provided
    /// </summary>
    [JsonPropertyName("metrics")]
    public Metric[] Metrics { get; set; } = [];

    /// <summary>
    ///     Optional. Default: []
    /// </summary>
    [JsonPropertyName("filters")]
    public MetricsFilter[] Filters { get; set; } = [];

    /// <summary>
    ///     Optional. Default: null. If provided, results will be grouped by time
    /// </summary>
    [JsonPropertyName("timeDimension")]
    public TimeDimension? TimeDimension { get; set; }
}

/// <summary>
///     Dimension for grouping metrics data
/// </summary>
public class MetricsDimension
{
    /// <summary>
    ///     Field to group by, e.g. "name", "userId", "sessionId"
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;
}

/// <summary>
///     Metric definition
/// </summary>
public class Metric
{
    /// <summary>
    ///     What to measure, e.g. "count", "latency", "value"
    /// </summary>
    [JsonPropertyName("measure")]
    public string Measure { get; set; } = string.Empty;

    /// <summary>
    ///     How to aggregate, e.g. "count", "sum", "avg", "p95", "histogram"
    /// </summary>
    [JsonPropertyName("aggregation")]
    public string Aggregation { get; set; } = string.Empty;
}

/// <summary>
///     Filter for metrics data
/// </summary>
public class MetricsFilter
{
    /// <summary>
    ///     Column to filter on
    /// </summary>
    [JsonPropertyName("column")]
    public string Column { get; set; } = string.Empty;

    /// <summary>
    ///     Operator, e.g. "=", "&gt;", "&lt;", "contains"
    /// </summary>
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    ///     Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    ///     Data type, e.g. "string", "number", "stringObject"
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     Required only when filtering on metadata
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }
}

/// <summary>
///     Time dimension for grouping results by time
/// </summary>
public class TimeDimension
{
    /// <summary>
    ///     Time period for grouping, e.g. "day", "hour", "week", "month"
    /// </summary>
    [JsonPropertyName("period")]
    public string Period { get; set; } = string.Empty;

    /// <summary>
    ///     Start date for the time range
    /// </summary>
    [JsonPropertyName("from")]
    public DateTime? From { get; set; }

    /// <summary>
    ///     End date for the time range
    /// </summary>
    [JsonPropertyName("to")]
    public DateTime? To { get; set; }
}