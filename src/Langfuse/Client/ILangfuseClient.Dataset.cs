using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Dataset;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of all datasets
    /// </summary>
    /// <param name="request">Pagination parameters for the dataset list</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of datasets with metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<PaginatedDatasets> GetDatasetListAsync(DatasetListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a specific dataset by its name
    /// </summary>
    /// <param name="datasetName">The unique name of the dataset to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dataset with the specified name</returns>
    /// <exception cref="LangfuseApiException">Thrown when the dataset is not found or an API error occurs</exception>
    Task<DatasetModel> GetDatasetAsync(string datasetName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new dataset
    /// </summary>
    /// <param name="request">Dataset creation parameters including name, description, and metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created dataset</returns>
    /// <exception cref="LangfuseApiException">Thrown when dataset creation fails</exception>
    Task<DatasetModel> CreateDatasetAsync(CreateDatasetRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a specific dataset run and its associated items
    /// </summary>
    /// <param name="datasetName">The name of the dataset containing the run</param>
    /// <param name="runName">The name of the dataset run to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dataset run with all its items and execution details</returns>
    /// <exception cref="LangfuseApiException">Thrown when the dataset run is not found or an API error occurs</exception>
    Task<DatasetRunWithItems> GetDatasetRunAsync(string datasetName, string runName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a dataset run and all its associated run items permanently
    /// </summary>
    /// <param name="datasetName">The name of the dataset containing the run</param>
    /// <param name="runName">The name of the dataset run to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation response</returns>
    /// <exception cref="LangfuseApiException">Thrown when the dataset run is not found or deletion fails</exception>
    /// <remarks>This action is irreversible and will permanently delete all run items associated with the run</remarks>
    Task<DeleteDatasetRunResponse> DeleteDatasetRunAsync(string datasetName, string runName,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a paginated list of dataset runs for a specific dataset
    /// </summary>
    /// <param name="datasetName">The name of the dataset to get runs for</param>
    /// <param name="request">Pagination parameters for the dataset runs list</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of dataset runs with execution metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when the dataset is not found or an API error occurs</exception>
    Task<PaginatedDatasetRuns> GetDatasetRunsAsync(string datasetName, DatasetRunListRequest request,
        CancellationToken cancellationToken = default);
}