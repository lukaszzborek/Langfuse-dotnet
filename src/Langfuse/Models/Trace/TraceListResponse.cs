using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Response containing a paginated list of traces
/// </summary>
public class TraceListResponse : PaginatedResponse<TraceModel>
{
    /// <summary>
    ///     Migration signal returned by deprecated endpoints.
    /// </summary>
    [JsonPropertyName("_deprecation")]
    public Deprecation? Deprecation { get; set; }
}