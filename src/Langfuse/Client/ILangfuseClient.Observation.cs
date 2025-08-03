using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Observation;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of observations with optional filtering
    /// </summary>
    /// <param name="request">
    ///     Filter and pagination parameters including name, user ID, type, trace ID, level, parent
    ///     observation ID, environment, time range, and version
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of observations (events, spans, and generations) matching the specified criteria</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Supports filtering by observation level (DEBUG, DEFAULT, WARNING, ERROR) and multiple environments</remarks>
    Task<ObservationListResponse> GetObservationListAsync(ObservationListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single observation by its unique identifier
    /// </summary>
    /// <param name="observationId">The unique Langfuse identifier of an observation (can be an event, span, or generation)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The observation with detailed information including type, timestamps, input/output, and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when the observation is not found or an API error occurs</exception>
    Task<ObservationModel> GetObservationAsync(string observationId, CancellationToken cancellationToken = default);
}