using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Request parameters for listing dataset runs.
/// </summary>
public class DatasetRunListRequest
{
    /// <summary>
    ///     Page number, starts at 1.
    /// </summary>
    [JsonPropertyName("page")]
    public int? Page { get; set; }

    /// <summary>
    ///     Limit of items per page.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }
}