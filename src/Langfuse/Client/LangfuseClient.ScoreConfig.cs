using zborek.Langfuse.Models.Score;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<ScoreConfigListResponse> GetScoreConfigListAsync(ScoreConfigListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<ScoreConfigListResponse>($"/api/public/score-configs{queryString}",
            "Get Score Config List", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ScoreConfig> GetScoreConfigAsync(string configId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(configId))
        {
            throw new ArgumentException("Score configuration ID cannot be null or empty", nameof(configId));
        }

        return await GetAsync<ScoreConfig>($"/api/public/score-configs/{Uri.EscapeDataString(configId)}",
            "Get Score Config", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ScoreConfig> CreateScoreConfigAsync(CreateScoreConfigRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ArgumentException("Score configuration name cannot be null or empty", nameof(request));
        }

        return await PostAsync<ScoreConfig>("/api/public/score-configs", request, "Create Score Config",
            cancellationToken);
    }
}