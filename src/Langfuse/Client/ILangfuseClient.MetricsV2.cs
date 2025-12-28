using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.MetricsV2;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Get metrics from the Langfuse project using a query object. V2 endpoint with optimized performance.
    /// </summary>
    /// <param name="request">
    ///     The metrics query request containing view, dimensions, metrics, filters, time range, and other parameters.
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Metrics data with dynamic structure based on the query parameters</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>
    ///     <para>V2 Differences from V1:</para>
    ///     <list type="bullet">
    ///         <item>Supports observations, scores-numeric, and scores-categorical views only (traces view not supported)</item>
    ///         <item>Direct access to tags and release fields on observations</item>
    ///         <item>High cardinality dimensions are not supported and will return a 400 error</item>
    ///     </list>
    /// </remarks>
    Task<MetricsV2Response> GetMetricsV2Async(MetricsV2Request request, CancellationToken cancellationToken = default);
}