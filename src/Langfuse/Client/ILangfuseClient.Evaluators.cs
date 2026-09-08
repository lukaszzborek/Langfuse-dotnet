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
    /// <returns>The created evaluator at version 1</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Names are not identifiers and do not need to be unique; this always creates a new evaluator.</remarks>
    Task<Evaluator> CreateEvaluatorAsync(
        CreateEvaluatorRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists evaluators in newest-first creation order. Every evaluator includes its latest definition and
    ///     version metadata plus associated evaluation rules
    /// </summary>
    /// <param name="limit">Optional maximum number of items. Defaults to 50, cannot exceed 100.</param>
    /// <param name="cursor">Optional opaque cursor returned by the previous page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cursor-paginated page of evaluators</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<EvaluatorsPage> GetEvaluatorsAsync(
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets one evaluator by its stable identifier, including its latest definition and version metadata
    ///     plus associated evaluation rules
    /// </summary>
    /// <param name="evaluatorId">Stable evaluator identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The evaluator</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<Evaluator> GetEvaluatorAsync(
        string evaluatorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an evaluator by its stable identifier. Metadata-only changes
    ///     (<see cref="UpdateEvaluatorMetadataRequest" />) do not create a version; definition replacements
    ///     (<see cref="UpdateLlmAsJudgeEvaluatorRequest" />, <see cref="UpdateCodeEvaluatorRequest" />) replace
    ///     the definition as a complete unit and create a new version. Evaluator type cannot change.
    /// </summary>
    /// <param name="evaluatorId">Stable evaluator identifier</param>
    /// <param name="request">Update body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated evaluator</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<Evaluator> UpdateEvaluatorAsync(
        string evaluatorId,
        UpdateEvaluatorRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an evaluator and all of its stored versions. Associated evaluation-rule assignments are also
    ///     removed; scores already produced by the evaluator are preserved
    /// </summary>
    /// <param name="evaluatorId">Stable evaluator identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<DeletedEvaluator> DeleteEvaluatorAsync(
        string evaluatorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists an evaluator's version history in newest-first order. Intended for history and audit use cases.
    /// </summary>
    /// <param name="evaluatorId">Stable evaluator identifier</param>
    /// <param name="limit">Optional maximum number of versions. Defaults to 50, cannot exceed 100.</param>
    /// <param name="cursor">Optional opaque cursor returned by the previous page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cursor-paginated page of evaluator versions</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<EvaluatorVersionsPage> GetEvaluatorVersionsAsync(
        string evaluatorId,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);
}