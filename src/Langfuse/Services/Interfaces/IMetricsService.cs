using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;

namespace zborek.Langfuse.Services.Interfaces;

/// <summary>
///     Service for interacting with Langfuse metrics endpoints
/// </summary>
public interface IMetricsService
{
    /// <summary>
    ///     Get metrics from the Langfuse project using a query object
    /// </summary>
    /// <param name="request">Metrics query parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Metrics response containing query results</returns>
    Task<MetricsResponse> GetMetricsAsync(MetricsRequest request, CancellationToken cancellationToken = default);
}