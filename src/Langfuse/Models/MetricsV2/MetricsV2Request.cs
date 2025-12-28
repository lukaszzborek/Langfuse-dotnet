using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.MetricsV2;

/// <summary>
///     Request parameters for the V2 metrics API endpoint.
///     This object is serialized to JSON and passed as a query parameter.
/// </summary>
public class MetricsV2Request
{
    /// <summary>
    ///     The view to query. One of "observations", "scores-numeric", "scores-categorical".
    /// </summary>
    [JsonPropertyName("view")]
    public string View { get; set; } = string.Empty;

    /// <summary>
    ///     Optional. Fields to group by (see available dimensions for each view).
    /// </summary>
    [JsonPropertyName("dimensions")]
    public List<MetricsV2Dimension>? Dimensions { get; set; }

    /// <summary>
    ///     Required. At least one metric must be provided.
    /// </summary>
    [JsonPropertyName("metrics")]
    public List<MetricsV2Metric> Metrics { get; set; } = [];

    /// <summary>
    ///     Optional. Filter conditions.
    /// </summary>
    [JsonPropertyName("filters")]
    public List<MetricsV2Filter>? Filters { get; set; }

    /// <summary>
    ///     Optional. If provided, results will be grouped by time.
    /// </summary>
    [JsonPropertyName("timeDimension")]
    public MetricsV2TimeDimension? TimeDimension { get; set; }

    /// <summary>
    ///     Required. ISO datetime string for start of time range.
    /// </summary>
    [JsonPropertyName("fromTimestamp")]
    public string FromTimestamp { get; set; } = string.Empty;

    /// <summary>
    ///     Required. ISO datetime string for end of time range (must be after fromTimestamp).
    /// </summary>
    [JsonPropertyName("toTimestamp")]
    public string ToTimestamp { get; set; } = string.Empty;

    /// <summary>
    ///     Optional. Order by fields.
    /// </summary>
    [JsonPropertyName("orderBy")]
    public List<MetricsV2OrderBy>? OrderBy { get; set; }

    /// <summary>
    ///     Optional. Query-specific configuration.
    /// </summary>
    [JsonPropertyName("config")]
    public MetricsV2Config? Config { get; set; }
}

/// <summary>
///     Dimension field for grouping metrics.
/// </summary>
public class MetricsV2Dimension
{
    /// <summary>
    ///     Field to group by (see available dimensions for each view).
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;
}

/// <summary>
///     Metric definition specifying what to measure and how to aggregate.
/// </summary>
public class MetricsV2Metric
{
    /// <summary>
    ///     What to measure (see available measures for each view).
    /// </summary>
    [JsonPropertyName("measure")]
    public string Measure { get; set; } = string.Empty;

    /// <summary>
    ///     How to aggregate: "sum", "avg", "count", "max", "min", "p50", "p75", "p90", "p95", "p99", "histogram".
    /// </summary>
    [JsonPropertyName("aggregation")]
    public string Aggregation { get; set; } = string.Empty;
}

/// <summary>
///     Filter condition for metrics queries.
/// </summary>
public class MetricsV2Filter
{
    /// <summary>
    ///     Column to filter on (any dimension field).
    /// </summary>
    [JsonPropertyName("column")]
    public string Column { get; set; } = string.Empty;

    /// <summary>
    ///     Operator based on type.
    /// </summary>
    [JsonPropertyName("operator")]
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    ///     Value to compare against.
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; set; }

    /// <summary>
    ///     Data type: "datetime", "string", "number", "stringOptions", "categoryOptions", "arrayOptions",
    ///     "stringObject", "numberObject", "boolean", "null".
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     Required only for stringObject/numberObject types (e.g., metadata filtering).
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }
}

/// <summary>
///     Time dimension configuration for grouping by time.
/// </summary>
public class MetricsV2TimeDimension
{
    /// <summary>
    ///     One of "auto", "minute", "hour", "day", "week", "month".
    /// </summary>
    [JsonPropertyName("granularity")]
    public string Granularity { get; set; } = string.Empty;
}

/// <summary>
///     Order by configuration.
/// </summary>
public class MetricsV2OrderBy
{
    /// <summary>
    ///     Field to order by (dimension or metric alias).
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    ///     "asc" or "desc".
    /// </summary>
    [JsonPropertyName("direction")]
    public string Direction { get; set; } = string.Empty;
}

/// <summary>
///     Query-specific configuration.
/// </summary>
public class MetricsV2Config
{
    /// <summary>
    ///     Optional. Number of bins for histogram aggregation (1-100), default: 10.
    /// </summary>
    [JsonPropertyName("bins")]
    public int? Bins { get; set; }

    /// <summary>
    ///     Optional. Maximum number of rows to return (1-1000), default: 100.
    /// </summary>
    [JsonPropertyName("row_limit")]
    public int? RowLimit { get; set; }
}
