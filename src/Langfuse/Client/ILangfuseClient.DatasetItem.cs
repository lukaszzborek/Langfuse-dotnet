using zborek.Langfuse.Models;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    Task<DatasetItem> CreateDatasetItemAsync(CreateDatasetItemRequest request,
        CancellationToken cancellationToken = default);

    Task<PaginatedDatasetItems> GetDatasetItemListAsync(DatasetItemListRequest request,
        CancellationToken cancellationToken = default);

    Task<DatasetItem> GetDatasetItemAsync(string id, CancellationToken cancellationToken = default);
    Task<DeleteDatasetItemResponse> DeleteDatasetItemAsync(string id, CancellationToken cancellationToken = default);
}