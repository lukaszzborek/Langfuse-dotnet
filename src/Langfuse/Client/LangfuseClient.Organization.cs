using zborek.Langfuse.Models.Organization;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<MembershipsResponse> GetMembershipsAsync(CancellationToken cancellationToken = default)
    {
        const string endpoint = "/api/public/organizations/memberships";
        return await GetAsync<MembershipsResponse>(endpoint, "Get Organization Memberships", cancellationToken);
    }

    public async Task<MembershipResponse> CreateOrUpdateMembershipAsync(CreateMembershipRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        const string endpoint = "/api/public/organizations/memberships";
        return await PutAsync<MembershipResponse>(endpoint, request, "Create/Update Organization Membership",
            cancellationToken);
    }

    public async Task<MembershipsResponse> GetProjectMembershipsAsync(string projectId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));
        }

        var endpoint = $"/api/public/projects/{Uri.EscapeDataString(projectId)}/memberships";
        return await GetAsync<MembershipsResponse>(endpoint, "Get Project Memberships", cancellationToken);
    }

    public async Task<MembershipResponse> CreateOrUpdateOrganizationProjectMembershipAsync(string projectId,
        CreateMembershipRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException("Project ID cannot be null or empty", nameof(projectId));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"/api/public/projects/{Uri.EscapeDataString(projectId)}/memberships";
        return await PutAsync<MembershipResponse>(endpoint, request, "Create/Update Project Membership",
            cancellationToken);
    }

    public async Task<OrganizationProjectsResponse> GetOrganizationProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        const string endpoint = "/api/public/organizations/projects";
        return await GetAsync<OrganizationProjectsResponse>(endpoint, "Get Organization Projects", cancellationToken);
    }
}