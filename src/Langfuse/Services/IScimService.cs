using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Services;

public interface IScimService
{
    Task<ScimServiceProviderConfig> GetServiceProviderConfigAsync(CancellationToken cancellationToken = default);
    Task<List<ScimResourceType>> GetResourceTypesAsync(CancellationToken cancellationToken = default);
    Task<List<ScimSchema>> GetSchemasAsync(CancellationToken cancellationToken = default);
    Task<PaginatedScimUsers> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<ScimUser> CreateUserAsync(CreateScimUserRequest request, CancellationToken cancellationToken = default);
    Task<ScimUser> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
}