using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Session;

/// <summary>
///     Response containing a paginated list of sessions
/// </summary>
public class SessionListResponse : PaginatedResponse<SessionModel>
{
    /// <summary>
    ///     Migration signal returned by deprecated endpoints.
    /// </summary>
    [JsonPropertyName("_deprecation")]
    public Deprecation? Deprecation { get; set; }
}