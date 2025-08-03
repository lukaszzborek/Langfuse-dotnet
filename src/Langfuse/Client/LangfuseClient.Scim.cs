using System.Text;
using System.Text.Json;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Scim;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<ScimServiceProviderConfig> GetServiceProviderConfigAsync(
        CancellationToken cancellationToken = default)
    {
        return await GetAsync<ScimServiceProviderConfig>("/api/public/scim/ServiceProviderConfig",
            "Get SCIM Service Provider Config", cancellationToken);
    }

    public async Task<List<ScimResourceType>> GetResourceTypesAsync(CancellationToken cancellationToken = default)
    {
        // This method needs transformation of the response, so using manual pattern
        var response = await _httpClient.GetAsync("/api/public/scim/ResourceTypes", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode,
                $"Failed to get SCIM resource types: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var resourceTypesResponse = JsonSerializer.Deserialize<ScimResourceTypesResponse>(content, JsonOptions)
                                    ?? throw new LangfuseApiException(500, "Failed to deserialize response");

        return resourceTypesResponse.Resources;
    }

    public async Task<List<ScimSchema>> GetSchemasAsync(CancellationToken cancellationToken = default)
    {
        // This method needs transformation of the response, so using manual pattern
        var response = await _httpClient.GetAsync("/api/public/scim/Schemas", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to get SCIM schemas: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var schemasResponse = JsonSerializer.Deserialize<ScimSchemasResponse>(content, JsonOptions)
                              ?? throw new LangfuseApiException(500, "Failed to deserialize response");

        return schemasResponse.Resources.Select(resource => new ScimSchema
        {
            Id = resource.Id,
            Name = resource.Name,
            Description = resource.Description,
            Attributes = new List<ScimAttribute>()
        }).ToList();
    }

    public async Task<PaginatedScimUsers> GetUsersAsync(
        string? filter = null,
        int? startIndex = null,
        int? count = null,
        CancellationToken cancellationToken = default)
    {
        // This method needs custom query string building, so using manual pattern
        var query = new List<string>();
        if (!string.IsNullOrEmpty(filter))
        {
            query.Add($"filter={Uri.EscapeDataString(filter)}");
        }

        if (startIndex.HasValue)
        {
            query.Add($"startIndex={startIndex.Value}");
        }

        if (count.HasValue)
        {
            query.Add($"count={count.Value}");
        }

        var queryString = query.Count > 0 ? "?" + string.Join("&", query) : "";
        return await GetAsync<PaginatedScimUsers>($"/api/public/scim/Users{queryString}", "Get SCIM Users",
            cancellationToken);
    }

    public async Task<ScimUser> CreateUserAsync(CreateScimUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // This method uses different content type (application/scim+json), so using manual pattern
        var json = JsonSerializer.Serialize(request, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/scim+json");

        var response = await _httpClient.PostAsync("/api/public/scim/Users", content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new LangfuseApiException((int)response.StatusCode, $"Failed to create SCIM user: {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<ScimUser>(responseContent, JsonOptions)
               ?? throw new LangfuseApiException(500, "Failed to deserialize response");
    }

    public async Task<ScimUser> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<ScimUser>($"/api/public/scim/Users/{Uri.EscapeDataString(userId)}", "Get SCIM User",
            cancellationToken);
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        await DeleteAsync($"/api/public/scim/Users/{Uri.EscapeDataString(userId)}", "Delete SCIM User",
            cancellationToken);
    }
}