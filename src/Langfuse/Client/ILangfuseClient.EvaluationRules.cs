using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Creates an evaluation rule that defines what incoming data should be evaluated and how prompt
    ///     variables should be populated from that data
    /// </summary>
    /// <param name="request">Evaluation rule configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created evaluation rule</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>This is an unstable API surface and may evolve while the evaluation data model is redesigned</remarks>
    Task<EvaluationRule> CreateEvaluationRuleAsync(
        CreateEvaluationRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists evaluation rules in the authenticated project
    /// </summary>
    /// <param name="page">Optional 1-based page number. Defaults to 1.</param>
    /// <param name="limit">Optional maximum number of items per page. Defaults to 50.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of evaluation rules</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<PaginatedEvaluationRules> GetEvaluationRulesAsync(
        int? page = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a single evaluation rule by its identifier
    /// </summary>
    /// <param name="evaluationRuleId">Evaluation rule identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The evaluation rule</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<EvaluationRule> GetEvaluationRuleAsync(
        string evaluationRuleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an evaluation rule. Provide only the fields you want to change.
    /// </summary>
    /// <param name="evaluationRuleId">Evaluation rule identifier</param>
    /// <param name="request">Partial update body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated evaluation rule</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<EvaluationRule> UpdateEvaluationRuleAsync(
        string evaluationRuleId,
        UpdateEvaluationRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an evaluation rule. This removes the live-ingestion rule only; it does not delete the
    ///     referenced evaluator.
    /// </summary>
    /// <param name="evaluationRuleId">Evaluation rule identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<DeleteEvaluationRuleResponse> DeleteEvaluationRuleAsync(
        string evaluationRuleId,
        CancellationToken cancellationToken = default);
}
