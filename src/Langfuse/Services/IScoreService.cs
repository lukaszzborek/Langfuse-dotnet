using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Services;

/// <summary>
///     Service for interacting with Langfuse score endpoints
/// </summary>
public interface IScoreService
{
    /// <summary>
    ///     Retrieves a paginated list of scores with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of scores</returns>
    Task<ScoreListResponse> ListAsync(ScoreListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single score by its ID
    /// </summary>
    /// <param name="scoreId">Unique identifier of the score</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The score with the specified ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when the score is not found or an API error occurs</exception>
    Task<Score> GetAsync(string scoreId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new score
    /// </summary>
    /// <param name="request">Score creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created score</returns>
    /// <exception cref="LangfuseApiException">Thrown when score creation fails</exception>
    Task<Score> CreateAsync(ScoreCreateRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a score by its ID
    /// </summary>
    /// <param name="scoreId">Unique identifier of the score to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the deletion operation</returns>
    /// <exception cref="LangfuseApiException">Thrown when the score is not found or deletion fails</exception>
    Task DeleteAsync(string scoreId, CancellationToken cancellationToken = default);
}