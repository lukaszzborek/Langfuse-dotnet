using System.Net;
using zborek.Langfuse.Models.Organization;
using zborek.Langfuse.Models.Project;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<PaginatedProjects> GetProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await GetAsync<PaginatedProjects>("/api/public/projects", "Get Projects", cancellationToken);
    }

    public async Task<ProjectModel> CreateProjectAsync(CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<ProjectModel>("/api/public/projects", request, "Create Project", cancellationToken);
    }

    public async Task<ProjectModel> UpdateProjectAsync(string projectId, UpdateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PutAsync<ProjectModel>($"/api/public/projects/{Uri.EscapeDataString(projectId)}", request,
            "Update Project", cancellationToken);
    }

    public async Task<ProjectDeletionResponse> DeleteProjectAsync(string projectId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));
        }

        var endpoint = $"/api/public/projects/{Uri.EscapeDataString(projectId)}";
        // Special case: DELETE project expects 202 Accepted instead of standard success codes
        return await DeleteAsync<ProjectDeletionResponse>(endpoint, "Delete Project", HttpStatusCode.Accepted,
            cancellationToken);
    }

    public async Task<ApiKeyList> GetApiKeysAsync(string projectId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<ApiKeyList>($"/api/public/projects/{Uri.EscapeDataString(projectId)}/apiKeys",
            "Get Project API Keys", cancellationToken);
    }

    public async Task<ApiKeyResponse> CreateApiKeyAsync(string projectId, CreateApiKeyRequest request,
        CancellationToken cancellationToken = default)
    {
        return await PostAsync<ApiKeyResponse>($"/api/public/projects/{Uri.EscapeDataString(projectId)}/apiKeys",
            request, "Create API Key", cancellationToken);
    }

    public async Task<ApiKeyDeletionResponse> DeleteApiKeyAsync(string projectId, string apiKeyId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));
        }

        if (string.IsNullOrWhiteSpace(apiKeyId))
        {
            throw new ArgumentException("API Key ID cannot be null or empty", nameof(apiKeyId));
        }

        var endpoint =
            $"/api/public/projects/{Uri.EscapeDataString(projectId)}/apiKeys/{Uri.EscapeDataString(apiKeyId)}";
        return await DeleteAsync<ApiKeyDeletionResponse>(endpoint, "Delete API Key", cancellationToken);
    }
}