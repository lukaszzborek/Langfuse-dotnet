using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Services;

public interface IOrganizationService
{
    Task<MembershipsResponse> GetMembershipsAsync(CancellationToken cancellationToken = default);

    Task<MembershipResponse> CreateOrUpdateMembershipAsync(CreateMembershipRequest request,
        CancellationToken cancellationToken = default);

    Task<MembershipsResponse> GetProjectMembershipsAsync(string projectId,
        CancellationToken cancellationToken = default);

    Task<MembershipResponse> CreateOrUpdateProjectMembershipAsync(string projectId, CreateMembershipRequest request,
        CancellationToken cancellationToken = default);

    Task<OrganizationProjectsResponse> GetProjectsAsync(CancellationToken cancellationToken = default);
}