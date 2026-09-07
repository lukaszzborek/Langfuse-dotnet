using zborek.Langfuse.Models.Evaluation;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<EvaluationRule> CreateEvaluationRuleAsync(
        CreateEvaluationRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PostAsync<EvaluationRule>("/api/public/v2/evaluation-rules", request,
            "Create Evaluation Rule", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EvaluationRulesPage> GetEvaluationRulesAsync(
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildCursorLimitQuery(limit, cursor);
        return await GetAsync<EvaluationRulesPage>($"/api/public/v2/evaluation-rules{query}",
            "Get Evaluation Rules", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EvaluationRule> GetEvaluationRuleAsync(
        string evaluationRuleId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(evaluationRuleId))
        {
            throw new ArgumentException("Evaluation rule ID cannot be null or empty", nameof(evaluationRuleId));
        }

        var endpoint = $"/api/public/v2/evaluation-rules/{Uri.EscapeDataString(evaluationRuleId)}";
        return await GetAsync<EvaluationRule>(endpoint, "Get Evaluation Rule", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EvaluationRule> UpdateEvaluationRuleAsync(
        string evaluationRuleId,
        UpdateEvaluationRuleRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(evaluationRuleId))
        {
            throw new ArgumentException("Evaluation rule ID cannot be null or empty", nameof(evaluationRuleId));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"/api/public/v2/evaluation-rules/{Uri.EscapeDataString(evaluationRuleId)}";
        return await PatchAsync<EvaluationRule>(endpoint, request, "Update Evaluation Rule", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeletedEvaluationRule> DeleteEvaluationRuleAsync(
        string evaluationRuleId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(evaluationRuleId))
        {
            throw new ArgumentException("Evaluation rule ID cannot be null or empty", nameof(evaluationRuleId));
        }

        var endpoint = $"/api/public/v2/evaluation-rules/{Uri.EscapeDataString(evaluationRuleId)}";
        return await DeleteAsync<DeletedEvaluationRule>(endpoint, "Delete Evaluation Rule", cancellationToken);
    }
}