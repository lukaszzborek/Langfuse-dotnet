using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Session;

/// <summary>
///     Response containing a paginated list of sessions
/// </summary>
public class SessionListResponse : PaginatedResponse<SessionModel>
{
}