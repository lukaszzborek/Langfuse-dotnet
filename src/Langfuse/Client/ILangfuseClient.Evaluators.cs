using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Creates an evaluator that defines how Langfuse should score data. Use
    ///     <see cref="CreateLlmAsJudgeEvaluatorRequest" /> for LLM-as-a-judge evaluators or
    ///     <see cref="CreateCodeEvaluatorRequest" /> for code evaluators
    /// </summary>
    /// <param name="request">Evaluator configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created evaluator version</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>
    ///     If the name already exists in the project, a new version is created and returned.
    ///     This is an unstable API surface and may evolve while the evaluation data model is redesigned.
    /// </remarks>
    Task<Evaluator> CreateEvaluatorAsync(
        CreateEvaluatorRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists the evaluators available to the authenticated project, returning the latest version of each
    /// </summary>
    /// <param name="page">Optional 1-based page number. Defaults to 1.</param>
    /// <param name="limit">Optional maximum number of items per page. Defaults to 50.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of evaluators</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Results can include both project evaluators and Langfuse-managed evaluators</remarks>
    Task<PaginatedEvaluators> GetEvaluatorsAsync(
        int? page = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a single evaluator by id, including its prompt, output definition, model configuration, and variables
    /// </summary>
    /// <param name="evaluatorId">Evaluator identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The evaluator</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<Evaluator> GetEvaluatorAsync(
        string evaluatorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an evaluator including all of its stored versions
    /// </summary>
    /// <param name="evaluatorId">Evaluator identifier; may reference any version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>
    ///     The API returns 409 while evaluation rules still reference the evaluator; delete those rules first.
    ///     Langfuse-managed evaluators (scope=managed) cannot be deleted and return 403.
    ///     Scores already produced by the evaluator are not deleted.
    /// </remarks>
    Task<DeleteEvaluatorResponse> DeleteEvaluatorAsync(
        string evaluatorId,
        CancellationToken cancellationToken = default);
}