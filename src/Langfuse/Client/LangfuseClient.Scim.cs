using System.Text;
using System.Text.Json;
using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<ScimServiceProviderConfig> GetServiceProviderConfigAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/api/public/scim/ServiceProviderConfig", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode,
                $"Failed to get SCIM service provider config: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ScimServiceProviderConfig>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<List<ScimResourceType>> GetResourceTypesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/api/public/scim/ResourceTypes", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode,
                $"Failed to get SCIM resource types: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ScimResourceType>>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<List<ScimSchema>> GetSchemasAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/api/public/scim/Schemas", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get SCIM schemas: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<List<ScimSchema>>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<PaginatedScimUsers> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync("/api/public/scim/Users", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get SCIM users: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<PaginatedScimUsers>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<ScimUser> CreateUserAsync(CreateScimUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(request, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");

        var response = await _httpClient.PostAsync("/api/public/scim/Users", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to create SCIM user: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ScimUser>(responseContent, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<ScimUser> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response =
            await _httpClient.GetAsync($"/api/public/scim/Users/{Uri.EscapeDataString(userId)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get SCIM user: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ScimUser>(content, _jsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"/api/public/scim/Users/{Uri.EscapeDataString(userId)}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to delete SCIM user: {errorContent}");
        }
    }
}