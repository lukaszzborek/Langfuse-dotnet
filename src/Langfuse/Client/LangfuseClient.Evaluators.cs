using zborek.Langfuse.Models.Evaluation;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<Evaluator> CreateEvaluatorAsync(
        CreateEvaluatorRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PostAsync<Evaluator>("/api/public/v2/evaluators", request, "Create Evaluator",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EvaluatorsPage> GetEvaluatorsAsync(
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildCursorLimitQuery(limit, cursor);
        return await GetAsync<EvaluatorsPage>($"/api/public/v2/evaluators{query}", "Get Evaluators",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Evaluator> GetEvaluatorAsync(
        string evaluatorId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(evaluatorId))
        {
            throw new ArgumentException("Evaluator ID cannot be null or empty", nameof(evaluatorId));
        }

        var endpoint = $"/api/public/v2/evaluators/{Uri.EscapeDataString(evaluatorId)}";
        return await GetAsync<Evaluator>(endpoint, "Get Evaluator", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Evaluator> UpdateEvaluatorAsync(
        string evaluatorId,
        UpdateEvaluatorRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(evaluatorId))
        {
            throw new ArgumentException("Evaluator ID cannot be null or empty", nameof(evaluatorId));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"/api/public/v2/evaluators/{Uri.EscapeDataString(evaluatorId)}";
        return await PatchAsync<Evaluator>(endpoint, request, "Update Evaluator", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeletedEvaluator> DeleteEvaluatorAsync(
        string evaluatorId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(evaluatorId))
        {
            throw new ArgumentException("Evaluator ID cannot be null or empty", nameof(evaluatorId));
        }

        var endpoint = $"/api/public/v2/evaluators/{Uri.EscapeDataString(evaluatorId)}";
        return await DeleteAsync<DeletedEvaluator>(endpoint, "Delete Evaluator", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<EvaluatorVersionsPage> GetEvaluatorVersionsAsync(
        string evaluatorId,
        int? limit = null,
        string? cursor = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(evaluatorId))
        {
            throw new ArgumentException("Evaluator ID cannot be null or empty", nameof(evaluatorId));
        }

        var query = QueryStringHelper.BuildCursorLimitQuery(limit, cursor);
        var endpoint = $"/api/public/v2/evaluators/{Uri.EscapeDataString(evaluatorId)}/versions{query}";
        return await GetAsync<EvaluatorVersionsPage>(endpoint, "Get Evaluator Versions", cancellationToken);
    }
}