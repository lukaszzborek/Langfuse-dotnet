using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Dataset;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Creates a new dataset run item that links a dataset item to execution results
    /// </summary>
    /// <param name="request">
    ///     Dataset run item creation parameters including dataset item reference, run name, and execution
    ///     results
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created dataset run item with execution metadata and results</returns>
    /// <exception cref="LangfuseApiException">Thrown when dataset run item creation fails</exception>
    /// <remarks>
    ///     Dataset run items connect dataset items to specific execution runs, allowing comparison of expected vs actual
    ///     outputs
    /// </remarks>
    Task<DatasetRunItem> CreateDataSetRunAsync(CreateDatasetRunItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a paginated list of dataset run items for a specific dataset and run
    /// </summary>
    /// <param name="request">Filter parameters including required dataset ID and run name, plus pagination options</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of dataset run items showing execution results for each dataset item in the run</returns>
    /// <exception cref="LangfuseApiException">Thrown when the dataset or run is not found, or an API error occurs</exception>
    /// <remarks>Both datasetId and runName are required parameters for filtering run items</remarks>
    Task<PaginatedDatasetRunItems> GetDatasetRunListAsync(DatasetRunItemListRequest request,
        CancellationToken cancellationToken = default);
}