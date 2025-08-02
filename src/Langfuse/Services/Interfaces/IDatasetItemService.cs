using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services.Interfaces;

public interface IDatasetItemService
{
    Task<DatasetItem> CreateAsync(CreateDatasetItemRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedDatasetItems> ListAsync(DatasetItemListRequest request, CancellationToken cancellationToken = default);
    Task<DatasetItem> GetAsync(string id, CancellationToken cancellationToken = default);
    Task<DeleteDatasetItemResponse> DeleteAsync(string id, CancellationToken cancellationToken = default);
}