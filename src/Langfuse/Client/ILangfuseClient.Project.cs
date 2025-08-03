using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of all projects accessible to the current user
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of projects with details and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<PaginatedProjects> GetProjectsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new project within the organization
    /// </summary>
    /// <param name="request">Project creation parameters including name, description, and configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created project with assigned ID and initial configuration</returns>
    /// <exception cref="LangfuseApiException">Thrown when project creation fails</exception>
    Task<Project> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an existing project's configuration and metadata
    /// </summary>
    /// <param name="projectId">The unique identifier of the project to update</param>
    /// <param name="request">Project update parameters including name, description, and settings</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated project with modified configuration</returns>
    /// <exception cref="LangfuseApiException">Thrown when the project is not found or update fails</exception>
    Task<Project> UpdateProjectAsync(string projectId, UpdateProjectRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a project and all its associated data permanently
    /// </summary>
    /// <param name="projectId">The unique identifier of the project to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation response</returns>
    /// <exception cref="LangfuseApiException">Thrown when the project is not found or deletion fails</exception>
    /// <remarks>This action is irreversible and will permanently delete all project data including traces, scores, and configurations</remarks>
    Task<ProjectDeletionResponse> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all API keys for a specific project
    /// </summary>
    /// <param name="projectId">The unique identifier of the project</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of API keys with metadata (secret keys are masked for security)</returns>
    /// <exception cref="LangfuseApiException">Thrown when the project is not found or an API error occurs</exception>
    Task<ApiKeyList> GetApiKeysAsync(string projectId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new API key for a project
    /// </summary>
    /// <param name="projectId">The unique identifier of the project</param>
    /// <param name="request">API key creation parameters including display name and permissions</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created API key with public key, secret key, and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when the project is not found or API key creation fails</exception>
    /// <remarks>Store the secret key securely as it will not be accessible again after creation</remarks>
    Task<ApiKeyResponse> CreateApiKeyAsync(string projectId, CreateApiKeyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an API key permanently, revoking all access
    /// </summary>
    /// <param name="projectId">The unique identifier of the project</param>
    /// <param name="apiKeyId">The unique identifier of the API key to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation response</returns>
    /// <exception cref="LangfuseApiException">Thrown when the project or API key is not found, or deletion fails</exception>
    /// <remarks>This action immediately revokes access for the API key and cannot be undone</remarks>
    Task<ApiKeyDeletionResponse> DeleteApiKeyAsync(string projectId, string apiKeyId,
        CancellationToken cancellationToken = default);
}