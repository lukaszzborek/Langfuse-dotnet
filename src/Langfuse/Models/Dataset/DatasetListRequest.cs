using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Request parameters for listing datasets.
/// </summary>
public class DatasetListRequest
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