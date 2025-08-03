using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Dataset;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Creates a new dataset item with input/output data and optional metadata
    /// </summary>
    /// <param name="request">Dataset item creation parameters including input, expected output, and metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created dataset item with assigned ID and timestamps</returns>
    /// <exception cref="LangfuseApiException">Thrown when dataset item creation fails</exception>
    Task<DatasetItem> CreateDatasetItemAsync(CreateDatasetItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a paginated list of dataset items with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters including dataset name, source trace/observation filters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of dataset items matching the specified criteria</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<PaginatedDatasetItems> GetDatasetItemListAsync(DatasetItemListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a specific dataset item by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the dataset item</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The dataset item with input, expected output, and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when the dataset item is not found or an API error occurs</exception>
    Task<DatasetItem> GetDatasetItemAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a dataset item and all its associated run items permanently
    /// </summary>
    /// <param name="id">The unique identifier of the dataset item to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation response</returns>
    /// <exception cref="LangfuseApiException">Thrown when the dataset item is not found or deletion fails</exception>
    /// <remarks>This action is irreversible and will permanently delete the dataset item and all associated run items</remarks>
    Task<DeleteDatasetItemResponse> DeleteDatasetItemAsync(string id, CancellationToken cancellationToken = default);
}