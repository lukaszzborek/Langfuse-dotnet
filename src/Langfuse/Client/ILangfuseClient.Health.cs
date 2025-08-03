using zborek.Langfuse.Models.Health;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Check health of API and database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health status response</returns>
    Task<HealthResponse> GetHealthAsync(CancellationToken cancellationToken = default);
}