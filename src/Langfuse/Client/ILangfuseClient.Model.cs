using zborek.Langfuse.Models;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    Task<PaginatedModels> GetModelListAsync(ModelListRequest request, CancellationToken cancellationToken = default);
    Task<Model> GetModelAsync(string modelId, CancellationToken cancellationToken = default);
    Task<Model> CreateModelAsync(CreateModelRequest request, CancellationToken cancellationToken = default);
    Task DeleteModelAsync(string modelId, CancellationToken cancellationToken = default);
}