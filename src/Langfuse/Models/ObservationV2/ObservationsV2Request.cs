using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.ObservationV2;

/// <summary>
///     Request parameters for the V2 observations list endpoint.
/// </summary>
public class ObservationsV2Request
{
    /// <summary>
    ///     Comma-separated list of field groups to include in the response.
    ///     Available groups: core, basic, time, io, metadata, model, usage, prompt, metrics.
    ///     If not specified, `core` and `basic` field groups are returned.
    /// </summary>
    /// <example>basic,usage,model</example>
    [JsonPropertyName("fields")]
    public string? Fields { get; set; }

    /// <summary>
    ///     Number of items to return per page. Maximum 1000, default 50.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    ///     Base64-encoded cursor for pagination.
    ///     Use the cursor from the previous response to get the next page.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    ///     Set to true to parse input/output fields as JSON, or false to return raw strings.
    ///     Defaults to false if not provided.
    /// </summary>
    [JsonPropertyName("parseIoAsJson")]
    public bool? ParseIoAsJson { get; set; }

    /// <summary>
    ///     Filter by observation name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Filter by user ID.
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    ///     Filter by observation type (e.g., "GENERATION", "SPAN", "EVENT").
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    ///     Filter by trace ID.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    ///     Optional filter for observations with a specific level (e.g., "DEBUG", "DEFAULT", "WARNING", "ERROR").
    /// </summary>
    [JsonPropertyName("level")]
    public LangfuseLogLevel? Level { get; set; }

    /// <summary>
    ///     Filter by parent observation ID.
    /// </summary>
    [JsonPropertyName("parentObservationId")]
    public string? ParentObservationId { get; set; }

    /// <summary>
    ///     Optional filter for observations where the environment is one of the provided values.
    /// </summary>
    [JsonPropertyName("environment")]
    public List<string>? Environment { get; set; }

    /// <summary>
    ///     Retrieve only observations with a start_time on or after this datetime (ISO 8601).
    /// </summary>
    [JsonPropertyName("fromStartTime")]
    public DateTime? FromStartTime { get; set; }

    /// <summary>
    ///     Retrieve only observations with a start_time before this datetime (ISO 8601).
    /// </summary>
    [JsonPropertyName("toStartTime")]
    public DateTime? ToStartTime { get; set; }

    /// <summary>
    ///     Optional filter to only include observations with a certain version.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    ///     JSON string containing an array of filter conditions.
    ///     When provided, this takes precedence over query parameter filters.
    /// </summary>
    [JsonPropertyName("filter")]
    public string? Filter { get; set; }
}
