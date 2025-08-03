using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves SCIM service provider configuration and capabilities
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>SCIM service provider configuration including supported operations and features</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Enterprise feature: Provides SCIM 2.0 protocol compliance information for identity providers</remarks>
    Task<ScimServiceProviderConfig> GetServiceProviderConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves available SCIM resource types
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of supported SCIM resource types (Users, Groups, etc.)</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Enterprise feature: Defines the types of resources that can be managed via SCIM</remarks>
    Task<List<ScimResourceType>> GetResourceTypesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves SCIM schema definitions for user and group resources
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of SCIM schemas defining resource structure and attributes</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Enterprise feature: Provides schema definitions for SCIM resource attributes and validation rules</remarks>
    Task<List<ScimSchema>> GetSchemasAsync(CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a paginated list of users via SCIM protocol
    /// </summary>
    /// <param name="filter">Optional SCIM filter expression for user selection (e.g., 'userName eq "john@example.com"')</param>
    /// <param name="startIndex">Optional pagination start index (1-based)</param>
    /// <param name="count">Optional number of users to return per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of SCIM users with profile information</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Enterprise feature: SCIM 2.0 compliant user retrieval for identity provider integration</remarks>
    Task<PaginatedScimUsers> GetUsersAsync(string? filter = null, int? startIndex = null, int? count = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new user via SCIM protocol
    /// </summary>
    /// <param name="request">SCIM user creation request with profile attributes</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created SCIM user with assigned ID and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when user creation fails</exception>
    /// <remarks>Enterprise feature: Allows identity providers to provision new users automatically</remarks>
    Task<ScimUser> CreateUserAsync(CreateScimUserRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a specific user by ID via SCIM protocol
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The SCIM user with profile information and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when the user is not found or an API error occurs</exception>
    /// <remarks>Enterprise feature: SCIM 2.0 compliant user retrieval for identity provider synchronization</remarks>
    Task<ScimUser> GetUserAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a user via SCIM protocol
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    /// <exception cref="LangfuseApiException">Thrown when the user is not found or deletion fails</exception>
    /// <remarks>Enterprise feature: Allows identity providers to deprovision users automatically</remarks>
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
}