using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Response containing a paginated list of scores
/// </summary>
public class ScoreListResponse : PaginatedResponse<ScoreModel>
{
    /// <summary>
    ///     Migration signal returned by deprecated endpoints.
    /// </summary>
    [JsonPropertyName("_deprecation")]
    public Deprecation? Deprecation { get; set; }
}