﻿using zborek.Langfuse.Models;
using zborek.Langfuse.Models.Requests;
using zborek.Langfuse.Models.Responses;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of score configurations with optional filtering
    /// </summary>
    /// <param name="request">Filter and pagination parameters for score configuration retrieval</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of score configurations defining evaluation criteria and data types</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>Score configurations define templates for creating scores with specific data types and validation rules</remarks>
    Task<ScoreConfigListResponse> GetScoreConfigListAsync(ScoreConfigListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a single score configuration by its ID
    /// </summary>
    /// <param name="configId">Unique identifier of the score configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The score configuration with the specified ID</returns>
    /// <exception cref="LangfuseApiException">Thrown when the score configuration is not found or an API error occurs</exception>
    Task<ScoreConfig> GetScoreConfigAsync(string configId, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new score configuration defining evaluation criteria
    /// </summary>
    /// <param name="request">Score configuration creation parameters including name, data type, description, and validation rules</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created score configuration with assigned ID and validation settings</returns>
    /// <exception cref="LangfuseApiException">Thrown when score configuration creation fails</exception>
    /// <remarks>Score configurations act as templates that define the structure and validation rules for scores</remarks>
    Task<ScoreConfig> CreateScoreConfigAsync(CreateScoreConfigRequest request,
        CancellationToken cancellationToken = default);
}