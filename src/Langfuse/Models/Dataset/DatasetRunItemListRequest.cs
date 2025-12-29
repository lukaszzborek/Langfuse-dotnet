using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Request parameters for listing dataset run items.
/// </summary>
public class DatasetRunItemListRequest
{
    /// <summary>
    ///     ID of the dataset.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the dataset run.
    /// </summary>
    [JsonPropertyName("runName")]
    public string RunName { get; set; } = string.Empty;

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