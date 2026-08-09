using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Paginated response containing dataset run items.
/// </summary>
public class PaginatedDatasetRunItems : PaginatedResponse<DatasetRunItem>
{
    /// <summary>
    ///     Migration signal returned by deprecated endpoints.
    /// </summary>
    [JsonPropertyName("_deprecation")]
    public Deprecation? Deprecation { get; set; }
}