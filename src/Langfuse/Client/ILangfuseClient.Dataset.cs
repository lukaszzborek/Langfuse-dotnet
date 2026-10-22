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
}