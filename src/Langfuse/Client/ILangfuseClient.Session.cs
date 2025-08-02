using zborek.Langfuse.Models;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of sessions with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of sessions</returns>
    Task<SessionListResponse> GetSessionListAsync(SessionListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single session by its ID with associated traces
    /// </summary>
    /// <param name="sessionId">Unique identifier of the session</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The session with the specified ID including associated traces</returns>
    /// <exception cref="LangfuseApiException">Thrown when the session is not found or an API error occurs</exception>
    Task<Session> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}