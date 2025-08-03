using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Response containing a paginated list of traces
/// </summary>
public class TraceListResponse : PaginatedResponse<TraceModel>
{
}