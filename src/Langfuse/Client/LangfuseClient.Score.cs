using System.Text.Json;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<CreateScoreBatchResponse> CreateScoreAsync(ScoreCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateScoreCreateRequest(request, nameof(request));

        var response = await PostScoresAsync([request], cancellationToken);

        // The batch endpoint answers HTTP 207 for a rejected score, which would otherwise look like success.
        if (response.Rejected > 0)
        {
            var errors = string.Join("; ", (response.Errors ?? []).Select(e => e.Message));
            throw new LangfuseApiException(207, $"Score was rejected: {errors}",
                JsonSerializer.Serialize(response, JsonOptions));
        }

        return response;
    }

    /// <inheritdoc />
    public async Task<CreateScoreBatchResponse> CreateScoresAsync(IReadOnlyCollection<ScoreCreateRequest> requests,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(requests);

        if (requests.Count == 0)
        {
            throw new ArgumentException("At least one score must be provided", nameof(requests));
        }

        foreach (var request in requests)
        {
            ValidateScoreCreateRequest(request, nameof(requests));
        }

        return await PostScoresAsync(requests, cancellationToken);
    }

    private async Task<CreateScoreBatchResponse> PostScoresAsync(IReadOnlyCollection<ScoreCreateRequest> requests,
        CancellationToken cancellationToken)
    {
        // The endpoint queues scores and returns no id, so assign one the caller can use to find or delete them.
        foreach (var request in requests)
        {
            request.Id ??= Guid.NewGuid().ToString();
        }

        const string endpoint = "/api/public/scores";
        return await PostAsync<CreateScoreBatchResponse>(endpoint, requests, "Create Scores", cancellationToken);
    }

    private static void ValidateScoreCreateRequest(ScoreCreateRequest? request, string paramName)
    {
        if (request == null)
        {
            throw new ArgumentNullException(paramName);
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Score name cannot be null or empty", paramName);
        }

        if (request.Value == null)
        {
            throw new ArgumentException("Score value cannot be null", paramName);
        }

        if (string.IsNullOrWhiteSpace(request.TraceId) &&
            string.IsNullOrWhiteSpace(request.ObservationId) &&
            string.IsNullOrWhiteSpace(request.SessionId))
        {
            throw new ArgumentException("At least one of TraceId, ObservationId, or SessionId must be provided",
                paramName);
        }
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