using zborek.Langfuse.Models.Model;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<PaginatedModels> GetModelListAsync(ModelListRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        return await GetAsync<PaginatedModels>($"/api/public/models{query}", "Get Model List", cancellationToken);
    }

    public async Task<Model> GetModelAsync(string modelId, CancellationToken cancellationToken = default)
    {
        return await GetAsync<Model>($"/api/public/models/{Uri.EscapeDataString(modelId)}", "Get Model",
            cancellationToken);
    }

    public async Task<Model> CreateModelAsync(CreateModelRequest request, CancellationToken cancellationToken = default)
    {
        return await PostAsync<Model>("/api/public/models", request, "Create Model", cancellationToken);
    }

    public async Task DeleteModelAsync(string modelId, CancellationToken cancellationToken = default)
    {
        await DeleteAsync($"/api/public/models/{Uri.EscapeDataString(modelId)}", "Delete Model", cancellationToken);
    }
}