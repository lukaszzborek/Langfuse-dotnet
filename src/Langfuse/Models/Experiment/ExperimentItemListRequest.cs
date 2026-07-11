using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Experiment;

/// <summary>
///     Request parameters for listing experiment items.
/// </summary>
public class ExperimentItemListRequest
{
    /// <summary>
    ///     Comma-separated list of field groups to include.
    ///     Available groups: <c>core</c>, <c>dataset</c>, <c>io</c>, <c>metadata</c>, <c>itemMetadata</c>,
    ///     <c>experimentMetadata</c>, <c>scores</c>. If omitted, <c>core,dataset</c> is returned.
    /// </summary>
    [JsonPropertyName("fields")]
    public string? Fields { get; set; }

    /// <summary>
    ///     Number of experiment items to return per page. Maximum 100, default 50.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    ///     Number of scores to return per experiment item when <c>fields=scores</c> is requested. Maximum 50, default 50.
    /// </summary>
    [JsonPropertyName("scoreLimit")]
    public int? ScoreLimit { get; set; }

    /// <summary>
    ///     Versioned base64url cursor from the previous response page.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    ///     Retrieve only experiment items started on or after this datetime. Required.
    /// </summary>
    [JsonPropertyName("fromStartTime")]
    public required DateTime FromStartTime { get; set; }

    /// <summary>
    ///     Retrieve only experiment items started before this datetime.
    /// </summary>
    [JsonPropertyName("toStartTime")]
    public DateTime? ToStartTime { get; set; }

    /// <summary>
    ///     Comma-separated list of experiment IDs.
    /// </summary>
    [JsonPropertyName("experimentId")]
    public string? ExperimentId { get; set; }

    /// <summary>
    ///     Comma-separated list of experiment names.
    /// </summary>
    [JsonPropertyName("experimentName")]
    public string? ExperimentName { get; set; }

    /// <summary>
    ///     Comma-separated list of experiment item IDs.
    /// </summary>
    [JsonPropertyName("experimentItemId")]
    public string? ExperimentItemId { get; set; }

    /// <summary>
    ///     Comma-separated list of dataset IDs.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public string? DatasetId { get; set; }

    /// <summary>
    ///     JSON string containing an array of structured filter conditions.
    ///     Supported columns are <c>experimentId</c>, <c>experimentName</c>, <c>experimentItemId</c>, and <c>datasetId</c>.
    /// </summary>
    [JsonPropertyName("filter")]
    public string? Filter { get; set; }
}