using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

/// <summary>
///     Service for interacting with Langfuse trace endpoints
/// </summary>
public interface ITraceService
{
    /// <summary>
    ///     Retrieves a paginated list of traces with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of traces</returns>
    Task<TraceListResponse> ListAsync(TraceListRequest? request = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single trace by its ID with detailed nested data
    /// </summary>
    /// <param name="traceId">Unique identifier of the trace</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The trace with the specified ID including nested observations</returns>
    /// <exception cref="LangfuseApiException">Thrown when the trace is not found or an API error occurs</exception>
    Task<TraceWithDetails> GetAsync(string traceId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a single trace by its ID
    /// </summary>
    /// <param name="traceId">Unique identifier of the trace to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing the deletion status</returns>
    /// <exception cref="LangfuseApiException">Thrown when the trace is not found or an API error occurs</exception>
    Task<DeleteTraceResponse> DeleteAsync(string traceId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes multiple traces based on filter criteria
    /// </summary>
    /// <param name="request">Filter criteria for selecting traces to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing the deletion status</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<DeleteTraceResponse> DeleteManyAsync(TraceListRequest? request = null, CancellationToken cancellationToken = default);
}