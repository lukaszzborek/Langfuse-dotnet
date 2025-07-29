using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

/// <summary>
///     Service for interacting with Langfuse health endpoints
/// </summary>
public interface IHealthService
{
    /// <summary>
    ///     Check health of API and database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Health status response</returns>
    Task<HealthResponse> GetHealthAsync(CancellationToken cancellationToken = default);
}