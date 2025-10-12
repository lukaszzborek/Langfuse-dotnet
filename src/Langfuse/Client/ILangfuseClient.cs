using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

/// <summary>
///     Represents a client for interacting with Langfuse api
/// </summary>
public partial interface ILangfuseClient
{
    /// <summary>
    ///     Ingests the specified ingestion event
    /// </summary>
    /// <param name="ingestionEvent">Langfuse event</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("This method uses the legacy ingestion endpoint. Please use the OpenTelemetry endpoint instead. Learn more: https://langfuse.com/integrations/native/opentelemetry")]
    Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Ingests the specified whole langfuse trace
    /// </summary>
    /// <param name="langfuseTrace">Langfuse trace with events, spans, generations</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("This method uses the legacy ingestion endpoint. Please use the OpenTelemetry endpoint instead. Learn more: https://langfuse.com/integrations/native/opentelemetry")]
    Task IngestAsync(LangfuseTrace langfuseTrace, CancellationToken cancellationToken = default);
}