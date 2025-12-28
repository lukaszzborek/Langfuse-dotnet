using zborek.Langfuse.Models.Prompt;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<PromptMetaListResponse> GetPromptListAsync(PromptListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/v2/prompts{query}";
        return await GetAsync<PromptMetaListResponse>(endpoint, "List Prompts", cancellationToken);
    }

    public async Task<PromptModel> GetPromptAsync(string promptName, int? version = null, string? label = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(promptName))
        {
            throw new ArgumentException("Prompt name cannot be null or empty", nameof(promptName));
        }

        var queryParams = new List<string>();
        if (version.HasValue)
        {
            queryParams.Add($"version={version.Value}");
        }

        if (!string.IsNullOrEmpty(label))
        {
            queryParams.Add($"label={Uri.EscapeDataString(label)}");
        }

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var endpoint = $"/api/public/v2/prompts/{Uri.EscapeDataString(promptName)}{query}";
        return await GetAsync<PromptModel>(endpoint, "Get Prompt", cancellationToken);
    }

    public async Task<PromptModel> CreatePromptAsync(CreatePromptRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        const string endpoint = "/api/public/v2/prompts";
        return await PostAsync<PromptModel>(endpoint, request, "Create Prompt", cancellationToken);
    }

    public async Task<PromptModel> UpdatePromptVersionAsync(string promptName, int version,
        UpdatePromptVersionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(promptName))
        {
            throw new ArgumentException("Prompt name cannot be null or empty", nameof(promptName));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"/api/public/v2/prompts/{Uri.EscapeDataString(promptName)}/versions/{version}";
        return await PatchAsync<PromptModel>(endpoint, request, "Update Prompt Version", cancellationToken);
    }

    public async Task DeletePromptAsync(string promptName, int? version = null, string? label = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(promptName))
        {
            throw new ArgumentException("Prompt name cannot be null or empty", nameof(promptName));
        }

        var queryParams = new List<string>();
        if (version.HasValue)
        {
            queryParams.Add($"version={version.Value}");
        }

        if (!string.IsNullOrEmpty(label))
        {
            queryParams.Add($"label={Uri.EscapeDataString(label)}");
        }

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var endpoint = $"/api/public/v2/prompts/{Uri.EscapeDataString(promptName)}{query}";
        await DeleteAsync(endpoint, "Delete Prompt", cancellationToken);
    }
}