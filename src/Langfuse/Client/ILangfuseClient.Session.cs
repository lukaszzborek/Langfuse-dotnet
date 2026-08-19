using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Session;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of sessions with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters including user ID, date range, and session metadata filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of sessions with metadata, duration, and trace count information</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Sessions group related traces together, typically representing user interactions or conversation flows</remarks>
    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. In Langfuse v4, read session data via GetObservationsV2Async with a sessionId filter. Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
    Task<SessionListResponse> GetSessionListAsync(SessionListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single session by its ID with associated traces
    /// </summary>
    /// <param name="sessionId">Unique identifier of the session</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The session with the specified ID including associated traces</returns>
    /// <exception cref="LangfuseApiException">Thrown when the session is not found or an API error occurs</exception>
    [Obsolete("Deprecated: on Langfuse Cloud this endpoint will be removed on November 16, 2026. In Langfuse v4, read session data via GetObservationsV2Async with a sessionId filter. Self-hosted deployments lose the endpoint when upgrading to Langfuse v4. See https://langfuse.com/self-hosting/upgrade/upgrade-guides/upgrade-v3-to-v4")]
    Task<SessionModel> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);
}