using zborek.Langfuse.Models;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of all available models
    /// </summary>
    /// <param name="request">Pagination parameters for the model list</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of models including both Langfuse-managed and custom models</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<PaginatedModels> GetModelListAsync(ModelListRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a specific model by its unique identifier
    /// </summary>
    /// <param name="modelId">The unique identifier of the model</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The model configuration including pricing, token limits, and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when the model is not found or an API error occurs</exception>
    Task<Model> GetModelAsync(string modelId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new custom model definition
    /// </summary>
    /// <param name="request">Model creation parameters including name, pricing, token limits, and metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created model with assigned ID and configuration</returns>
    /// <exception cref="LangfuseApiException">Thrown when model creation fails</exception>
    /// <remarks>Custom models can override Langfuse-managed model definitions with the same modelName</remarks>
    Task<Model> CreateModelAsync(CreateModelRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a custom model definition permanently
    /// </summary>
    /// <param name="modelId">The unique identifier of the model to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    /// <exception cref="LangfuseApiException">Thrown when the model is not found, cannot be deleted, or an API error occurs</exception>
    /// <remarks>Cannot delete models managed by Langfuse. Only custom model definitions can be deleted</remarks>
    Task DeleteModelAsync(string modelId, CancellationToken cancellationToken = default);
}