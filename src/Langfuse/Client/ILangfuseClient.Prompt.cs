﻿using zborek.Langfuse.Models;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of all prompts with metadata
    /// </summary>
    /// <param name="request">Pagination parameters for the prompt list</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of prompt metadata including names, versions, and labels</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<PromptMetaListResponse> GetPromptListAsync(PromptListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a specific prompt by name with optional version or label targeting
    /// </summary>
    /// <param name="promptName">The unique name of the prompt to retrieve</param>
    /// <param name="version">Optional specific version number to retrieve</param>
    /// <param name="label">Optional label to target a specific labeled version (e.g., "production", "latest")</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The prompt with content, configuration, and version metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when the prompt is not found or an API error occurs</exception>
    /// <remarks>If neither version nor label is specified, returns the latest version</remarks>
    Task<Prompt> GetPromptAsync(string promptName, int? version = null, string? label = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new prompt with initial version
    /// </summary>
    /// <param name="request">Prompt creation parameters including name, content, configuration, and optional labels</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created prompt with assigned version number and metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when prompt creation fails</exception>
    /// <remarks>Creates the first version (v1) of a new prompt template</remarks>
    Task<Prompt> CreatePromptAsync(CreatePromptRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Updates a specific version of an existing prompt
    /// </summary>
    /// <param name="promptName">The name of the prompt to update</param>
    /// <param name="version">The specific version number to update</param>
    /// <param name="request">Prompt version update parameters including labels and metadata</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated prompt version with modified metadata</returns>
    /// <exception cref="LangfuseApiException">Thrown when the prompt version is not found or update fails</exception>
    /// <remarks>Allows updating labels and metadata for existing prompt versions without creating new versions</remarks>
    Task<Prompt> UpdatePromptVersionAsync(string promptName, int version, UpdatePromptVersionRequest request,
        CancellationToken cancellationToken = default);
}