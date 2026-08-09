using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Observation;

/// <summary>
///     Response containing a paginated list of observations
/// </summary>
public class ObservationListResponse : PaginatedResponse<ObservationModel>
{
    /// <summary>
    ///     Migration signal returned by deprecated endpoints.
    /// </summary>
    [JsonPropertyName("_deprecation")]
    public Deprecation? Deprecation { get; set; }
}