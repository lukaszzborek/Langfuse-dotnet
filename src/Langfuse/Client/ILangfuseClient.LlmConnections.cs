using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.LlmConnection;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves all LLM connections for the current project
    /// </summary>
    /// <param name="page">Optional page number, starts at 1</param>
    /// <param name="limit">Optional limit of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of LLM connections with their configurations</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Secret keys are masked in the response for security</remarks>
    Task<PaginatedLlmConnections> GetLlmConnectionsAsync(
        int? page = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or updates an LLM connection configuration
    /// </summary>
    /// <param name="request">LLM connection upsert request containing provider, adapter, and credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created or updated LLM connection</returns>
    /// <exception cref="LangfuseApiException">Thrown when the upsert operation fails</exception>
    /// <remarks>The connection is upserted based on the provider name which must be unique within the project</remarks>
    Task<LlmConnection> UpsertLlmConnectionAsync(
        UpsertLlmConnectionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an LLM connection by id
    /// </summary>
    /// <param name="id">The unique identifier of the LLM connection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <exception cref="LangfuseApiException">Thrown when the delete operation fails</exception>
    /// <remarks>Evaluators that depend on the deleted connection are automatically paused</remarks>
    Task<DeleteLlmConnectionResponse> DeleteLlmConnectionAsync(
        string id,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all organization-level LLM connections
    /// </summary>
    /// <param name="page">Optional page number, starts at 1</param>
    /// <param name="limit">Optional limit of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of organization-level LLM connections</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Requires an organization-scoped API key. Secret keys are masked in the response.</remarks>
    Task<PaginatedLlmConnections> GetOrganizationLlmConnectionsAsync(
        int? page = null,
        int? limit = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or updates an organization-level LLM connection
    /// </summary>
    /// <param name="request">LLM connection upsert request containing provider, adapter, and credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created or updated LLM connection</returns>
    /// <exception cref="LangfuseApiException">Thrown when the upsert operation fails</exception>
    /// <remarks>
    ///     Requires an organization-scoped API key. The connection is upserted on provider and inherited by projects
    ///     without a provider override.
    /// </remarks>
    Task<LlmConnection> UpsertOrganizationLlmConnectionAsync(
        UpsertLlmConnectionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an organization-level LLM connection by id
    /// </summary>
    /// <param name="id">The unique identifier of the LLM connection</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <exception cref="LangfuseApiException">Thrown when the delete operation fails</exception>
    /// <remarks>
    ///     Requires an organization-scoped API key. Evaluators in projects without an override for the provider are
    ///     automatically paused.
    /// </remarks>
    Task<DeleteLlmConnectionResponse> DeleteOrganizationLlmConnectionAsync(
        string id,
        CancellationToken cancellationToken = default);
}
