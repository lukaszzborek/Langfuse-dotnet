using zborek.Langfuse.Models.Score;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<ScoreListResponse> GetScoreListAsync(ScoreListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/v2/scores{queryString}";
        return await GetAsync<ScoreListResponse>(endpoint, "Get Score List", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ScoreModel> GetScoreAsync(string scoreId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scoreId))
        {
            throw new ArgumentException("Score ID cannot be null or empty", nameof(scoreId));
        }

        var endpoint = $"/api/public/v2/scores/{Uri.EscapeDataString(scoreId)}";
        return await GetAsync<ScoreModel>(endpoint, "Get Score", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<CreateScoreResponse> CreateScoreAsync(ScoreCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Score name cannot be null or empty", nameof(request));
        }

        if (request.Value == null)
        {
            throw new ArgumentException("Score value cannot be null", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.TraceId) &&
            string.IsNullOrWhiteSpace(request.ObservationId) &&
            string.IsNullOrWhiteSpace(request.SessionId))
        {
            throw new ArgumentException("At least one of TraceId, ObservationId, or SessionId must be provided",
                nameof(request));
        }

        const string endpoint = "/api/public/scores";
        return await PostAsync<CreateScoreResponse>(endpoint, request, "Create Score", cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteScoreAsync(string scoreId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(scoreId))
        {
            throw new ArgumentException("Score ID cannot be null or empty", nameof(scoreId));
        }

        var endpoint = $"/api/public/scores/{Uri.EscapeDataString(scoreId)}";
        await DeleteAsync(endpoint, "Delete Score", cancellationToken);
    }
}