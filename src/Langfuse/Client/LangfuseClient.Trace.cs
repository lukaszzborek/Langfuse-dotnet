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
    public async Task<DeleteTraceResponse> DeleteTraceManyAsync(TraceListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/traces{queryString}";
        return await DeleteAsync<DeleteTraceResponse>(endpoint, "Delete Multiple Traces", cancellationToken);
    }
}