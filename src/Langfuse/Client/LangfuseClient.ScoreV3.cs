using zborek.Langfuse.Models.Score;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<GetScoresV3Response> GetScoresV3Async(ScoreV3ListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/v3/scores{queryString}";
        return await GetAsync<GetScoresV3Response>(endpoint, "Get Scores V3", cancellationToken);
    }
}