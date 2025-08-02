using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;
using zborek.Langfuse.Services.Interfaces;

namespace zborek.Langfuse.Services;

/// <summary>
///     Implementation of annotation queue service for Langfuse API
/// </summary>
internal class AnnotationQueueService : IAnnotationQueueService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseUpper) }
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<AnnotationQueueService> _logger;

    /// <summary>
    ///     Initializes a new instance of the AnnotationQueueService class
    /// </summary>
    /// <param name="httpClient">HTTP client configured for Langfuse API</param>
    /// <param name="logger">Logger instance</param>
    public AnnotationQueueService(HttpClient httpClient, ILogger<AnnotationQueueService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<AnnotationQueueListResponse> ListQueuesAsync(AnnotationQueueListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var queryString = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/annotation-queues{queryString}";

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching annotation queues from endpoint: {Endpoint}", endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<AnnotationQueueListResponse>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize annotation queue list response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched {Count} annotation queues", result.Data.Length);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch annotation queues was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching annotation queues");
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred while fetching annotation queues", ex);
        }
    }

    /// <inheritdoc />
    public async Task<AnnotationQueue> GetQueueAsync(string queueId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(queueId))
        {
            throw new ArgumentException("Queue ID cannot be null or empty", nameof(queueId));
        }

        var endpoint = $"/api/public/annotation-queues/{Uri.EscapeDataString(queueId)}";
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching annotation queue {QueueId} from endpoint: {Endpoint}", queueId, endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<AnnotationQueue>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize annotation queue response for ID: {queueId}");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched annotation queue {QueueId}", queueId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch annotation queue {QueueId} was cancelled", queueId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching annotation queue {QueueId}", queueId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while fetching annotation queue {queueId}", ex);
        }
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

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching annotation queue items for queue {QueueId} from endpoint: {Endpoint}", queueId,
                endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result =
                JsonSerializer.Deserialize<AnnotationQueueItemListResponse>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize annotation queue item list response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched {Count} annotation queue items for queue {QueueId}",
                    result.Data.Length, queueId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch annotation queue items for queue {QueueId} was cancelled", queueId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching annotation queue items for queue {QueueId}", queueId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while fetching annotation queue items for queue {queueId}", ex);
        }
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
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug(
                "Creating annotation queue item for object {ObjectId} in queue {QueueId} at endpoint: {Endpoint}",
                request.ObjectId, queueId, endpoint);
        }

        try
        {
            var json = JsonSerializer.Serialize(request, SerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<AnnotationQueueItem>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize created annotation queue item response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(
                    "Successfully created annotation queue item {ItemId} for object {ObjectId} in queue {QueueId}",
                    result.Id, request.ObjectId, queueId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to create annotation queue item was cancelled");
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while creating annotation queue item for object {ObjectId} in queue {QueueId}",
                request.ObjectId, queueId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while creating annotation queue item for object {request.ObjectId} in queue {queueId}",
                ex);
        }
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
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Fetching annotation queue item {ItemId} from queue {QueueId} at endpoint: {Endpoint}",
                itemId, queueId, endpoint);
        }

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<AnnotationQueueItem>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize annotation queue item response for ID: {itemId}");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully fetched annotation queue item {ItemId} from queue {QueueId}", itemId,
                    queueId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to fetch annotation queue item {ItemId} from queue {QueueId} was cancelled",
                itemId, queueId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching annotation queue item {ItemId} from queue {QueueId}",
                itemId, queueId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while fetching annotation queue item {itemId} from queue {queueId}", ex);
        }
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
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Updating annotation queue item {ItemId} in queue {QueueId} at endpoint: {Endpoint}",
                itemId, queueId, endpoint);
        }

        try
        {
            var json = JsonSerializer.Serialize(request, SerializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<AnnotationQueueItem>(responseContent, SerializerOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    "Failed to deserialize updated annotation queue item response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully updated annotation queue item {ItemId} in queue {QueueId}", itemId,
                    queueId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to update annotation queue item {ItemId} in queue {QueueId} was cancelled",
                itemId, queueId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while updating annotation queue item {ItemId} in queue {QueueId}",
                itemId, queueId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while updating annotation queue item {itemId} in queue {queueId}", ex);
        }
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
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Deleting annotation queue item {ItemId} from queue {QueueId} at endpoint: {Endpoint}",
                itemId, queueId, endpoint);
        }

        try
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result =
                JsonSerializer.Deserialize<DeleteAnnotationQueueItemResponse>(responseContent, SerializerOptions);

            if (result == null)
            {
                result = new DeleteAnnotationQueueItemResponse { Success = true };
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully deleted annotation queue item {ItemId} from queue {QueueId}", itemId,
                    queueId);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request to delete annotation queue item {ItemId} from queue {QueueId} was cancelled",
                itemId, queueId);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while deleting annotation queue item {ItemId} from queue {QueueId}",
                itemId, queueId);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred while deleting annotation queue item {itemId} from queue {queueId}", ex);
        }
    }

    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        var statusCode = (int)response.StatusCode;

        var errorMessage = response.StatusCode switch
        {
            HttpStatusCode.NotFound => "The requested annotation queue or item was not found",
            HttpStatusCode.Unauthorized => "Authentication failed. Please check your API credentials",
            HttpStatusCode.Forbidden => "Access forbidden. You don't have permission to access this resource",
            HttpStatusCode.TooManyRequests => "Rate limit exceeded. Please retry after some time",
            HttpStatusCode.BadRequest => "Invalid request. Please check your request parameters",
            HttpStatusCode.Conflict => "Annotation queue item already exists or conflict with existing data",
            _ => $"API request failed with status code {statusCode}"
        };

        throw new LangfuseApiException(statusCode, errorMessage, details: new Dictionary<string, object>
        {
            ["responseContent"] = errorContent,
            ["statusCode"] = statusCode
        });
    }
}