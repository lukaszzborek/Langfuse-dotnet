using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services.Interfaces;

public interface IDatasetRunItemService
{
    Task<DatasetRunItem> CreateAsync(CreateDatasetRunItemRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedDatasetRunItems> ListAsync(DatasetRunItemListRequest request, CancellationToken cancellationToken = default);
}