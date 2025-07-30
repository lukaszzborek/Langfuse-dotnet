using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Services;

/// <summary>
///     Service for interacting with Langfuse score configuration endpoints
/// </summary>
public interface IScoreConfigService
{
    /// <summary>
    ///     Retrieves a paginated list of score configurations with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of score configurations</returns>
    Task<ScoreConfigListResponse> ListAsync(ScoreConfigListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single score configuration by its ID
    /// </summary>
    /// <param name="configId">Unique identifier of the score configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The score configuration with the specified ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when the score configuration is not found or an API error occurs</exception>
    Task<ScoreConfig> GetAsync(string configId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new score configuration
    /// </summary>
    /// <param name="request">Score configuration creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created score configuration</returns>
    /// <exception cref="LangfuseApiException">Thrown when score configuration creation fails</exception>
    Task<ScoreConfig> CreateAsync(CreateScoreConfigRequest request, CancellationToken cancellationToken = default);
}