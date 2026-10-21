using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Creates a new score for a trace, observation or session
    /// </summary>
    /// <param name="request">Score creation parameters including name, value, trace/observation ID, and optional metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Batch result for the single score (see <see cref="CreateScoresAsync" />)</returns>
    /// <exception cref="LangfuseApiException">Thrown when score creation fails or the server rejects the score</exception>
    /// <remarks>
    ///     Scores are created asynchronously, so the response carries no score ID. When
    ///     <see cref="ScoreCreateRequest.Id" /> is null, the client sets it to a new GUID before sending.
    /// </remarks>
    Task<CreateScoreBatchResponse> CreateScoreAsync(ScoreCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates multiple scores in a single asynchronous batch request
    /// </summary>
    /// <param name="requests">Scores to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    ///     Batch result: "Accepted" message when all scores were accepted (HTTP 202), or accepted/rejected counts
    ///     with error messages on partial acceptance (HTTP 207)
    /// </returns>
    /// <exception cref="LangfuseApiException">Thrown when batch creation fails</exception>
    /// <remarks>
    ///     Do not automatically retry a partially accepted batch - accepted scores would be duplicated. Requests with a
    ///     null <see cref="ScoreCreateRequest.Id" /> get a new GUID before sending.
    /// </remarks>
    Task<CreateScoreBatchResponse> CreateScoresAsync(IReadOnlyCollection<ScoreCreateRequest> requests,
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