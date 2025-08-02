using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    Task<PaginatedProjects> GetProjectsAsync(CancellationToken cancellationToken = default);
    Task<Project> CreateProjectAsync(CreateProjectRequest request, CancellationToken cancellationToken = default);

    Task<Project> UpdateProjectAsync(string projectId, UpdateProjectRequest request,
        CancellationToken cancellationToken = default);

    Task<ProjectDeletionResponse> DeleteProjectAsync(string projectId, CancellationToken cancellationToken = default);
    Task<ApiKeyList> GetApiKeysAsync(string projectId, CancellationToken cancellationToken = default);

    Task<ApiKeyResponse> CreateApiKeyAsync(string projectId, CreateApiKeyRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiKeyDeletionResponse> DeleteApiKeyAsync(string projectId, string apiKeyId,
        CancellationToken cancellationToken = default);
}