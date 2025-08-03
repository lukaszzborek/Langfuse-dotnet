using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Trace;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of traces with optional filtering
    /// </summary>
    /// <param name="request">
    ///     Filter and pagination parameters including name, user ID, session ID, tags, timestamps, and
    ///     metadata filters
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of traces with summary information, duration, and observation counts</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>
    ///     Traces represent top-level execution units that contain spans, generations, and events in a hierarchical
    ///     structure
    /// </remarks>
    Task<TraceListResponse> GetTraceListAsync(TraceListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single trace by its ID with detailed nested data
    /// </summary>
    /// <param name="traceId">Unique identifier of the trace</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The trace with the specified ID including nested observations</returns>
    /// <exception cref="LangfuseApiException">Thrown when the trace is not found or an API error occurs</exception>
    Task<TraceWithDetails> GetTraceAsync(string traceId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a single trace by its ID
    /// </summary>
    /// <param name="traceId">Unique identifier of the trace to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing the deletion status</returns>
    /// <exception cref="LangfuseApiException">Thrown when the trace is not found or an API error occurs</exception>
    Task<DeleteTraceResponse> DeleteTraceAsync(string traceId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes multiple traces based on filter criteria
    /// </summary>
    /// <param name="request">
    ///     Filter criteria for selecting traces to delete, including date ranges, user filters, and metadata
    ///     criteria
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing the deletion status and count of deleted traces</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>This action permanently deletes traces and all their nested observations (spans, generations, events)</remarks>
    Task<DeleteTraceResponse> DeleteTraceManyAsync(TraceListRequest? request = null,
        CancellationToken cancellationToken = default);
}