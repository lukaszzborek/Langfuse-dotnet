using zborek.Langfuse.Models;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    Task<PaginatedDatasets> GetDatasetListAsync(DatasetListRequest request,
        CancellationToken cancellationToken = default);

    Task<Dataset> GetDatasetAsync(string datasetName, CancellationToken cancellationToken = default);
    Task<Dataset> CreateDatasetAsync(CreateDatasetRequest request, CancellationToken cancellationToken = default);

    Task<DatasetRunWithItems> GetDatasetRunAsync(string datasetName, string runName,
        CancellationToken cancellationToken = default);

    Task<DeleteDatasetRunResponse> DeleteDatasetRunAsync(string datasetName, string runName,
        CancellationToken cancellationToken = default);

    Task<PaginatedDatasetRuns> GetDatasetRunsAsync(string datasetName, DatasetRunListRequest request,
        CancellationToken cancellationToken = default);
}