using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Metrics;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves analytics metrics from the Langfuse project using a complex query structure
    /// </summary>
    /// <param name="request">
    ///     Comprehensive metrics query with view, dimensions, metrics, filters, time ranges, and optional
    ///     grouping
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Metrics response containing aggregated data based on the specified query parameters</returns>
    /// <exception cref="LangfuseApiException">Thrown when the query is invalid or an API error occurs</exception>
    /// <remarks>
    ///     Supports complex analytics queries with:
    ///     - Views: traces, observations, scores-numeric, scores-categorical
    ///     - Dimensions: grouping by fields like name, userId, sessionId
    ///     - Metrics: count, latency, value with aggregations (count, sum, avg, p95, histogram)
    ///     - Filters: column-based filtering with various operators
    ///     - Time dimensions: grouping by minute, hour, day, week, month, auto
    ///     - Advanced features: histogram binning (1-100 bins), row limits (1-1000)
    /// </remarks>
    Task<MetricsResponse> GetMetricsAsync(MetricsRequest request, CancellationToken cancellationToken = default);
}