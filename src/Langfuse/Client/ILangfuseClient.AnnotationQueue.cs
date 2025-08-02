using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of annotation queues with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of annotation queues</returns>
    Task<AnnotationQueueListResponse> ListQueuesAsync(AnnotationQueueListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single annotation queue by its ID
    /// </summary>
    /// <param name="queueId">Unique identifier of the annotation queue</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The annotation queue with the specified ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when the annotation queue is not found or an API error occurs</exception>
    Task<AnnotationQueue> GetQueueAsync(string queueId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a paginated list of annotation queue items with optional filtering
    /// </summary>
    /// <param name="queueId">Unique identifier of the annotation queue</param>
    /// <param name="request">Filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of annotation queue items</returns>
    Task<AnnotationQueueItemListResponse> ListItemsAsync(string queueId, AnnotationQueueItemListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new annotation queue item
    /// </summary>
    /// <param name="queueId">Unique identifier of the annotation queue</param>
    /// <param name="request">Annotation queue item creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created annotation queue item</returns>
    /// <exception cref="LangfuseApiException">Thrown when annotation queue item creation fails</exception>
    Task<AnnotationQueueItem> CreateItemAsync(string queueId, CreateAnnotationQueueItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single annotation queue item by its ID
    /// </summary>
    /// <param name="queueId">Unique identifier of the annotation queue</param>
    /// <param name="itemId">Unique identifier of the annotation queue item</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The annotation queue item with the specified ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when the annotation queue item is not found or an API error occurs</exception>
    Task<AnnotationQueueItem> GetItemAsync(string queueId, string itemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates an annotation queue item
    /// </summary>
    /// <param name="queueId">Unique identifier of the annotation queue</param>
    /// <param name="itemId">Unique identifier of the annotation queue item</param>
    /// <param name="request">Annotation queue item update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated annotation queue item</returns>
    /// <exception cref="LangfuseApiException">Thrown when annotation queue item update fails</exception>
    Task<AnnotationQueueItem> UpdateItemAsync(string queueId, string itemId, UpdateAnnotationQueueItemRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an annotation queue item by its ID
    /// </summary>
    /// <param name="queueId">Unique identifier of the annotation queue</param>
    /// <param name="itemId">Unique identifier of the annotation queue item to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion response</returns>
    /// <exception cref="LangfuseApiException">Thrown when the annotation queue item is not found or deletion fails</exception>
    Task<DeleteAnnotationQueueItemResponse> DeleteItemAsync(string queueId, string itemId,
        CancellationToken cancellationToken = default);
}