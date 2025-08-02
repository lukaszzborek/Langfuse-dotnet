using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

/// <summary>
///     Represents a client for interacting with Langfuse api
/// </summary>
public interface ILangfuseClient
{
    /// <summary>
    ///     Service for interacting with Langfuse observation endpoints
    /// </summary>
    IObservationService Observations { get; }

    /// <summary>
    ///     Service for interacting with Langfuse trace endpoints
    /// </summary>
    ITraceService Traces { get; }

    /// <summary>
    ///     Service for interacting with Langfuse session endpoints
    /// </summary>
    ISessionService Sessions { get; }

    /// <summary>
    ///     Service for interacting with Langfuse score endpoints
    /// </summary>
    IScoreService Scores { get; }

    /// <summary>
    ///     Service for interacting with Langfuse prompt endpoints
    /// </summary>
    IPromptService Prompts { get; }

    /// <summary>
    ///     Service for interacting with Langfuse dataset endpoints
    /// </summary>
    IDatasetService Datasets { get; }

    /// <summary>
    ///     Service for interacting with Langfuse model endpoints
    /// </summary>
    IModelService Models { get; }

    /// <summary>
    ///     Service for interacting with Langfuse comment endpoints
    /// </summary>
    ICommentService Comments { get; }

    /// <summary>
    ///     Service for interacting with Langfuse metrics endpoints
    /// </summary>
    IMetricsService Metrics { get; }

    /// <summary>
    ///     Service for interacting with Langfuse health endpoints
    /// </summary>
    IHealthService Health { get; }

    /// <summary>
    ///     Service for interacting with Langfuse dataset item endpoints
    /// </summary>
    IDatasetItemService DatasetItems { get; }

    /// <summary>
    ///     Service for interacting with Langfuse dataset run item endpoints
    /// </summary>
    IDatasetRunItemService DatasetRunItems { get; }

    /// <summary>
    ///     Service for interacting with Langfuse score configuration endpoints
    /// </summary>
    IScoreConfigService ScoreConfigs { get; }

    /// <summary>
    ///     Service for interacting with Langfuse media endpoints
    /// </summary>
    IMediaService Media { get; }

    /// <summary>
    ///     Service for interacting with Langfuse annotation queue endpoints
    /// </summary>
    IAnnotationQueueService AnnotationQueues { get; }

    /// <summary>
    ///     Service for interacting with Langfuse organization endpoints
    /// </summary>
    IOrganizationService Organizations { get; }

    /// <summary>
    ///     Service for interacting with Langfuse project endpoints
    /// </summary>
    IProjectService Projects { get; }

    /// <summary>
    ///     Service for interacting with Langfuse SCIM endpoints
    /// </summary>
    IScimService Scim { get; }

    /// <summary>
    ///     Ingests the specified ingestion event
    /// </summary>
    /// <param name="ingestionEvent">Langfuse event</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Ingests the specified whole langfuse trace
    /// </summary>
    /// <param name="langfuseTrace">Langfuse trace with events, spans, generations</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task IngestAsync(LangfuseTrace langfuseTrace, CancellationToken cancellationToken = default);
}