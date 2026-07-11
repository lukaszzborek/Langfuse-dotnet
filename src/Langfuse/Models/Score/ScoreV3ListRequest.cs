using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Request parameters for listing v3 scores. Filters are combined with AND; comma-separated values within a
///     single filter are combined with OR.
/// </summary>
public class ScoreV3ListRequest
{
    /// <summary>
    ///     Number of items per page. Maximum 100, default 50. Requests with a limit greater than 100 return HTTP 400.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    ///     URL-safe base64 (base64url) cursor for pagination. Use the cursor from the previous response to get the
    ///     next page. Absent on the final page.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    ///     Comma-separated field groups to include in addition to the always-returned core fields.
    ///     Allowed: details, subject, annotation. Unknown names return HTTP 400.
    /// </summary>
    [JsonPropertyName("fields")]
    public string? Fields { get; set; }

    /// <summary>
    ///     Comma-separated list of score IDs to filter by
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     Comma-separated list of score names to filter by
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Comma-separated list of score sources to filter by (e.g. API, ANNOTATION, EVAL). Case-insensitive.
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    ///     Comma-separated list of data types to filter by (NUMERIC, BOOLEAN, CATEGORICAL, TEXT, CORRECTION).
    ///     Case-insensitive. Must be a single value when used with <see cref="Value" />, <see cref="ValueMin" /> or
    ///     <see cref="ValueMax" />, and must be NUMERIC when used with <see cref="ValueMin" /> or <see cref="ValueMax" />.
    /// </summary>
    [JsonPropertyName("dataType")]
    public string? DataType { get; set; }

    /// <summary>
    ///     Comma-separated list of environments to filter by
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    /// <summary>
    ///     Comma-separated list of score config IDs to filter by
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }

    /// <summary>
    ///     Comma-separated list of annotation queue IDs to filter by
    /// </summary>
    [JsonPropertyName("queueId")]
    public string? QueueId { get; set; }

    /// <summary>
    ///     Comma-separated list of author user IDs to filter by
    /// </summary>
    [JsonPropertyName("authorUserId")]
    public string? AuthorUserId { get; set; }

    /// <summary>
    ///     Comma-separated list of exact values to filter by. Requires a single <see cref="DataType" /> of NUMERIC,
    ///     BOOLEAN or CATEGORICAL. For BOOLEAN, each value must be "true" or "false"; for NUMERIC, each value must be
    ///     a finite number.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    ///     Inclusive lower bound on the numeric value. Requires <see cref="DataType" />=NUMERIC as a single value.
    /// </summary>
    [JsonPropertyName("valueMin")]
    public double? ValueMin { get; set; }

    /// <summary>
    ///     Inclusive upper bound on the numeric value. Requires <see cref="DataType" />=NUMERIC as a single value.
    /// </summary>
    [JsonPropertyName("valueMax")]
    public double? ValueMax { get; set; }

    /// <summary>
    ///     Comma-separated list of trace IDs to filter by. Mutually exclusive with <see cref="SessionId" /> and
    ///     <see cref="ExperimentId" />. May be combined with <see cref="ObservationId" /> to scope the observation
    ///     lookup to a specific trace.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    ///     Comma-separated list of session IDs to filter by. Mutually exclusive with <see cref="TraceId" />,
    ///     <see cref="ObservationId" /> and <see cref="ExperimentId" />.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    ///     Comma-separated list of observation IDs to filter by. Requires <see cref="TraceId" /> to be specified,
    ///     because observation IDs are scoped to a trace. Mutually exclusive with <see cref="SessionId" /> and
    ///     <see cref="ExperimentId" />.
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     Comma-separated list of dataset run IDs (experiment IDs) to filter by. Mutually exclusive with
    ///     <see cref="TraceId" />, <see cref="SessionId" /> and <see cref="ObservationId" />.
    /// </summary>
    [JsonPropertyName("experimentId")]
    public string? ExperimentId { get; set; }

    /// <summary>
    ///     Inclusive lower bound on the score timestamp
    /// </summary>
    [JsonPropertyName("fromTimestamp")]
    public DateTime? FromTimestamp { get; set; }

    /// <summary>
    ///     Exclusive upper bound on the score timestamp
    /// </summary>
    [JsonPropertyName("toTimestamp")]
    public DateTime? ToTimestamp { get; set; }
}