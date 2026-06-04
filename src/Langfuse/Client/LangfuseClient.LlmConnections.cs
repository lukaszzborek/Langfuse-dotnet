using zborek.Langfuse.Models.LlmConnection;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <summary>
    ///     Get all LLM connections in the project.
    /// </summary>
    /// <param name="page">Page number, starts at 1</param>
    /// <param name="limit">Limit of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of LLM connections</returns>
    public async Task<PaginatedLlmConnections> GetLlmConnectionsAsync(
        int? page = null,
        int? limit = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();

        if (page.HasValue)
        {
            queryParams.Add($"page={page.Value}");
        }

        if (limit.HasValue)
        {
            queryParams.Add($"limit={limit.Value}");
        }

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
        var endpoint = $"/api/public/llm-connections{query}";

        return await GetAsync<PaginatedLlmConnections>(endpoint, "Get LLM Connections", cancellationToken);
    }

    /// <summary>
    ///     Create or update an LLM connection.
    ///     The connection is upserted based on the provider name.
    /// </summary>
    /// <param name="request">LLM connection configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created or updated LLM connection</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
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

    /// <summary>
    ///     Delete an LLM connection by id.
    ///     Evaluators that depend on the deleted connection are automatically paused.
    /// </summary>
    /// <param name="id">The unique identifier of the LLM connection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <exception cref="ArgumentException">Thrown when id is null or empty</exception>
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