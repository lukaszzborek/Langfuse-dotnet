using zborek.Langfuse.Models;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    Task<DatasetRunItem> CreateDataSetRunAsync(CreateDatasetRunItemRequest request,
        CancellationToken cancellationToken = default);

    Task<PaginatedDatasetRunItems> GetDatasetRunListAsync(DatasetRunItemListRequest request,
        CancellationToken cancellationToken = default);
}