using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

public interface IDatasetService
{
    Task<PaginatedDatasets> ListAsync(DatasetListRequest request, CancellationToken cancellationToken = default);
    Task<Dataset> GetAsync(string datasetName, CancellationToken cancellationToken = default);
    Task<Dataset> CreateAsync(CreateDatasetRequest request, CancellationToken cancellationToken = default);

    Task<DatasetRunWithItems> GetRunAsync(string datasetName, string runName,
        CancellationToken cancellationToken = default);

    Task<DeleteDatasetRunResponse> DeleteRunAsync(string datasetName, string runName,
        CancellationToken cancellationToken = default);

    Task<PaginatedDatasetRuns> GetRunsAsync(string datasetName, DatasetRunListRequest request,
        CancellationToken cancellationToken = default);
}