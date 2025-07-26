using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

/// <summary>
///     Service for interacting with Langfuse observation endpoints
/// </summary>
public interface IObservationService
{
    /// <summary>
    ///     Retrieves a paginated list of observations with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of observations</returns>
    Task<ObservationListResponse> ListAsync(ObservationListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single observation by its ID
    /// </summary>
    /// <param name="observationId">Unique identifier of the observation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The observation with the specified ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when the observation is not found or an API error occurs</exception>
    Task<Observation> GetAsync(string observationId, CancellationToken cancellationToken = default);
}