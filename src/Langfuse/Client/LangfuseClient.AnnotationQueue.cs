using zborek.Langfuse.Models.AnnotationQueue;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    public async Task<AnnotationQueueListResponse> ListQueuesAsync(AnnotationQueueListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/annotation-queues{queryString}";
        return await GetAsync<AnnotationQueueListResponse>(endpoint, "List Annotation Queues", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AnnotationQueueModel> GetQueueAsync(string queueId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueId))
        {
            throw new ArgumentException("Queue ID cannot be null or empty", nameof(queueId));
        }

        var endpoint = $"/api/public/annotation-queues/{Uri.EscapeDataString(queueId)}";
        return await GetAsync<AnnotationQueueModel>(endpoint, "Get Annotation Queue", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AnnotationQueueItemListResponse> ListItemsAsync(string queueId,
        AnnotationQueueItemListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueId))
        {
            throw new ArgumentException("Queue ID cannot be null or empty", nameof(queueId));
        }

        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/annotation-queues/{Uri.EscapeDataString(queueId)}/items{queryString}";
        return await GetAsync<AnnotationQueueItemListResponse>(endpoint, "List Annotation Queue Items",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AnnotationQueueItem> CreateItemAsync(string queueId, CreateAnnotationQueueItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueId))
        {
            throw new ArgumentException("Queue ID cannot be null or empty", nameof(queueId));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.ObjectId))
        {
            throw new ArgumentException("Object ID cannot be null or empty", nameof(request));
        }

        var endpoint = $"/api/public/annotation-queues/{Uri.EscapeDataString(queueId)}/items";
        return await PostAsync<AnnotationQueueItem>(endpoint, request, "Create Annotation Queue Item",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AnnotationQueueItem> GetItemAsync(string queueId, string itemId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueId))
        {
            throw new ArgumentException("Queue ID cannot be null or empty", nameof(queueId));
        }

        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("Item ID cannot be null or empty", nameof(itemId));
        }

        var endpoint =
            $"/api/public/annotation-queues/{Uri.EscapeDataString(queueId)}/items/{Uri.EscapeDataString(itemId)}";
        return await GetAsync<AnnotationQueueItem>(endpoint, "Get Annotation Queue Item", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AnnotationQueueItem> UpdateItemAsync(string queueId, string itemId,
        UpdateAnnotationQueueItemRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueId))
        {
            throw new ArgumentException("Queue ID cannot be null or empty", nameof(queueId));
        }

        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("Item ID cannot be null or empty", nameof(itemId));
        }

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint =
            $"/api/public/annotation-queues/{Uri.EscapeDataString(queueId)}/items/{Uri.EscapeDataString(itemId)}";
        return await PatchAsync<AnnotationQueueItem>(endpoint, request, "Update Annotation Queue Item",
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeleteAnnotationQueueItemResponse> DeleteItemAsync(string queueId, string itemId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueId))
        {
            throw new ArgumentException("Queue ID cannot be null or empty", nameof(queueId));
        }

        if (string.IsNullOrWhiteSpace(itemId))
        {
            throw new ArgumentException("Item ID cannot be null or empty", nameof(itemId));
        }

        var endpoint =
            $"/api/public/annotation-queues/{Uri.EscapeDataString(queueId)}/items/{Uri.EscapeDataString(itemId)}";

        try
        {
            var result =
                await DeleteAsync<DeleteAnnotationQueueItemResponse>(endpoint, "Delete Annotation Queue Item",
                    cancellationToken);
            return result;
        }
        catch (LangfuseApiException ex) when (ex.StatusCode == 500 && ex.Message.Contains("Failed to deserialize"))
        {
            // Special case: if deserialization fails, create a default success response
            // This handles cases where the API returns empty or malformed response bodies
            return new DeleteAnnotationQueueItemResponse { Success = true };
        }
    }
}