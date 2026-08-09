using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient : ILangfuseClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<LangfuseClient> _logger;

    public LangfuseClient(
        HttpClient httpClient,
        ILogger<LangfuseClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
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

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                
                _logger.LogDebug("Successfully completed {Operation} request to {Endpoint}. Response: {ResponseData}",
                    operationName, endpoint, responseContent);
                
                var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
                if (result == null)
                {
                    throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                        $"Failed to deserialize {operationName} response");
                }
                
                return result;
            }
            
            var responseResult = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);

            if (responseResult == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize {operationName} response");
            }

            return responseResult;
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
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogDebug("Successfully completed {Operation} request to {Endpoint}. Response: {ResponseData}",
                    operationName, endpoint, responseContent);

                var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
                if (result == null)
                {
                    throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                        $"Failed to deserialize {operationName} response");
                }

                return result;
            }

            var responseResult = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
            if (responseResult == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize {operationName} response");
            }

            return responseResult;
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
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogDebug("Successfully completed {Operation} request to {Endpoint}. Response: {ResponseData}",
                    operationName, endpoint, responseContent);
                
                var result = JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
                if (result == null)
                {
                    throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                        $"Failed to deserialize {operationName} response");
                }
                
                return result;
            }
            
            var responseResult = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
            
            if (responseResult == null)
            {
                throw new LangfuseApiException((int)HttpStatusCode.InternalServerError,
                    $"Failed to deserialize {operationName} response");
            }
            
            return responseResult;
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

        throw new LangfuseApiException(statusCode,
            $"API request failed with status code {statusCode} and response body : {errorContent}", errorContent);
    }
}
