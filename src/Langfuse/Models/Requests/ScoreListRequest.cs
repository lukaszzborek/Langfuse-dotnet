using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

/// <summary>
///     Request parameters for listing scores
/// </summary>
public class ScoreListRequest
{
    /// <summary>
    ///     Page number, starts at 1 (OpenAPI standard pagination)
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    ///     Number of items to return per page
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    ///     Filter by user ID associated to the trace
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
    
    /// <summary>
    ///     Filter by score name
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Filter by minimum timestamp
    /// </summary>
    [JsonPropertyName("fromTimestamp")]
    public DateTime? FromTimestamp { get; set; }

    /// <summary>
    ///     Filter by maximum timestamp
    /// </summary>
    [JsonPropertyName("toTimestamp")]
    public DateTime? ToTimestamp { get; set; }
    
    /// <summary>
    ///     Filter by environment
    /// </summary>
    [JsonPropertyName("environment")]
    public string[]? Environment { get; set; }
    
    /// <summary>
    ///     Filter by source
    /// </summary>
    [JsonPropertyName("source")]
    public ScoreSource? Source { get; set; }

    /// <summary>
    ///     Filter by operator
    /// </summary>
    [JsonPropertyName("operator")]
    public string? Operator { get; set; }

    /// <summary>
    ///     Order by value
    /// </summary>
    [JsonPropertyName("value")]
    public int? Value { get; set; }

    /// <summary>
    ///     Comma-separated list of score IDs to limit the results to
    /// </summary>
    [JsonPropertyName("scoreIds")]
    public string? ScoreIds { get; set; }

    /// <summary>
    ///     Retrieve only scores with a specific configId
    /// </summary>
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }
    
    /// <summary>
    ///     Retrieve only scores with a specific configId
    /// </summary>
    [JsonPropertyName("queueId")]
    public string? QueueId { get; set; }
    
    /// <summary>
    ///     Filter by data type
    /// </summary>
    [JsonPropertyName("dataType")]
    public ScoreDataType? DataType { get; set; }
    
    /// <summary>
    ///     Only scores linked to traces that include all of these tags will be returned
    /// </summary>
    [JsonPropertyName("traceTags")]
    public string[]? TraceTags { get; set; }
}