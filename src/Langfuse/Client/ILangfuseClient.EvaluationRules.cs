using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Creates an evaluation rule that defines which incoming observations should be evaluated and how
    ///     prompt variables should be populated. Rules always use the latest version of each assigned evaluator.
    /// </summary>
    /// <param name="request">Evaluation rule configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created evaluation rule</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<EvaluationRule> CreateEvaluationRuleAsync(
        CreateEvaluationRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists evaluation rules in newest-first creation order, including legacy trace and dataset rules so
    ///     they can be inspected and migrated
    /// </summary>
    /// <param name="limit">Optional maximum number of items. Defaults to 50, cannot exceed 100.</param>
    /// <param name="cursor">Optional opaque cursor returned by the previous page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cursor-paginated page of evaluation rules</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<EvaluationRulesPage> GetEvaluationRulesAsync(
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets one evaluation rule, including a legacy trace or dataset rule, by its stable identifier
    /// </summary>
    /// <param name="evaluationRuleId">Stable evaluation-rule identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The evaluation rule</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<EvaluationRule> GetEvaluationRuleAsync(
        string evaluationRuleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an evaluation rule. Provide only the fields to change. Providing evaluator assignments
    ///     replaces the complete list; an empty list disables the rule. Legacy trace and dataset rules can only
    ///     be deactivated.
    /// </summary>
    /// <param name="evaluationRuleId">Stable evaluation-rule identifier</param>
    /// <param name="request">Partial update body</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated evaluation rule</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<EvaluationRule> UpdateEvaluationRuleAsync(
        string evaluationRuleId,
        UpdateEvaluationRuleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an evaluation rule. This removes the live-ingestion rule only; associated evaluators and
    ///     scores already produced by them are preserved.
    /// </summary>
    /// <param name="evaluationRuleId">Stable evaluation-rule identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<DeletedEvaluationRule> DeleteEvaluationRuleAsync(
        string evaluationRuleId,
        CancellationToken cancellationToken = default);
}