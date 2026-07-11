using zborek.Langfuse.Models.Experiment;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Lists experiments with cursor-based pagination. Results are ordered by latest experiment activity descending.
    /// </summary>
    /// <param name="request">Request parameters. <see cref="ExperimentListRequest.FromStartTime" /> is required.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Experiments with cursor-based pagination metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<ExperimentsResponse> GetExperimentsAsync(
        ExperimentListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists experiment items with cursor-based pagination. Use this endpoint to export experiment item inputs,
    ///     outputs, expected outputs, metadata, and optionally item/trace scores. Results are ordered by time descending.
    /// </summary>
    /// <param name="request">Request parameters. <see cref="ExperimentItemListRequest.FromStartTime" /> is required.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Experiment items with cursor-based pagination metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<ExperimentItemsResponse> GetExperimentItemsAsync(
        ExperimentItemListRequest request,
        CancellationToken cancellationToken = default);
}