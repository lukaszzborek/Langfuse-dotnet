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

        return await PostAsync<Evaluator>("/api/public/unstable/evaluators", request,
            "Create Evaluator", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PaginatedEvaluators> GetEvaluatorsAsync(
        int? page = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildPageLimitQuery(page, limit);
        var endpoint = $"/api/public/unstable/evaluators{query}";

        return await GetAsync<PaginatedEvaluators>(endpoint, "Get Evaluators", cancellationToken);
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

        var endpoint = $"/api/public/unstable/evaluators/{Uri.EscapeDataString(evaluatorId)}";
        return await GetAsync<Evaluator>(endpoint, "Get Evaluator", cancellationToken);
    }
}
