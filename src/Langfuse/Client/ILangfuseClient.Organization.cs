using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    Task<MembershipsResponse> GetMembershipsAsync(CancellationToken cancellationToken = default);

    Task<MembershipResponse> CreateOrUpdateMembershipAsync(CreateMembershipRequest request,
        CancellationToken cancellationToken = default);

    Task<MembershipsResponse> GetProjectMembershipsAsync(string projectId,
        CancellationToken cancellationToken = default);

    Task<MembershipResponse> CreateOrUpdateOrganizationProjectMembershipAsync(string projectId,
        CreateMembershipRequest request,
        CancellationToken cancellationToken = default);

    Task<OrganizationProjectsResponse> GetOrganizationProjectsAsync(CancellationToken cancellationToken = default);
}