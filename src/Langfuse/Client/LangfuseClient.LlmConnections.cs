using zborek.Langfuse.Models.LlmConnection;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<PaginatedLlmConnections> GetLlmConnectionsAsync(
        int? page = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildPageLimitQuery(page, limit);
        var endpoint = $"/api/public/llm-connections{query}";

        return await GetAsync<PaginatedLlmConnections>(endpoint, "Get LLM Connections", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<LlmConnection> UpsertLlmConnectionAsync(
        UpsertLlmConnectionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PutAsync<LlmConnection>("/api/public/llm-connections", request,
            "Create/Update LLM Connection", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeleteLlmConnectionResponse> DeleteLlmConnectionAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("LLM connection ID cannot be null or empty", nameof(id));
        }

        var endpoint = $"/api/public/llm-connections/{Uri.EscapeDataString(id)}";
        return await DeleteAsync<DeleteLlmConnectionResponse>(endpoint, "Delete LLM Connection", cancellationToken);
    }
}