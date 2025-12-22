using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient : ILangfuseClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly Channel<IIngestionEvent> _channel;
    private readonly IOptions<LangfuseConfig> _config;
    private readonly HttpClient _httpClient;
    private readonly ILogger<LangfuseClient> _logger;

    public LangfuseClient(
        HttpClient httpClient,
        Channel<IIngestionEvent> channel,
        IOptions<LangfuseConfig> config,
        ILogger<LangfuseClient> logger)
    {
        _httpClient = httpClient;
        _channel = channel;
        _config = config;
        _logger = logger;
    }

    [Obsolete("This method uses the legacy ingestion endpoint. Please use the OpenTelemetry endpoint instead. Learn more: https://langfuse.com/integrations/native/opentelemetry")]
    public async Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default)
    {
        await IngestInternalAsync(ingestionEvent, cancellationToken);
    }

    [Obsolete("This method uses the legacy ingestion endpoint. Please use the OpenTelemetry endpoint instead. Learn more: https://langfuse.com/integrations/native/opentelemetry")]
    public async Task IngestAsync(LangfuseTrace langfuseTrace, CancellationToken cancellationToken = default)
    {
        List<IIngestionEvent> events = langfuseTrace.GetEvents();
        foreach (var @event in events)
        {
            await IngestAsync(@event, cancellationToken);
        }
    }

    private async Task IngestInternalAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken)
    {
        if (_config.Value.BatchMode)
        {
            await _channel.Writer.WriteAsync(ingestionEvent, cancellationToken);
        }
        else
        {
            var ingestionRequest = new IngestionRequest
            {
                Batch = [ingestionEvent]
            };
            await IngestInternalAsync(ingestionRequest);
        }
    }

    // TODO: For optimalization
    internal async Task<IngestionResponse> IngestInternalAsync(IngestionRequest request)
    {
        const int maxBatchSizeBytes = 3_500_000; // 3.5MB in bytes

        // First check if the entire request is under the limit
        var json = JsonSerializer.Serialize(request, JsonOptions);
        if (Encoding.UTF8.GetByteCount(json) <= maxBatchSizeBytes)
        {
            // If under limit, process normally
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/public/ingestion", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to ingest event");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var ingestionResponse = JsonSerializer.Deserialize<IngestionResponse>(responseJson);

            if (ingestionResponse == null)
            {
                throw new Exception("Failed to parse ingestion response");
            }

            return ingestionResponse;
        }

        // If over limit, split into smaller batches
        List<object> allEvents = request.Batch.ToList();
        var currentBatch = new List<object>();

        // save all results
        var successResponses = new List<IngestionSuccessResponse>();
        var errorResponses = new List<IngestionErrorResponse>();

        foreach (var eventItem in allEvents)
        {
            currentBatch.Add(eventItem);

            // Check if adding this event would exceed the size limit
            var batchRequest = new IngestionRequest { Batch = currentBatch.ToArray() };
            var batchJson = JsonSerializer.Serialize(batchRequest, JsonOptions);

            if (Encoding.UTF8.GetByteCount(batchJson) <= maxBatchSizeBytes)
            {
                continue;
            }

            // Remove the last added event as it caused overflow
            currentBatch.RemoveAt(currentBatch.Count - 1);

            // Process current batch
            if (currentBatch.Count > 0)
            {
                var batchIngestionRequest = new IngestionRequest { Batch = currentBatch.ToArray() };
                var lastResponse = await IngestInternalAsync(batchIngestionRequest);
                successResponses.AddRange(lastResponse?.Successes ?? []);
                errorResponses.AddRange(lastResponse?.Errors ?? []);
            }

            // Start new batch with the overflow event
            currentBatch.Clear();
            currentBatch.Add(eventItem);
        }

        // Process any remaining events in the last batch
        if (currentBatch.Count > 0)
        {
            var finalBatchRequest = new IngestionRequest { Batch = currentBatch.ToArray() };
            var lastResponse = await IngestInternalAsync(finalBatchRequest);
            successResponses.AddRange(lastResponse?.Successes ?? []);
            errorResponses.AddRange(lastResponse?.Errors ?? []);
        }

        return new IngestionResponse
        {
            Successes = successResponses.ToArray(),
            Errors = errorResponses.ToArray()
        } ?? throw new Exception("No events were processed");
    }

    /// <summary>
    ///     Base method for executing HTTP requests with standardized logging and error handling
    /// </summary>
    private async Task<TResponse> ExecuteRequestAsync<TResponse>(
        Func<Task<HttpResponseMessage>> httpOperation,
        string operationName,
        string endpoint,
        object? requestData = null,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Starting {Operation} request to {Endpoint}. Request data: {RequestData}",
                operationName, endpoint, requestData);
        }

        try
        {
            var response = await httpOperation();
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);

            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize {operationName} response");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully completed {Operation} request to {Endpoint}. Response: {ResponseData}",
                    operationName, endpoint, responseContent);
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request for {Operation} to {Endpoint} was cancelled", operationName, endpoint);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {Operation} request to {Endpoint}", operationName, endpoint);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred during {operationName}", ex);
        }
    }

    /// <summary>
    ///     Executes GET requests with standardized handling
    /// </summary>
    private async Task<TResponse> GetAsync<TResponse>(
        string endpoint,
        string operationName,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        return await ExecuteRequestAsync<TResponse>(
            () => _httpClient.GetAsync(endpoint, cancellationToken),
            operationName,
            endpoint,
            null,
            cancellationToken);
    }

    /// <summary>
    ///     Executes POST requests with standardized handling
    /// </summary>
    private async Task<TResponse> PostAsync<TResponse>(
        string endpoint,
        object requestBody,
        string operationName,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await ExecuteRequestAsync<TResponse>(
            () => _httpClient.PostAsync(endpoint, content, cancellationToken),
            operationName,
            endpoint,
            requestBody,
            cancellationToken);
    }

    /// <summary>
    ///     Executes PUT requests with standardized handling
    /// </summary>
    private async Task<TResponse> PutAsync<TResponse>(
        string endpoint,
        object requestBody,
        string operationName,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await ExecuteRequestAsync<TResponse>(
            () => _httpClient.PutAsync(endpoint, content, cancellationToken),
            operationName,
            endpoint,
            requestBody,
            cancellationToken);
    }

    /// <summary>
    ///     Executes PATCH requests with standardized handling
    /// </summary>
    private async Task<TResponse> PatchAsync<TResponse>(
        string endpoint,
        object requestBody,
        string operationName,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await ExecuteRequestAsync<TResponse>(
            () => _httpClient.PatchAsync(endpoint, content, cancellationToken),
            operationName,
            endpoint,
            requestBody,
            cancellationToken);
    }

    /// <summary>
    ///     Executes PATCH requests with standardized handling (no response)
    /// </summary>
    private async Task PatchAsync(
        string endpoint,
        object requestBody,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        var content = new StringContent(JsonSerializer.Serialize(requestBody, JsonOptions), Encoding.UTF8,
            "application/json");

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Starting {Operation} request to {Endpoint}", operationName, endpoint);
        }

        try
        {
            var response = await _httpClient.PatchAsync(endpoint, content, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully completed {Operation} request to {Endpoint}", operationName, endpoint);
            }
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request for {Operation} to {Endpoint} was cancelled", operationName, endpoint);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {Operation} request to {Endpoint}", operationName, endpoint);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred during {operationName}", ex);
        }
    }

    /// <summary>
    ///     Executes DELETE requests with standardized handling
    /// </summary>
    private async Task DeleteAsync(
        string endpoint,
        string operationName,
        CancellationToken cancellationToken = default)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Starting {Operation} request to {Endpoint}", operationName, endpoint);
        }

        try
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully completed {Operation} request to {Endpoint}", operationName, endpoint);
            }
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request for {Operation} to {Endpoint} was cancelled", operationName, endpoint);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {Operation} request to {Endpoint}", operationName, endpoint);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred during {operationName}", ex);
        }
    }

    /// <summary>
    ///     Executes DELETE requests with standardized handling and returns response data
    /// </summary>
    private async Task<TResponse> DeleteAsync<TResponse>(
        string endpoint,
        string operationName,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Starting {Operation} request to {Endpoint}", operationName, endpoint);
        }

        try
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            await EnsureSuccessStatusCodeAsync(response);

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully completed {Operation} request to {Endpoint}. Response: {ResponseData}",
                    operationName, endpoint, responseContent);
            }

            var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize {operationName} response");
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request for {Operation} to {Endpoint} was cancelled", operationName, endpoint);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {Operation} request to {Endpoint}", operationName, endpoint);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred during {operationName}", ex);
        }
    }

    /// <summary>
    ///     Executes DELETE requests with standardized handling and returns response data (with custom status code validation)
    /// </summary>
    private async Task<TResponse> DeleteAsync<TResponse>(
        string endpoint,
        string operationName,
        HttpStatusCode expectedStatusCode,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Starting {Operation} request to {Endpoint}", operationName, endpoint);
        }

        try
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);

            // Custom status code validation
            if (response.StatusCode != expectedStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new LangfuseApiException((int)response.StatusCode,
                    $"Failed to {operationName.ToLower()}: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Successfully completed {Operation} request to {Endpoint}. Response: {ResponseData}",
                    operationName, endpoint, responseContent);
            }

            var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
            if (result == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize {operationName} response");
            }

            return result;
        }
        catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Request for {Operation} to {Endpoint} was cancelled", operationName, endpoint);
            throw;
        }
        catch (LangfuseApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during {Operation} request to {Endpoint}", operationName, endpoint);
            throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                $"An unexpected error occurred during {operationName}", ex);
        }
    }

    /// <summary>
    ///     Executes DELETE requests with a request body and standardized handling
    /// </summary>
    private async Task<TResponse> DeleteWithBodyAsync<TResponse>(
        string endpoint,
        object requestBody,
        string operationName,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        var json = JsonSerializer.Serialize(requestBody, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
        {
            Content = content
        };

        return await ExecuteRequestAsync<TResponse>(
            () => _httpClient.SendAsync(request, cancellationToken),
            operationName,
            endpoint,
            requestBody,
            cancellationToken);
    }

    /// <summary>
    ///     Enhanced status code validation with comprehensive HTTP status mapping
    /// </summary>
    private static async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        var statusCode = (int)response.StatusCode;

        throw new LangfuseApiException(statusCode, $"API request failed with status code {statusCode} and response body : {errorContent}", errorContent);
    }
}