using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services.Interfaces;

public interface IModelService
{
    Task<PaginatedModels> ListAsync(ModelListRequest request, CancellationToken cancellationToken = default);
    Task<Model> GetAsync(string modelId, CancellationToken cancellationToken = default);
    Task<Model> CreateAsync(CreateModelRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string modelId, CancellationToken cancellationToken = default);
}