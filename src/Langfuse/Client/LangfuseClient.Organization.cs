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

    /// <summary>
    ///     Delete a membership from the organization associated with the API key.
    ///     Requires organization-scoped API key.
    /// </summary>
    /// <param name="request">Membership deletion details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    public async Task<MembershipDeletionResponse> DeleteOrganizationMembershipAsync(
        DeleteMembershipRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        const string endpoint = "/api/public/organizations/memberships";
        return await DeleteWithBodyAsync<MembershipDeletionResponse>(endpoint, request,
            "Delete Organization Membership", cancellationToken);
    }

    /// <summary>
    ///     Delete a membership from a specific project.
    ///     The user must be a member of the organization.
    ///     Requires organization-scoped API key.
    /// </summary>
    /// <param name="projectId">The unique identifier of the project</param>
    /// <param name="request">Membership deletion details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    public async Task<MembershipDeletionResponse> DeleteProjectMembershipAsync(
        string projectId,
        DeleteMembershipRequest request,
        CancellationToken cancellationToken = default)
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
        return await DeleteWithBodyAsync<MembershipDeletionResponse>(endpoint, request,
            "Delete Project Membership", cancellationToken);
    }

    /// <summary>
    ///     Get all API keys for the organization associated with the API key.
    ///     Requires organization-scoped API key.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of organization API keys</returns>
    public async Task<OrganizationApiKeysResponse> GetOrganizationApiKeysAsync(
        CancellationToken cancellationToken = default)
    {
        const string endpoint = "/api/public/organizations/apiKeys";
        return await GetAsync<OrganizationApiKeysResponse>(endpoint, "Get Organization API Keys", cancellationToken);
    }
}