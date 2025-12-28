using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of scores with optional filtering
    /// </summary>
    /// <param name="request">
    ///     Filter and pagination parameters including name, user ID, trace ID, observation ID, data type,
    ///     and value filters
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of scores with numeric or categorical values and associated metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Supports filtering by score configuration name, data type (NUMERIC, CATEGORICAL, BOOLEAN), and value ranges</remarks>
    Task<ScoreListResponse> GetScoreListAsync(ScoreListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single score by its ID
    /// </summary>
    /// <param name="scoreId">Unique identifier of the score</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The score with the specified ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when the score is not found or an API error occurs</exception>
    Task<ScoreModel> GetScoreAsync(string scoreId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new score for a trace or observation
    /// </summary>
    /// <param name="request">Score creation parameters including name, value, trace/observation ID, and optional metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing the ID of the created score</returns>
    /// <exception cref="LangfuseApiException">Thrown when score creation fails</exception>
    /// <remarks>Scores can be numeric, categorical, or boolean values used for evaluation and analysis</remarks>
    Task<CreateScoreResponse> CreateScoreAsync(ScoreCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a score by its ID
    /// </summary>
    /// <param name="scoreId">Unique identifier of the score to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the deletion operation</returns>
    /// <exception cref="LangfuseApiException">Thrown when the score is not found or deletion fails</exception>
    Task DeleteScoreAsync(string scoreId, CancellationToken cancellationToken = default);
}