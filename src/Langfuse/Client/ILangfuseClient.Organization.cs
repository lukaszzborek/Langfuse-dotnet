using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Organization;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves all memberships for the organization associated with the API key
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of organization memberships with user details and roles</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Requires organization-scoped API key for access</remarks>
    Task<MembershipsResponse> GetMembershipsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or updates a membership for the organization associated with the API key
    /// </summary>
    /// <param name="request">Membership creation/update parameters including user email and role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created or updated membership with user and role details</returns>
    /// <exception cref="LangfuseApiException">Thrown when membership operation fails</exception>
    /// <remarks>Requires organization-scoped API key for access</remarks>
    Task<MembershipResponse> CreateOrUpdateMembershipAsync(CreateMembershipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all memberships for a specific project within the organization
    /// </summary>
    /// <param name="projectId">The unique identifier of the project</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of project memberships with user details and roles</returns>
    /// <exception cref="LangfuseApiException">Thrown when the project is not found or an API error occurs</exception>
    /// <remarks>Requires organization-scoped API key for access</remarks>
    Task<MembershipsResponse> GetProjectMembershipsAsync(string projectId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or updates a project membership for a user within the organization
    /// </summary>
    /// <param name="projectId">The unique identifier of the project</param>
    /// <param name="request">Membership creation/update parameters including user email and project role</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created or updated project membership with user and role details</returns>
    /// <exception cref="LangfuseApiException">Thrown when the project is not found or membership operation fails</exception>
    /// <remarks>Requires organization-scoped API key for access</remarks>
    Task<MembershipResponse> CreateOrUpdateOrganizationProjectMembershipAsync(string projectId,
        CreateMembershipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all projects associated with the organization
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of organization projects with project details and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Requires organization-scoped API key for access</remarks>
    Task<OrganizationProjectsResponse> GetOrganizationProjectsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a membership from the organization associated with the API key
    /// </summary>
    /// <param name="request">Membership deletion request containing user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation with user ID and message</returns>
    /// <exception cref="LangfuseApiException">Thrown when membership deletion fails</exception>
    /// <remarks>Requires organization-scoped API key for access</remarks>
    Task<MembershipDeletionResponse> DeleteOrganizationMembershipAsync(DeleteMembershipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a membership from a specific project within the organization
    /// </summary>
    /// <param name="projectId">The unique identifier of the project</param>
    /// <param name="request">Membership deletion request containing user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation with user ID and message</returns>
    /// <exception cref="LangfuseApiException">Thrown when the project is not found or membership deletion fails</exception>
    /// <remarks>Requires organization-scoped API key for access. The user must be a member of the organization.</remarks>
    Task<MembershipDeletionResponse> DeleteProjectMembershipAsync(string projectId, DeleteMembershipRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves all API keys for the organization associated with the API key
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of organization API keys with metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Requires organization-scoped API key for access. Secret keys are masked for security.</remarks>
    Task<OrganizationApiKeysResponse> GetOrganizationApiKeysAsync(CancellationToken cancellationToken = default);
}