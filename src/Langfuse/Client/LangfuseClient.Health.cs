using zborek.Langfuse.Models.Health;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<HealthResponse> GetHealthAsync(CancellationToken cancellationToken = default)
    {
        const string endpoint = "/api/public/health";
        return await GetAsync<HealthResponse>(endpoint, "Get Health Status", cancellationToken);
    }
}