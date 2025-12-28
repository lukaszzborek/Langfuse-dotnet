using zborek.Langfuse.Models.Trace;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<TraceListResponse> GetTraceListAsync(TraceListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<TraceListResponse>($"/api/public/traces{queryString}", "Get Trace List",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TraceWithDetails> GetTraceAsync(string traceId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(traceId))
        {
            throw new ArgumentException("Trace ID cannot be null or empty", nameof(traceId));
        }

        return await GetAsync<TraceWithDetails>($"/api/public/traces/{Uri.EscapeDataString(traceId)}", "Get Trace",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeleteTraceResponse> DeleteTraceAsync(string traceId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(traceId))
        {
            throw new ArgumentException("Trace ID cannot be null or empty", nameof(traceId));
        }

        var endpoint = $"/api/public/traces/{Uri.EscapeDataString(traceId)}";
        return await DeleteAsync<DeleteTraceResponse>(endpoint, "Delete Trace", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeleteTraceResponse> DeleteTraceManyAsync(DeleteTraceManyRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.TraceIds == null || request.TraceIds.Length == 0)
        {
            throw new ArgumentException("At least one trace ID must be provided", nameof(request));
        }

        const string endpoint = "/api/public/traces";
        return await DeleteWithBodyAsync<DeleteTraceResponse>(endpoint, request,
            "Delete Multiple Traces", cancellationToken);
    }
}